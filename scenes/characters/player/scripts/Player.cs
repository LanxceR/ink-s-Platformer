using Godot;
using System;

public partial class Player : CharacterBody2D
{
    #region Constants
    // Get the gravity from the project settings to be synced with RigidBody nodes.
    // Defaults to 980.0f pixels per second (100px = 1m)
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    #endregion

    #region Exported Variables
    [Export]
    public MovementData moveData;

    [Export]
    public AnimatedSprite2D animSprite2D;

    [Export]
    public Timer coyoteTimer;

    [Export]
    public Timer jumpBufferTimer;
    #endregion

    public void ApplyGravity(Vector2 velocity, double delta)
    {
        if (!IsOnFloor())
            velocity.Y += gravity * moveData.gravityScale * (float)delta;
        Velocity = velocity;
    }

    public void HandleAccelerationX(
        Vector2 velocity,
        Vector2 input_axis,
        double delta,
        float acceleration
    )
    {
        if (input_axis != Vector2.Zero)
        {
            velocity.X = Mathf.MoveToward(
                velocity.X,
                input_axis.X * moveData.speed,
                acceleration * (float)delta
            );
        }

        Velocity = velocity;
    }

    public void HandleDecelerationX(
        Vector2 velocity,
        Vector2 input_axis,
        double delta,
        float deceleration
    )
    {
        // If player's current input is different from velocity direction
        if (Mathf.Sign(input_axis.X) != Mathf.Sign(velocity.X) && velocity.X != 0f)
        {
            velocity.X = Mathf.MoveToward(velocity.X, 0, deceleration * (float)delta);
        }

        Velocity = velocity;
    }

    public void HandleCoyoteTime()
    {
        bool justLeftLedge = !IsOnFloor() && Velocity.Y >= 0;
        if (justLeftLedge)
        {
            coyoteTimer.Start();
            GD.Print($"Coyote Time");
        }
    }
}
