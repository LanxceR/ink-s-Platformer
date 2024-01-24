using Godot;
using Godot.Collections;
using System;

/// <summary>
/// Player's air state.
/// </summary>
public partial class PlayerAir : PlayerState
{
    [Export]
    private int _airJumpsCounter;
    private bool doJumpImmediately = false;

    public override void Enter(Dictionary _msg = null)
    {
        player.animSprite2D.Play("jump");

        if (_msg.ContainsKey("coyoteTime"))
            player.coyoteTimer.Start();
        else if (_msg.ContainsKey("doJump"))
            doJumpImmediately = true;

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
        // Jump key has already been pressed from state transition
        if (doJumpImmediately)
        {
            GD.Print("Jump Immediately");
            // Jump
            velocity.Y = player.moveData.jumpVelocity;
            doJumpImmediately = false;
        }
        // Jump key is pressed when falling/mid-air
        else if (Input.IsActionJustPressed("jump"))
        {
            if (player.coyoteTimer.TimeLeft > 0)
            {
                GD.Print("Coyote Jump");
                // Jump
                velocity.Y = player.moveData.jumpVelocity;
                player.coyoteTimer.Stop();
                player.jumpBufferTimer.Stop();
            }
            else if (_airJumpsCounter > 0)
            {
                GD.Print("Air Jump");
                // Jump at 0.8x velocity
                velocity.Y = player.moveData.jumpVelocity * 0.8f;
                _airJumpsCounter--;
            }
            else
            {
                GD.Print("Buffer Start");
                // Jump was not possible, start jump buffer
                player.jumpBufferTimer.Start();
            }
        }

        // Divide current velocity by half if button is released mid jumping for a shorter jump peak.
        if (Input.IsActionJustReleased("jump") && velocity.Y < 0)
            velocity.Y = velocity.Y / 2;

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
