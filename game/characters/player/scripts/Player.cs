using Godot;
using System;
using System.Linq;

public partial class Player : CharacterBody2D
{
    #region Exported Variables
    [ExportGroup("Movement")]
    [Export]
    public MovementData moveData;

    [Export]
    public Timer coyoteTimer;

    [Export]
    public Timer wallJumpRecoveryTimer;

    [ExportGroup("Collisions")]
    [Export]
    public Area2D hazardDetectionArea;

    [ExportGroup("Animations")]
    [Export]
    public AnimationPlayer animPlayer;

    [Export]
    public Sprite2D sprite2D;

    [ExportGroup("Misc")]
    [Export]
    public StateMachine stateMachine;
    #endregion

    private Node[] _childNodes;
    private Vector2 spawnPos;
    private Vector2 _wallNormal;
    private bool _shortWallJump;
    private bool _forceMoveX;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    // Defaults to 980.0f pixels per second (100px = 1m)
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public override void _Ready()
    {
        _childNodes = stateMachine.GetChildren().ToArray();

        foreach (Node child in _childNodes)
        {
            if (child is PlayerAir)
                (child as PlayerAir).WallJump += HandleWallJumpTime;
        }

        // Hazard Detection
        hazardDetectionArea.AreaEntered += OnHazardEntered;
        spawnPos = GlobalPosition;
    }

    public override void _Process(double delta)
    {
        //GD.Print(Velocity);
    }

    #region Methods
    /// <summary>
    /// Applies gravity to the player by adding Y velocity according to gravity
    /// acceleration value. Use <c>physics_process</c> for this.
    /// </summary>
    /// <param name="velocity">Current velocity.</param>
    /// <param name="delta">Physics process delta.</param>
    /// <param name="mult">(Optional) Gravity multiplier. Defaults to <c>1f</c>.</param>
    /// <param name="multDuration">(Optional) Duration in seconds of
    /// which said multiplier is applied.</param>
    /// <param name="maxCap">(Optional) Max fall cap. Defaults to <c>0f</c>, meaning default max fall speed.</param>
    /// <param name="maxCapDuration">(Optional) Duration in which maxCap gradually recovers to default max fall speed. Defaults to <c>0f</c>.</param>
    /// <returns>Returns <c>true</c> if player is at max fall velocity, otherwise <c>false</c>.</returns>
    public bool ApplyGravity(
        Vector2 velocity,
        double delta,
        float mult = 1f,
        float multDuration = 0f,
        float maxCap = 0f,
        float maxCapDuration = 0f
    )
    {
        // Apparently Celeste uses .5 gravity for the first couple of moments before Y
        // speed reaches 20, creating a non-perfect parabola curve. Something to think
        // about.
        float grav = gravity * moveData.gravityScale;
        float modGravThreshold = grav * mult * multDuration;
        float normalGrav = grav * (float)delta;
        float finalGrav =
            velocity.Y < modGravThreshold && modGravThreshold != 0 ? normalGrav * mult : normalGrav;

        float amtToAdd = (moveData.maxFallSpeed - maxCap) * (float)delta / maxCapDuration;
        float maxFall =
            maxCap != 0f && velocity.Y >= maxCap
                ? Mathf.MoveToward(velocity.Y, moveData.maxFallSpeed, amtToAdd)
                : moveData.maxFallSpeed;

        if (!IsOnFloor())
            velocity.Y = Mathf.MoveToward(velocity.Y, maxFall, finalGrav);
        Velocity = velocity;

        if (Velocity.Y >= moveData.maxFallSpeed)
            return true;
        else
            return false;
    }

    public void HandleAccelerationX(
        Vector2 velocity,
        Vector2 input_axis,
        double delta,
        float acceleration
    )
    {
        if (wallJumpRecoveryTimer.TimeLeft > 0)
        {
            // Recovering from wall slide jump.
            velocity.X = Mathf.MoveToward(
                velocity.X,
                _wallNormal.X * moveData.maxSpeed,
                acceleration * (float)delta
            );
        }
        else
        {
            _forceMoveX = false;
            velocity.X = Mathf.MoveToward(
                velocity.X,
                input_axis.X * moveData.maxSpeed,
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
        if (Mathf.Sign(input_axis.X) != Mathf.Sign(velocity.X) && velocity.X != 0f && !_forceMoveX)
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
        }
    }

    public void HandleWallJumpTime(bool shortWallJump, Vector2 wallNormal)
    {
        wallJumpRecoveryTimer.Start();
        _forceMoveX = true;
        _shortWallJump = shortWallJump;
        _wallNormal = wallNormal;
    }

    private void OnHazardEntered(Area2D area)
    {
        GD.Print("DAMAGE");
        GlobalPosition = spawnPos;
    }
    #endregion
}
