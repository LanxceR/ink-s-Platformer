using Godot;
using Microsoft.VisualBasic;
using System;
using System.Threading.Tasks;

public partial class LevelTransition : Node
{
    [Export]
    private AnimationPlayer _animPlayer;

    public async Task FadeToBlack()
    {
        _animPlayer.Play("fade_to_black");
        await ToSignal(_animPlayer, AnimationPlayer.SignalName.AnimationFinished);
    }

    public async Task FadeFromBlack()
    {
        _animPlayer.Play("fade_from_black");
        await ToSignal(_animPlayer, AnimationPlayer.SignalName.AnimationFinished);
    }
}
