using Godot;
using System;

public partial class LevelCompleted : ColorRect
{
	[Signal]
    public delegate void RetryEventHandler();

    [Signal]
    public delegate void NextLevelEventHandler();

    [Export]
    private Button _retryBtn;

    [Export]
    public Button nextLevelBtn;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _retryBtn.Pressed += OnRetry;
        nextLevelBtn.Pressed += OnNextLevel;
        VisibilityChanged += OnBecomeVisible;
    }

    private void OnRetry()
    {
        EmitSignal(SignalName.Retry);
    }

    private void OnNextLevel()
    {
        EmitSignal(SignalName.NextLevel);
    }

    private void OnBecomeVisible()
    {
        if (Visible)
            _retryBtn.GrabFocus();
    }
}
