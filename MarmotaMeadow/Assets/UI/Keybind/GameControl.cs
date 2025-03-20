using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

[System.Serializable]
public class ControllerButton
{

    [SerializeField] public GamepadButton WhatButton;

    public ButtonControl GetButtonControl()
    {
       return Gamepad.current[WhatButton];
    }
}

public class GameControl
{
    [SerializeField] private KeyCode m_key;
    [SerializeField] private ControllerButton m_controllerButton;

    public GameControl(KeyCode key, GamepadButton controllerButton)
    {
        m_key = key;
        m_controllerButton = new() { WhatButton = controllerButton };
    }

    public bool GetKey()
    {
        if (Gamepad.current != null)
        {
            return m_controllerButton.GetButtonControl().isPressed;
        }
        else
        {
            return Input.GetKey(m_key);
        }
    }

    public bool GetKeyUp()
    {
        if (Gamepad.current != null)
        {
            return m_controllerButton.GetButtonControl().wasReleasedThisFrame;
        }
        else
        {
            return Input.GetKeyUp(m_key);
        }
    }

    public bool GetKeyDown()
    {
        if (m_key == KeyCode.None)
        {
            return m_controllerButton.GetButtonControl().wasPressedThisFrame;
        }
        else
        {
            return Input.GetKeyDown(m_key);
        }
    }

    public override string ToString()
    {
        if (Gamepad.current == null) return m_key.ToString();
        else return m_controllerButton.WhatButton.ToString();
    }

    public void SetKey(KeyCode key)
    {
        m_key = key;
    }

    public void SetControllerButton(GamepadButton button)
    {
        m_controllerButton = new() { WhatButton = button };
    }

    public static List<GamepadButton> GetJustPressed()
    {
        List<GamepadButton> controls = new();
        if (Gamepad.current == null) return controls;
        foreach (GamepadButton button in System.Enum.GetValues(typeof(GamepadButton)))
        {
            ControllerButton controllerButton = new()
            {
                WhatButton = button
            };

            if (controllerButton.GetButtonControl().wasPressedThisFrame)
            {
                controls.Add(button);
            }
        }
        return controls;
    }
}