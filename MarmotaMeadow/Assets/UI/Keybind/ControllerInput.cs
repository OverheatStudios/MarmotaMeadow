using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerInput : MonoBehaviour
{
    private bool m_isControllerConnected;

    private void Awake()
    {
        m_isControllerConnected = Gamepad.current != null;
    }

    private void Update()
    {
        m_isControllerConnected = Gamepad.current != null;
    }

    public bool IsControllerConnected()
    {
        return m_isControllerConnected;
    }
}
