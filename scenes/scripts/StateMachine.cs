using Godot;
using Godot.Collections;
using System;

/// <summary>
/// Generic state machine. Initializes states and delegates node callbacks [<c>_Process(), _PhysicsProcess()</c>, <c>_UnhandledInput()</c>] to the active state.
/// </summary>
public partial class StateMachine : Node
{
    [Signal]
    public delegate void TransitionedEventHandler(string stateName);

    [Export]
    public NodePath initialState;

    public State state;

    // Called when the node enters the scene tree for the first time.
    public override async void _Ready()
    {
        // Set initial state
        state = GetNode<State>(initialState);

        await ToSignal(Owner, SignalName.Ready);

        // Assign StateMachine to all states
        foreach (State child in GetChildren())
            child.stateMachine = this;

        state.Enter();
    }

    #region Node Callbacks
    //  The state machine subscribes to node callbacks and delegates them to the state objects.
    public override void _UnhandledInput(InputEvent inputEvent)
    {
        state.HandleInput(inputEvent);
    }

    public override void _Process(double delta)
    {
        state.Process(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        state.PhysicsProcess(delta);
        //GD.Print(state.Name);
    }
    #endregion

    /// <summary>
    /// Transition into a state.
    /// </summary>
    /// <param name="targetStateName">Target state's name in <c>string</c>.</param>
    /// <param name="msg">Optional <c>msg</c> dictionary to pass to the target state's <c>Enter()</c> method</param>
    public void TransitionTo(string targetStateName, Dictionary msg = null)
    {
        if (!HasNode(targetStateName))
            return;

        state.Exit();
        state = GetNode<State>(targetStateName);
        state.Enter(msg);
        EmitSignal(SignalName.Transitioned);
    }
}
