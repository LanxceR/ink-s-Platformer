using Godot;
using System;
using System.Threading.Tasks;

public partial class TestWorld : Node
{
    [Export]
    private Control _levelCompletedScreen;

    [Export]
    private PackedScene _nextLevelScene;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("Hello C#!");

        // Connect to Events autoload signal for level complete
        GetNode<Events>("/root/Events").LevelCompleted += OnLevelComplete;
    }

    private async void OnLevelComplete()
    {
        // Check if this instance is still valid (not disposed off of from scene change).
        if (!IsInstanceValid(this))
            return;

        _levelCompletedScreen.Show();

        if (_nextLevelScene is not PackedScene)
            return;

        GetTree().Paused = true;
        await ToSignal(GetTree().CreateTimer(1f), Timer.SignalName.Timeout);
        LevelTransition lt = GetNode<LevelTransition>("/root/LevelTransition");
        await lt.FadeToBlack();
        GetTree().Paused = false;

        // Disconnect from signals.
        GetNode<Events>("/root/Events").LevelCompleted -= OnLevelComplete;
        GetTree().ChangeSceneToPacked(_nextLevelScene);
        await lt.FadeFromBlack();
    }
}
