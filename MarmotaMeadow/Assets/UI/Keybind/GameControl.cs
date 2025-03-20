using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

[System.Serializable]
public class ControllerButton
{
    public enum Type
    {
        A, B, X, Y
    }

    [SerializeField] public Type WhatButton;

    public ButtonControl GetButtonControl()
    {
        return WhatButton switch
        {
            Type.A => Gamepad.current.aButton,
            Type.B => Gamepad.current.bButton,
            Type.X => Gamepad.current.xButton,
            Type.Y => Gamepad.current.yButton,
            _ => null,
        };
    }
}

public class GameControl
{
    [SerializeField] private KeyCode m_key;
    [SerializeField] private ControllerButton m_controllerButton;

    public GameControl(KeyCode key)
    {
        m_key = key;
        m_controllerButton = null;
    }

    public GameControl(ControllerButton.Type controllerButton)
    {
        m_key = KeyCode.None;
        m_controllerButton = new()
        {
            WhatButton = controllerButton
        };
    }

    public bool GetKey()
    {
        if (m_key == KeyCode.None)
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
        if (m_key == KeyCode.None)
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
        if (m_key != KeyCode.None) return m_key.ToString();
        else return m_controllerButton.WhatButton.ToString();
    }

    public static List<GameControl> GetJustPressed()
    {
        List<GameControl> controls = new();
        foreach (ControllerButton.Type button in System.Enum.GetValues(typeof(ControllerButton.Type)))
        {
            ControllerButton controllerButton = new()
            {
                WhatButton = button
            };

            if (controllerButton.GetButtonControl().wasPressedThisFrame)
            {
                controls.Add(new(button));
            }
        }
        return controls;
    }
}