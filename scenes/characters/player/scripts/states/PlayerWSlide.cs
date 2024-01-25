using Godot;
using Godot.Collections;
using System;

public partial class PlayerWSlide : PlayerState
{
    public override void Enter(Dictionary _msg = null)
    {
        // TODO: Animation here
        GD.Print("Wall Slide");
    }

    public override void PhysicsProcess(double _delta)
    {
        player.ApplyGravity(
            player.Velocity,
            _delta,
            player.moveData.wSlideGravScale,
            player.moveData.wSlideClingDuration
        );

        Vector2 inputAxis = Input.GetVector("move_left", "move_right", "ui_up", "ui_down");
        HandleWallJump(inputAxis);
    }

    #region Method
    private void HandleWallJump(Vector2 inputAxis)
    {
        // Normal is a vector that points away from the wall
        Vector2 wallNormal = player.GetWallNormal(); 

        // TODO: Wall jump

        return;
    }
    #endregion
}
