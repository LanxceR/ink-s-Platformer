using Godot;
using System;

/// <summary>
/// State derived boilerplate class for player states, for autocompletions and <c>Player</c> type checks.
/// </summary>
public partial class PlayerState : State
{
	public Player player;

    // Called when the node enters the scene tree for the first time.
    public override async void _Ready()
    {
        await ToSignal(Owner, SignalName.Ready);

        player = GetOwner<Player>();
        
        // Logs error if there if the root node is not Player.
        Debug.Assert(player != null, $"{player}");
    }
}
