using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem.LowLevel;
using System.Runtime.CompilerServices;
using UnityEngine.InputSystem;
using Unity.VisualScripting.FullSerializer;

public static class GameInput
{
    private static readonly Dictionary<string, GameControl> m_defaultKeys = new Dictionary<string, GameControl>
            {
                { "Interact", new(KeyCode.Mouse0, GamepadButton.A) },
                { "OpenInventory", new(KeyCode.E, GamepadButton.Select) },
                { "Pause", new(KeyCode.Escape, GamepadButton.Start) },
                { "ExitMinigame", new(KeyCode.Mouse1, GamepadButton.Y) },
                { "Reload", new(KeyCode.R, GamepadButton.X) }
        };

    internal static Dictionary<string, GameControl> m_keycodes = new Dictionary<string, GameControl>();
    internal static float m_framesSinceKeybindChange = 0;

    /// <summary>
    /// Get the direction a player is trying to move (WASD or left joystick)
    /// </summary>
    /// <returns>Not normalised direction</returns>
    public static Vector2 GetPlayerMovementInputDirection()
    {
        if (Gamepad.current == null)
        {
            Vector2 dir = Vector2.zero;
            if (Input.GetKey(KeyCode.W)) dir.y++;
            if (Input.GetKey(KeyCode.S)) dir.y--;
            if (Input.GetKey(KeyCode.D)) dir.x++;
            if (Input.GetKey(KeyCode.A)) dir.x--;
            return dir;
        }
        else
        {
            return Gamepad.current.leftStick.value;
        }
    }

    /// <summary>
    /// Check if keybinds changed
    /// </summary>
    /// <returns>True if the keybinds changed this frame or last frame, false if they didn't</returns>
    public static bool DidKeybindsChange()
    {
        return m_framesSinceKeybindChange <= 2;
    }

    /// <summary>
    /// Get the keycode associated with an action (the action name is the game object name, case sensitive)
    /// </summary>
    /// <param name="actionName">Action name</param>
    /// <returns>The control or the default control if none is bound, or null if the action is invalid</returns>
    public static GameControl GetKeybind(string actionName)
    {
        if (m_keycodes.ContainsKey(actionName)) return m_keycodes[actionName];
        else
        {
            GameControl key = null;
            try
            {
                key = JsonUtility.FromJson<GameControl>(PlayerPrefs.GetString("action_keybind." + actionName));
            }
            catch (Exception) { };
            if (key == null)
            {
                if (m_defaultKeys.ContainsKey(actionName)) key = m_defaultKeys[actionName];
                else Debug.LogError("Missing key code for action: " + actionName);
            }
            m_keycodes[actionName] = key;
            return key;
        }
    }
}