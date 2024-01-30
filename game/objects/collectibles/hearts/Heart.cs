using Godot;
using System;
using System.Linq;

public partial class Heart : Area2D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        QueueFree();

        Node[] hearts = GetTree().GetNodesInGroup("Hearts").ToArray();
        if (hearts.Length <= 1)
        {
            // This is the last heart.
            GD.Print("Level Completed");
            GetNode<Events>("/root/Events").EmitSignal(Events.SignalName.LevelCompleted);
        }
    }
}
