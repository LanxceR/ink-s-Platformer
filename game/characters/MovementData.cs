using Godot;
using System;

[GlobalClass]
public partial class MovementData : Resource
{
    /// <summary>
    /// Velocity in pixels per second.
    /// </summary>
    [Export]
    public float maxSpeed = 100.0f;

    /// <summary>
    /// Acceleration in <c>px/s^2</c>, on the ground.
    /// </summary>
    [Export]
    public float groundAcceleration = 1200.0f;

    /// <summary>
    /// Deceleration/friction in <c>px/s^2</c>, on the ground.
    /// </summary>
    [Export]
    public float groundDeceleration = 1200.0f;

    /// <summary>
    /// Acceleration in <c>px/s^2</c>, in the air.
    /// </summary>
    [Export]
    public float airAcceleration = 600.0f;

    /// <summary>
    /// Deceleration/friction in <c>px/s^2</c>, in the air.
    /// </summary>
    [Export]
    public float airDeceleration = 900.0f;

    /// <summary>
    /// Jump velocity in pixels per second. Calculated as <c>v = sqrt(2gh)</c>.
    /// For example: <c>-300.0f px/s = ~45.918px peak height.</c>
    /// </summary>
    [Export]
    public float jumpVelocity = -260.0f;

    /// <summary>
    /// Gravity scale.
    /// </summary>
    [Export]
    public float gravityScale = 1f;

    /// <summary>
    /// Maximum fall speed.
    /// </summary>
    [Export]
    public float maxFallSpeed = 200f;

    /// <summary>
    /// Wall slide gravity scale.
    /// </summary>
    [Export]
    public float wSlideStrongGravScale = .5f;

    /// <summary>
    /// Wall slide cling duration.
    /// </summary>
    [Export]
    public float wSlideStrongDuration = .5f;

    /// <summary>
    /// Wall slide cling duration.
    /// </summary>
    [Export]
    public float wSlideDuration = 1.2f;

    /// <summary>
    /// Wall slide cling duration.
    /// </summary>
    [Export]
    public float wSlideInitialMaxFallSpeed = 20f;

    /// <summary>
    /// Wall jump horizontal speed.
    /// </summary>
    [Export]
    public float wJumpXSpeed = 140.0f;

    /// <summary>
    /// Wall jump vertical speed.
    /// </summary>
    [Export]
    public float wJumpYSpeed = -260.0f;

    /// <summary>
    /// Midair jumps.
    /// </summary>
    [Export]
    public int airJumps = 0;
}
