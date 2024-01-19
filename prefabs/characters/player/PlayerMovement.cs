using Godot;
using System;
using System.Diagnostics;

public partial class PlayerMovement : CharacterBody2D
{
    #region Constants
    // Get the gravity from the project settings to be synced with RigidBody nodes.
    // Defaults to 980.0f pixels per second (100px = 1m)
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    #endregion

    #region Exported Variables
    [Export]
    private MovementData _movementData;

    [Export]
    private AnimatedSprite2D _animSprite2D;

    [Export]
    private Timer _coyoteTimer;

    [Export]
    private Timer _jumpBufferTimer;
    #endregion

    private int _airJumpsCounter;

    // By default, delta is 0.01666... (60 calc/sec).
    public override void _PhysicsProcess(double delta)
    {
        ApplyGravity(Velocity, delta);
        if (!HandleWallJump(Velocity))
            HandleJump(Velocity);
        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 inputAxis = Input.GetVector("move_left", "move_right", "ui_up", "ui_down");
        HandleAccelerationX(Velocity, inputAxis, delta);
        HandleDecelerationX(Velocity, inputAxis, delta);
        UpdateAnimation(inputAxis);

        bool wasOnFloor = IsOnFloor();
        MoveAndSlide();
        HandleCoyoteTime(wasOnFloor);
    }

    #region Methods
    private void ApplyGravity(Vector2 velocity, double delta)
    {
        if (!IsOnFloor())
            velocity.Y += gravity * _movementData.gravityScale * (float)delta;
        Velocity = velocity;
    }

    private bool HandleWallJump(Vector2 velocity)
    {
        bool nearWall =
            TestMove(GlobalTransform, Vector2.Left)
            || TestMove(GlobalTransform, Vector2.Right);

        GD.Print(nearWall);

        if (!nearWall || IsOnFloor())
            return false;

        _airJumpsCounter = _movementData.airJumps;

        // Normal is a vector that points away from the wall
        Vector2 wallNormal = GetWallNormal();

        if (Input.IsActionJustPressed("jump") || _jumpBufferTimer.TimeLeft > 0)
        {
            velocity = new Vector2(wallNormal.X * _movementData.speed, _movementData.jumpVelocity);
            GD.Print("Wall Jumped");
        }

        Velocity = velocity;
        return true;
    }

    private void HandleJump(Vector2 velocity)
    {
        if (IsOnFloor())
            _airJumpsCounter = _movementData.airJumps;

        if (IsOnFloor() || _coyoteTimer.TimeLeft > 0)
        {
            // Jump full height first.
            if (Input.IsActionJustPressed("jump") || _jumpBufferTimer.TimeLeft > 0)
                velocity.Y = _movementData.jumpVelocity;
        }
        else if (!IsOnFloor())
        {
            // Divide current velocity by half if button is released mid jumping for a shorter jump peak.
            if (Input.IsActionJustReleased("jump") && velocity.Y < 0)
                velocity.Y = velocity.Y / 2;

            if (Input.IsActionJustPressed("jump"))
            {
                // Air jumps
                if (_airJumpsCounter > 0)
                {
                    velocity.Y = _movementData.jumpVelocity * 0.8f;
                    _airJumpsCounter--;
                }
                else
                    // Jump was not possible, start jump buffer
                    _jumpBufferTimer.Start();
            }
        }
        Velocity = velocity;
    }

    private void HandleAccelerationX(Vector2 velocity, Vector2 input_axis, double delta)
    {
        if (input_axis != Vector2.Zero)
        {
            if (IsOnFloor())
                velocity.X = Mathf.MoveToward(
                    velocity.X,
                    input_axis.X * _movementData.speed,
                    _movementData.groundAcceleration * (float)delta
                );
            else
                velocity.X = Mathf.MoveToward(
                    velocity.X,
                    input_axis.X * _movementData.speed,
                    _movementData.airAcceleration * (float)delta
                );
        }
        Velocity = velocity;
    }

    private void HandleDecelerationX(Vector2 velocity, Vector2 input_axis, double delta)
    {
        // If player's current input is different from velocity direction
        if (Mathf.Sign(input_axis.X) != Mathf.Sign(velocity.X) && velocity.X != 0f)
        {
            if (IsOnFloor())
                velocity.X = Mathf.MoveToward(
                    velocity.X,
                    0,
                    _movementData.groundDeceleration * (float)delta
                );
            else
                velocity.X = Mathf.MoveToward(
                    velocity.X,
                    0,
                    _movementData.airDeceleration * (float)delta
                );
        }

        Velocity = velocity;
    }

    private void UpdateAnimation(Vector2 input_axis)
    {
        if (input_axis.X != 0)
        {
            _animSprite2D.Play("run");
            _animSprite2D.FlipH = input_axis.X < 0;
        }
        else
            _animSprite2D.Play("idle");

        if (!IsOnFloor())
            _animSprite2D.Play("jump");
    }

    private void HandleCoyoteTime(bool wasOnFloor)
    {
        bool justLeftLedge = wasOnFloor && !IsOnFloor() && Velocity.Y >= 0;
        if (justLeftLedge)
            _coyoteTimer.Start();
    }
    #endregion

    #region Debugging
    private void PrintPosition()
    {
        GD.Print($"Position = X: {GlobalPosition.X} | Y: {GlobalPosition.Y}");
    }

    private void PrintVelocity()
    {
        GD.Print($"Velocity = X: {Velocity.X} | Y: {Velocity.Y}");
    }
    #endregion
}
