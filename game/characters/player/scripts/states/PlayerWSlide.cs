using Godot;
using Godot.Collections;
using System;

public partial class PlayerWSlide : PlayerState
{
    // Normal is a vector that points away from the wall
    private Vector2 _wallNormal;

    public override void Enter(Dictionary _msg = null)
    {
        // TODO: Animation here
        _wallNormal = player.GetWallNormal();

        // Reset Y velocity to prevent player from jumping after letting go of wall
        player.Velocity = new Vector2(player.Velocity.X, 0f);
    }

    public override void PhysicsProcess(double _delta)
    {
        Vector2 inputAxis = new Vector2(Input.GetAxis("move_left", "move_right"), 0);

        HandleWallSlide(inputAxis, _delta);

        player.MoveAndSlide();

        CheckState(inputAxis);
    }

    #region Method
    private void HandleWallSlide(Vector2 inputAxis, double delta)
    {
        if (Mathf.Sign(inputAxis.X) == -_wallNormal.X)
            player.ApplyGravity(
                player.Velocity,
                delta,
                player.moveData.wSlideStrongGravScale,
                player.moveData.wSlideStrongDuration,
                player.moveData.wSlideInitialMaxFallSpeed,
                player.moveData.wSlideDuration
            );
    }
    #endregion

    #region Change State
    private void CheckState(Vector2 inputAxis)
    {
        // Let go
        if (Mathf.Sign(inputAxis.X) != -_wallNormal.X || !player.IsOnWallOnly())
            stateMachine.TransitionTo(nameof(PlayerAir));

        // Wall jump
        if (InputBuffer.IsActionJustPressed("jump"))
            stateMachine.TransitionTo(nameof(PlayerAir), new Dictionary { ["doJump"] = true });

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
