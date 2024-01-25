using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Keeps track of recent inputs in order to make timing windows more flexible.
/// Use this script as an autoload in Project Settings.
/// </summary>
public partial class InputBuffer : Node
{
    /// <summary>
    /// How many milliseconds ahead of time the player can make an input and have it still be recognized.
    /// 0.083 = ~5 frames in 60 fps.
    /// </summary>
    private static readonly double BUFFER_WINDOW = 0.083;

    /// <summary>
    /// Godot has a 0.5f default deadzone.
    /// </summary>
    private static readonly float JOY_DEADZONE = 0.5f;

    // Dictionaries for storing when a key was last pressed.
    private static Dictionary<Key, double> _keyboardTimers;
    private static Dictionary<MouseButton, double> _mouseButtonTimers;
    private static Dictionary<JoyButton, double> _joypadTimers;
    private static Dictionary<JoyAxis, double> _joystickTimers;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Inherit;

        // Initialize dictionaries.
        _keyboardTimers = new Dictionary<Key, double>();
        _mouseButtonTimers = new Dictionary<MouseButton, double>();
        _joypadTimers = new Dictionary<JoyButton, double>();
        _joystickTimers = new Dictionary<JoyAxis, double>();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Countdown timers
        foreach (KeyValuePair<Key, double> k in _keyboardTimers)
            if (_keyboardTimers[k.Key] > 0)
                _keyboardTimers[k.Key] = Mathf.MoveToward(k.Value, 0, delta);

        foreach (KeyValuePair<MouseButton, double> mb in _mouseButtonTimers)
            if (_mouseButtonTimers[mb.Key] > 0)
                _mouseButtonTimers[mb.Key] = Mathf.MoveToward(mb.Value, 0, delta);

        foreach (KeyValuePair<JoyButton, double> jb in _joypadTimers)
            if (_joypadTimers[jb.Key] > 0)
                _joypadTimers[jb.Key] = Mathf.MoveToward(jb.Value, 0, delta);

        foreach (KeyValuePair<JoyAxis, double> ja in _joystickTimers)
            if (_joystickTimers[ja.Key] > 0)
                _joystickTimers[ja.Key] = Mathf.MoveToward(ja.Value, 0, delta);
    }

    // Called whenever the player makes an input.
    public override void _Input(InputEvent @event)
    {
        // Get the input and store it in the dictionary.
        // Keyboard
        if (@event is InputEventKey)
        {
            InputEventKey eventKey = @event as InputEventKey;
            if (!eventKey.Pressed || eventKey.IsEcho())
                return;

            Key k = eventKey.PhysicalKeycode;

            if (_keyboardTimers.ContainsKey(k))
                // Set the most recent timestamp of a key.
                _keyboardTimers[k] = BUFFER_WINDOW;
            else
                // Add entry of the key and it's timestamp.
                _keyboardTimers.Add(k, BUFFER_WINDOW);
        }
        // Mouse Button
        else if (@event is InputEventMouseButton)
        {
            InputEventMouseButton eventMouseButton = @event as InputEventMouseButton;
            if (!eventMouseButton.Pressed || eventMouseButton.IsEcho())
                return;

            MouseButton mb = eventMouseButton.ButtonIndex;

            if (_mouseButtonTimers.ContainsKey(mb))
                // Set the most recent timestamp of a button.
                _mouseButtonTimers[mb] = BUFFER_WINDOW;
            else
                // Add entry of the button and it's timestamp.
                _mouseButtonTimers.Add(mb, BUFFER_WINDOW);
        }
        // Joypad
        else if (@event is InputEventJoypadButton)
        {
            InputEventJoypadButton eventJoypadButton = @event as InputEventJoypadButton;
            if (!eventJoypadButton.Pressed || eventJoypadButton.IsEcho())
                return;

            JoyButton jb = eventJoypadButton.ButtonIndex;

            if (_joypadTimers.ContainsKey(jb))
                // Set the most recent timestamp of a button.
                _joypadTimers[jb] = BUFFER_WINDOW;
            else
                // Add entry of the button and it's timestamp.
                _joypadTimers.Add(jb, BUFFER_WINDOW);
        }
        // Joystick
        else if (@event is InputEventJoypadMotion)
        {
            InputEventJoypadMotion eventJoypadMotion = @event as InputEventJoypadMotion;
            if (Math.Abs(eventJoypadMotion.AxisValue) < JOY_DEADZONE)
                return;

            JoyAxis ja = eventJoypadMotion.Axis;

            if (_joystickTimers.ContainsKey(ja))
                // Set the most recent timestamp of a button.
                _joystickTimers[ja] = BUFFER_WINDOW;
            else
                // Add entry of the button and it's timestamp.
                _joystickTimers.Add(ja, BUFFER_WINDOW);
        }
    }

    /// <summary>
    /// Returns whether any of the keys/buttons in the given action were pressed within the buffer window.
    /// </summary>
    /// <param name="action">The action to check for in the buffer.</param>
    /// <returns>
    ///  True if any of the action's associated keys/buttons were pressed within the buffer window, false otherwise.
    /// </returns>
    public static bool IsActionJustPressed(string action)
    {
        foreach (InputEvent @event in InputMap.ActionGetEvents(action))
        {
            // Keyboard
            if (@event is InputEventKey)
            {
                InputEventKey eventKey = @event as InputEventKey;
                Key k = eventKey.PhysicalKeycode;

                if (_keyboardTimers.ContainsKey(k))
                    if (_keyboardTimers[k] > 0)
                    {
                        return true;
                    }
            }
            // Mouse Button
            else if (@event is InputEventMouseButton)
            {
                InputEventMouseButton eventMouseButton = @event as InputEventMouseButton;
                MouseButton mb = eventMouseButton.ButtonIndex;

                if (_mouseButtonTimers.ContainsKey(mb))
                    if (_mouseButtonTimers[mb] > 0)
                    {
                        return true;
                    }
            }
            // Joypad
            else if (@event is InputEventJoypadButton)
            {
                InputEventJoypadButton eventJoypadButton = @event as InputEventJoypadButton;
                JoyButton b = eventJoypadButton.ButtonIndex;

                if (_joypadTimers.ContainsKey(b))
                    if (_joypadTimers[b] > 0)
                    {
                        return true;
                    }
            }
            // Joystick
            else if (@event is InputEventJoypadMotion)
            {
                InputEventJoypadMotion eventJoypadMotion = @event as InputEventJoypadMotion;
                if (Math.Abs(eventJoypadMotion.AxisValue) < JOY_DEADZONE)
                    return false;

                JoyAxis ja = eventJoypadMotion.Axis;

                if (_joystickTimers.ContainsKey(ja))
                    if (_joystickTimers[ja] > 0)
                    {
                        return true;
                    }
            }
        }

        return false;
    }    
}
