using Godot;
using System;

public partial class TestWorld : Node
{
    [Export]
    private Control _levelCompletedScreen;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("Hello C#!");

        // Set clear background to black
        RenderingServer.SetDefaultClearColor(new Color(0, 0, 0, 1));
        
        // Connect to Events autoload signal for level complete
        GetNode<Events>("/root/Events").LevelCompleted += OnLevelComplete;
    }

    private void OnLevelComplete()
    {
        _levelCompletedScreen.Show();
        GetTree().Paused = true;
    }
}
