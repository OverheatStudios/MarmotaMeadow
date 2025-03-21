using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class IsControllerText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_text;
    [SerializeField] private string m_usingController = "Controller support enabled, disconnect your controller to use mouse and keyboard.";
    [SerializeField] private string m_notUsingController = "Controller support disabled, connect your controller to enable it.";

    void Update()
    {
        m_text.text = Gamepad.current == null ? m_notUsingController : m_usingController;
    }
}
