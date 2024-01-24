using Godot;
using System;

internal static class Debug
{
    internal static void Assert(bool condition, string msg)
    # if DEBUG
    {
        if (condition)
            return;

        GD.PrintErr(msg);
        throw new ApplicationException($"Assert failed: {msg}");
    }
    # else
    {}
    #endif
}
