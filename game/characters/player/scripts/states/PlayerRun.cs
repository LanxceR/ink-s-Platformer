using Godot;
using Godot.Collections;
using System;

/// <summary>
/// Player's run state.
/// </summary>
public partial class PlayerRun : PlayerState
{
    public override void Enter(Dictionary _msg = null)
    {
        player.animSprite2D.Play("run");
    }

    public override void PhysicsProcess(double _delta)
    {
        player.ApplyGravity(player.Velocity, _delta);

        Vector2 inputAxis = new Vector2(Input.GetAxis("move_left", "move_right"), 0);
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

        UpdateAnim(inputAxis);
        player.MoveAndSlide();
        player.HandleCoyoteTime();
        
        CheckState(inputAxis);
    }

    #region Methods
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
        if (!player.IsOnFloor())
            // Fall.
            stateMachine.TransitionTo(nameof(PlayerAir), new Dictionary { ["coyoteTime"] = true });

        if (InputBuffer.IsActionJustPressed("jump"))
            // Jump.
            stateMachine.TransitionTo(nameof(PlayerAir), new Dictionary { ["doJump"] = true });

        if (Mathf.IsEqualApprox(inputAxis.X, 0))
            stateMachine.TransitionTo(nameof(PlayerIdle));
    }
    #endregion
}
