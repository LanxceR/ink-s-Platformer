using Godot;
using Godot.Collections;
using System;

/// <summary>
/// Player's idle state.
/// </summary>
public partial class PlayerIdle : PlayerState
{
    public override void Enter(Dictionary _msg = null)
    {
        player.animSprite2D.Play("idle");
    }

    public override void PhysicsProcess(double _delta)
    {
        player.ApplyGravity(player.Velocity, _delta);

        Vector2 inputAxis = new Vector2(Input.GetAxis("move_left", "move_right"), 0);
        player.HandleDecelerationX(
            player.Velocity,
            inputAxis,
            _delta,
            player.moveData.groundDeceleration
        );

        player.MoveAndSlide();
        player.HandleCoyoteTime();

        CheckState();
    }

    #region Change State
    private void CheckState()
    {
        if (!player.IsOnFloor())
            // Fall.
            stateMachine.TransitionTo(nameof(PlayerAir), new Dictionary { ["coyoteTime"] = true });

        if (InputBuffer.IsActionJustPressed("jump"))
            // Jump.
            stateMachine.TransitionTo(nameof(PlayerAir), new Dictionary { ["doJump"] = true });

        if (Input.GetVector("move_left", "move_right", "ui_up", "ui_down").X != 0f)
            // Do run.
            stateMachine.TransitionTo(nameof(PlayerRun));
    }
    #endregion
}
