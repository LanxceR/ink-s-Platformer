using Godot;
using Godot.Collections;
using System;

public partial class State : Node
{
    public StateMachine stateMachine;

    public virtual void HandleInput(InputEvent _event) { }
    public virtual void Process(double _delta) { }
    public virtual void PhysicsProcess(double _delta) { }
    public virtual void Enter(Dictionary _msg = null) { }
    public virtual void Exit() { }
}
