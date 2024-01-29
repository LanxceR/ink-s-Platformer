using Godot;
using Godot.Collections;
using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Player's air state.
/// </summary>
public partial class PlayerAir : PlayerState
{
    [Signal]
    public delegate void WallJumpEventHandler(bool shortWallJump, Vector2 wallNormal);

    [Export]
    private int _airJumpsCounter;
    
    private bool _jumpOnEnter = false;
    private bool _canWallJump = false;

    public override void Enter(Dictionary _msg = null)
    {
        player.animSprite2D.Play("jump");

        if (_msg != null)
        {
            if (_msg.ContainsKey("coyoteTime"))
                player.coyoteTimer.Start();
            if (_msg.ContainsKey("doJump"))
                _jumpOnEnter = true;
            if (_msg.ContainsKey("wallJump"))
                _canWallJump = true;
        }

        _airJumpsCounter = player.moveData.airJumps;
    }

    public override void PhysicsProcess(double _delta)
    {
        player.ApplyGravity(player.Velocity, _delta);
        HandleJump(player.Velocity);

        Vector2 inputAxis = Input.GetVector("move_left", "move_right", "ui_up", "ui_down");
        player.HandleAccelerationX(
            player.Velocity,
            inputAxis,
            _delta,
            player.moveData.airAcceleration
        );
        player.HandleDecelerationX(
            player.Velocity,
            inputAxis,
            _delta,
            player.moveData.airDeceleration
        );

        UpdateAnim(inputAxis);
        player.MoveAndSlide();

        CheckState(inputAxis);
    }

    #region Methods
    private void HandleJump(Vector2 velocity)
    {
        bool nearWallLeft = player.TestMove(player.GlobalTransform, Vector2.Left * 2);
        bool nearWallRight = player.TestMove(player.GlobalTransform, Vector2.Right * 2);
        int wallNormalX = nearWallLeft
            ? 1
            : nearWallRight
                ? -1
                : 0;

        // Jump key has already been pressed from state transition.
        if (_jumpOnEnter)
        {
            GD.Print("Jump");
            // Jump
            velocity.Y = player.moveData.jumpVelocity;
            _jumpOnEnter = false;
        }
        // Jump key is pressed when falling/mid-air.
        else if (InputBuffer.IsActionJustPressed("jump"))
        {
            if (wallNormalX != 0)
            {
                GD.Print("Wall Jump");
                // Wall Jump by proximity.
                velocity = new Vector2(
                    wallNormalX * player.moveData.wJumpXSpeed,
                    player.moveData.wJumpYSpeed
                );
                EmitSignal(SignalName.WallJump, false, new Vector2(wallNormalX, 0));
            }
            if (player.coyoteTimer.TimeLeft > 0)
            {
                GD.Print("Coyote Jump");
                // Jump
                velocity.Y = player.moveData.jumpVelocity;
                player.coyoteTimer.Stop();
                _jumpOnEnter = false;
            }
            else if (_airJumpsCounter > 0)
            {
                GD.Print("Air Jump");
                // Jump at 0.8x velocity.
                velocity.Y = player.moveData.jumpVelocity * 0.8f;
                _airJumpsCounter--;
            }
        }

        // Divide current velocity by half if button is released mid air for a shorter jump peak.
        if (Input.IsActionJustReleased("jump") && velocity.Y < 0)
            velocity.Y /= 2;

        player.Velocity = velocity;
    }

    private void UpdateAnim(Vector2 inputAxis)
    {
        if (inputAxis.X != 0)
        {
            player.animSprite2D.FlipH = inputAxis.X < 0;
        }
    }
    #endregion

    #region Change State
    private void CheckState(Vector2 inputAxis)
    {
        // TODO: Maybe implement wall jump based off of proximity to walls instead?
        // Wall Slide
        if (player.IsOnWall() && Mathf.Sign(inputAxis.X) == -player.GetWallNormal().X)
            stateMachine.TransitionTo(
                nameof(PlayerWSlide),
                new Dictionary { ["inputAxis"] = inputAxis }
            );

        // Landing
        if (player.IsOnFloor())
        {
            if (Mathf.IsEqualApprox(inputAxis.X, 0))
                stateMachine.TransitionTo(nameof(PlayerIdle));
            else
                stateMachine.TransitionTo(nameof(PlayerRun));
        }
    }
    #endregion
}
