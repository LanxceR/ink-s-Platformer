using Godot;
using Godot.Collections;
using System;

/// <summary>
/// Player's run state.
/// </summary>
public partial class PlayerRun : PlayerState
{
    public override void PhysicsProcess(double _delta)
    {
        player.ApplyGravity(player.Velocity, _delta);

        Vector2 inputAxis = Input.GetVector("move_left", "move_right", "ui_up", "ui_down");
        player.HandleAccelerationX(
            player.Velocity,
            inputAxis,
            _delta,
            player.moveData.groundAcceleration
        );
        player.HandleDecelerationX(
            player.Velocity,
            inputAxis,
            _delta,
            player.moveData.groundDeceleration
        );

        player.MoveAndSlide();
        player.HandleCoyoteTime();

        CheckState(inputAxis);
    }

    #region Change State
    private void CheckState(Vector2 inputAxis)
    {
        if (!player.IsOnFloor())
            // Fall.
            stateMachine.TransitionTo(nameof(PlayerAir), new Dictionary { ["coyoteTime"] = true });

        if (Input.IsActionJustPressed("jump") || player.jumpBufferTimer.TimeLeft > 0)
            // Jump.
            stateMachine.TransitionTo(nameof(PlayerAir), new Dictionary { ["doJump"] = true });

        if (Mathf.IsEqualApprox(inputAxis.X, 0))
            stateMachine.TransitionTo(nameof(PlayerIdle));
    }
    #endregion
}
