using Godot;
using System;

public partial class Events : Node
{
    [Signal]
    public delegate void LevelCompletedEventHandler();
}
