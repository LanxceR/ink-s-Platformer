using Godot;
using System;
using System.Threading.Tasks;

public partial class StartMenu : CenterContainer
{
    [Export]
    private PackedScene _startGameScene;

    [Export]
    private Button _startGameBtn;

    [Export]
    private Button _quitGameBtn;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Set clear background to black.
        RenderingServer.SetDefaultClearColor(new Color(0, 0, 0, 1));

        _startGameBtn.Pressed += OnStartGameAsync;
        _quitGameBtn.Pressed += OnQuitGame;
        _startGameBtn.GrabFocus();
    }

    private async void OnStartGameAsync()
    {
        if (_startGameScene is not PackedScene)
            return;

        LevelTransition lt = GetNode<LevelTransition>("/root/LevelTransition");
        await lt.FadeToBlack();
        GetTree().ChangeSceneToPacked(_startGameScene);
        await lt.FadeFromBlack();
    }

    private void OnQuitGame()
    {
        GetTree().Quit();
    }
}
