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

    public override void Enter(Dictionary _msg = null)
    {
        if (_msg != null)
        {
            if (_msg.ContainsKey("coyoteTime"))
                player.coyoteTimer.Start();
            if (_msg.ContainsKey("doJump"))
                _jumpOnEnter = true;
        }
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

        player.MoveAndSlide();
        UpdateAnim(inputAxis);

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

        if (Input.IsActionJustPressed("jump") || _jumpOnEnter)
        {
            if (player.IsOnFloor())
            {
                // Jump (NOTE: The IsOnFloor() check is for when the player is entering PlayerAir from the ground,
                // meaning that entering PlayerAir DOES NOT mean that the player is in the air (in this case it's on the ground for 1 frame in PlayerAir), just about to be.)
                GD.Print("Jump");
                velocity.Y = player.moveData.jumpVelocity;
                player.coyoteTimer.Stop();
                _jumpOnEnter = false;
            }
            else if (wallNormalX != 0)
            {
                GD.Print("Wall Jump");
                // Wall Jump by proximity.
                velocity = new Vector2(
                    wallNormalX * player.moveData.wJumpXSpeed,
                    player.moveData.wJumpYSpeed
                );
                _jumpOnEnter = false;
                EmitSignal(SignalName.WallJump, false, new Vector2(wallNormalX, 0));
            }
            else if (player.coyoteTimer.TimeLeft > 0)
            {
                GD.Print("Coyote Jump");
                // Jump
                velocity.Y = player.moveData.jumpVelocity;
                player.coyoteTimer.Stop();
                _jumpOnEnter = false;
            }
            else if (_airJumpsCounter > 0 && !_jumpOnEnter)
            {
                GD.Print("Air Jump");
                // Jump at 0.8x velocity.
                velocity.Y = player.moveData.jumpVelocity * 0.8f;
                _airJumpsCounter--;
            }
        }

        // Divide current velocity by half if button is released mid air for a shorter jump peak.
        if (Input.IsActionJustReleased("jump") && velocity.Y < 0)
        {
            velocity.Y /= 2;
        }

        player.Velocity = velocity;
    }

    private void UpdateAnim(Vector2 inputAxis)
    {
        if (inputAxis.X != 0)
        {
            player.sprite2D.FlipH = inputAxis.X < 0;
        }

        if (player.Velocity.Y < 0 && player.animPlayer.CurrentAnimation != "jump")
        {
            player.animPlayer.Play("jump");
        }
        else if (player.Velocity.Y > 0 && player.animPlayer.CurrentAnimation != "fall")
        {
            player.animPlayer.Play("fall");
        }
        else if (player.IsOnFloor() && player.animPlayer.CurrentAnimation != "land")
        {
            player.animPlayer.Play("land");
        }
    }
    #endregion

    #region Change State
    private void CheckState(Vector2 inputAxis)
    {
        // Wall Slide
        if (
            player.IsOnWallOnly()
            && Mathf.Sign(inputAxis.X) == -player.GetWallNormal().X
            && player.Velocity.Y > 0
        )
            stateMachine.TransitionTo(
                nameof(PlayerWSlide),
                new Dictionary { ["inputAxis"] = inputAxis }
            );

        // Landing
        if (player.IsOnFloor())
        {
            _airJumpsCounter = player.moveData.airJumps;
            if (Mathf.IsEqualApprox(inputAxis.X, 0))
                stateMachine.TransitionTo(
                    nameof(PlayerIdle),
                    new Dictionary { ["land"] = player.animPlayer.CurrentAnimation }
                );
            else
                stateMachine.TransitionTo(
                    nameof(PlayerRun),
                    new Dictionary { ["land"] = player.animPlayer.CurrentAnimation }
                );
        }
    }
    #endregion
}
