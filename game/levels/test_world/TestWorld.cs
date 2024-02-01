using Godot;
using System;
using System.Threading.Tasks;

public partial class TestWorld : Node
{
    [ExportGroup("Interface")]
    [Export]
    private Control _startCountScreen;

    [Export]
    private Label _startCountLabel;

    [Export]
    private AnimationPlayer _startCountAnimPlayer;

    [Export]
    private Label _levelTimerLabel;

    [Export]
    private LevelCompleted _levelCompletedScreen;

    [ExportGroup("")]
    [Export]
    private PackedScene _nextLevelScene;

    private LevelTransition _levelTransition;
    private ulong _levelStartMSec;

    // Called when the node enters the scene tree for the first time.
    public override async void _Ready()
    {
        GD.Print("Hello C#!");

        _levelTransition = GetNode<LevelTransition>("/root/LevelTransition");

        if (_nextLevelScene is not PackedScene){
            _levelCompletedScreen.nextLevelBtn.Text = "Main Menu";
            _nextLevelScene = GD.Load<PackedScene>("res://game/interface/start_menu/start_menu.tscn");
        }

        // Connect to Events autoload signal for level complete & other signals.
        GetNode<Events>("/root/Events").LevelCompleted += OnLevelComplete;
        _levelCompletedScreen.Retry += RetryLevel;
        _levelCompletedScreen.NextLevel += GoToNextLevel;

        GetTree().Paused = true;
        _levelTransition.FadeFromBlack();
        _startCountAnimPlayer.Play("countdown");
        await ToSignal(_startCountAnimPlayer, AnimationPlayer.SignalName.AnimationFinished);
        GetTree().Paused = false;

        _levelStartMSec = Time.GetTicksMsec();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.

    public override void _Process(double delta)
    {
        // Level timer
        ulong t = Time.GetTicksMsec() - _levelStartMSec;
        _levelTimerLabel.Text = $"{t / 1000.0}";
    }

    private void OnLevelComplete()
    {
        _levelCompletedScreen.Show();

        GetTree().Paused = true;
    }

    private async void RetryLevel()
    {
        // Check if this instance is still valid (not disposed off of from scene change).
        if (!IsInstanceValid(this))
            return;

        await ToSignal(GetTree().CreateTimer(1f), Timer.SignalName.Timeout);
        await _levelTransition.FadeToBlack();
        GetTree().Paused = false;

        // Disconnect from signals.
        GetNode<Events>("/root/Events").LevelCompleted -= OnLevelComplete;
        GetTree().ChangeSceneToFile(SceneFilePath);
    }

    private async void GoToNextLevel()
    {
        // Check if this instance is still valid (not disposed off of from scene change).
        if (!IsInstanceValid(this))
            return;

        if (_nextLevelScene is not PackedScene)
            return;

        await ToSignal(GetTree().CreateTimer(1f), Timer.SignalName.Timeout);
        await _levelTransition.FadeToBlack();
        GetTree().Paused = false;

        // Disconnect from signals.
        GetNode<Events>("/root/Events").LevelCompleted -= OnLevelComplete;
        GetTree().ChangeSceneToPacked(_nextLevelScene);
    }
}
