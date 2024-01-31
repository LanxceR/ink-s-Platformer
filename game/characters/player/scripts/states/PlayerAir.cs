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
    private bool _checkInput;

    public override void Enter(Dictionary _msg = null)
    {
        player.animSprite2D.Play("jump");

        if (_msg != null)
        {
            if (_msg.ContainsKey("coyoteTime"))
                player.coyoteTimer.Start();
            if (_msg.ContainsKey("doJump"))
                _jumpOnEnter = true;
        }

        _checkInput = true;
        _airJumpsCounter = player.moveData.airJumps;
    }

    public override void PhysicsProcess(double _delta)
    {
        player.ApplyGravity(player.Velocity, _delta);
        HandleJump(player.Velocity);

        Vector2 inputAxis = new Vector2(Input.GetAxis("move_left", "move_right"), 0);
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
        bool nearWallLeft = player.TestMove(player.GlobalTransform, Vector2.Left * 3);
        bool nearWallRight = player.TestMove(player.GlobalTransform, Vector2.Right * 3);
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
            _checkInput = false;
        }
        // Jump key is pressed when falling/mid-air.
        else if (InputBuffer.IsActionJustPressed("jump") && _checkInput)
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
                _checkInput = false;
            }            
            else if (player.coyoteTimer.TimeLeft > 0)
            {
                GD.Print("Coyote Jump");
                // Jump
                velocity.Y = player.moveData.jumpVelocity;
                player.coyoteTimer.Stop();
                _jumpOnEnter = false;
                _checkInput = false;
            }
            else if (_airJumpsCounter > 0)
            {
                GD.Print("Air Jump");
                // Jump at 0.8x velocity.
                velocity.Y = player.moveData.jumpVelocity * 0.8f;
                _airJumpsCounter--;
                _checkInput = false;
            }
        }

        // Divide current velocity by half if button is released mid air for a shorter jump peak.
        if (Input.IsActionJustReleased("jump") && velocity.Y < 0)
        {
            velocity.Y /= 2;
            _checkInput = true;
        }

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
        // Wall Slide
        if (player.IsOnWallOnly() && Mathf.Sign(inputAxis.X) == -player.GetWallNormal().X && player.Velocity.Y > 0)
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
