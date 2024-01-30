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
    private static Dictionary<string, double> _keyboardTimers;
    private static Dictionary<string, double> _mouseButtonTimers;
    private static Dictionary<string, double> _joypadTimers;
    private static Dictionary<JoyAxis, double> _joystickTimers;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Inherit;

        // Initialize dictionaries.
        _keyboardTimers = new Dictionary<string, double>();
        _mouseButtonTimers = new Dictionary<string, double>();
        _joypadTimers = new Dictionary<string, double>();
        _joystickTimers = new Dictionary<JoyAxis, double>();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Countdown timers
        foreach (KeyValuePair<string, double> k in _keyboardTimers)
            if (_keyboardTimers[k.Key] > 0)
                _keyboardTimers[k.Key] = Mathf.MoveToward(k.Value, 0, delta);

        foreach (KeyValuePair<string, double> mb in _mouseButtonTimers)
            if (_mouseButtonTimers[mb.Key] > 0)
                _mouseButtonTimers[mb.Key] = Mathf.MoveToward(mb.Value, 0, delta);

        foreach (KeyValuePair<string, double> jb in _joypadTimers)
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
        if (@event is InputEventKey eventKey)
        {
            if (eventKey.IsEcho())
                return;
                
            Key k = eventKey.PhysicalKeycode;

            if (eventKey.Pressed)
            {
                if (_keyboardTimers.ContainsKey($"{k}_pressed"))
                    // Set the most recent timestamp of a key.
                    _keyboardTimers[$"{k}_pressed"] = BUFFER_WINDOW;
                else
                    // Add entry of the key and it's timestamp.
                    _keyboardTimers.Add($"{k}_pressed", BUFFER_WINDOW);
            }
            else
            {
                if (_keyboardTimers.ContainsKey($"{k}_released"))
                    // Set the most recent timestamp of a key.
                    _keyboardTimers[$"{k}_released"] = BUFFER_WINDOW;
                else
                    // Add entry of the key and it's timestamp.
                    _keyboardTimers.Add($"{k}_released", BUFFER_WINDOW);
            }
        }
        // Mouse Button
        else if (@event is InputEventMouseButton eventMouseButton)
        {
            MouseButton mb = eventMouseButton.ButtonIndex;

            if (eventMouseButton.Pressed)
            {
                if (_keyboardTimers.ContainsKey($"{mb}_pressed"))
                    // Set the most recent timestamp of a mouse button.
                    _keyboardTimers[$"{mb}_pressed"] = BUFFER_WINDOW;
                else
                    // Add entry of the key and it's timestamp.
                    _keyboardTimers.Add($"{mb}_pressed", BUFFER_WINDOW);
            }
            else
            {
                if (_keyboardTimers.ContainsKey($"{mb}_released"))
                    // Set the most recent timestamp of a mouse button.
                    _keyboardTimers[$"{mb}_released"] = BUFFER_WINDOW;
                else
                    // Add entry of the key and it's timestamp.
                    _keyboardTimers.Add($"{mb}_released", BUFFER_WINDOW);
            }
        }
        // Joypad
        else if (@event is InputEventJoypadButton eventJoypadButton)
        {
            JoyButton jb = eventJoypadButton.ButtonIndex;

            if (eventJoypadButton.Pressed)
            {
                if (_keyboardTimers.ContainsKey($"{jb}_pressed"))
                    // Set the most recent timestamp of a joy button.
                    _keyboardTimers[$"{jb}_pressed"] = BUFFER_WINDOW;
                else
                    // Add entry of the key and it's timestamp.
                    _keyboardTimers.Add($"{jb}_pressed", BUFFER_WINDOW);
            }
            else
            {
                if (_keyboardTimers.ContainsKey($"{jb}_released"))
                    // Set the most recent timestamp of a joy button.
                    _keyboardTimers[$"{jb}_released"] = BUFFER_WINDOW;
                else
                    // Add entry of the key and it's timestamp.
                    _keyboardTimers.Add($"{jb}_released", BUFFER_WINDOW);
            }
        }
        // Joystick
        else if (@event is InputEventJoypadMotion eventJoypadMotion)
        {
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
            if (@event is InputEventKey eventKey)
            {
                Key k = eventKey.PhysicalKeycode;

                if (_keyboardTimers.ContainsKey($"{k}_pressed"))
                    if (_keyboardTimers[$"{k}_pressed"] > 0)
                    {
                        return true;
                    }
            }
            // Mouse Button
            else if (@event is InputEventMouseButton eventMouseButton)
            {
                MouseButton mb = eventMouseButton.ButtonIndex;

                if (_mouseButtonTimers.ContainsKey($"{mb}_pressed"))
                    if (_mouseButtonTimers[$"{mb}_pressed"] > 0)
                    {
                        return true;
                    }
            }
            // Joypad
            else if (@event is InputEventJoypadButton eventJoypadButton)
            {
                JoyButton jb = eventJoypadButton.ButtonIndex;

                if (_joypadTimers.ContainsKey($"{jb}_pressed"))
                    if (_joypadTimers[$"{jb}_pressed"] > 0)
                    {
                        return true;
                    }
            }
            // Joystick
            else if (@event is InputEventJoypadMotion eventJoypadMotion)
            {
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

    /// <summary>
    /// Returns whether any of the keys/buttons in the given action were released within the buffer window.
    /// </summary>
    /// <param name="action">The action to check for in the buffer.</param>
    /// <returns>
    ///  True if any of the action's associated keys/buttons were released within the buffer window, false otherwise.
    /// </returns>
    public static bool IsActionJustReleased(string action)
    {
        foreach (InputEvent @event in InputMap.ActionGetEvents(action))
        {
            // Keyboard
            if (@event is InputEventKey eventKey)
            {
                Key k = eventKey.PhysicalKeycode;

                if (_keyboardTimers.ContainsKey($"{k}_released"))
                    if (_keyboardTimers[$"{k}_released"] > 0)
                    {
                        return true;
                    }
            }
            // Mouse Button
            else if (@event is InputEventMouseButton eventMouseButton)
            {
                MouseButton mb = eventMouseButton.ButtonIndex;

                if (_mouseButtonTimers.ContainsKey($"{mb}_released"))
                    if (_mouseButtonTimers[$"{mb}_released"] > 0)
                    {
                        return true;
                    }
            }
            // Joypad
            else if (@event is InputEventJoypadButton eventJoypadButton)
            {
                JoyButton jb = eventJoypadButton.ButtonIndex;

                if (_joypadTimers.ContainsKey($"{jb}_released"))
                    if (_joypadTimers[$"{jb}_released"] > 0)
                    {
                        return true;
                    }
            }
            // Joystick
            else if (@event is InputEventJoypadMotion eventJoypadMotion)
            {
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
