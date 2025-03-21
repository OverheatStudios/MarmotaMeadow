using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class Keybind : MonoBehaviour
{
    private static Keybind m_selectedKeybind;

    [SerializeField] private GameObject m_selectedOverlay;
    [SerializeField] private RectTransform m_background;
    [SerializeField] private TextMeshProUGUI m_text;
    [SerializeField] private Vector2 m_padding = new Vector2(20, -25);
    [SerializeField] private CursorHandlerScript m_cursorHandlerScript;
    private GameControl m_key;
    private float m_minWidth;
    private float m_backgroundX;
    private bool m_controllerConnectedLastFrame;

    private static bool IsControllerConnected()
    {
        return Gamepad.current != null;
    }

    private void Awake()
    {
        Assert.IsNotNull(m_selectedOverlay.GetComponent<RectTransform>());

        LoadFromPrefs();
    }

    /// <summary>
    /// Reset the keybind to initial state and load the key from player prefs
    /// </summary>
    private void LoadFromPrefs()
    {

        m_controllerConnectedLastFrame = IsControllerConnected();
        bool selected = IsSelected();

        m_key = GameInput.GetKeybind(gameObject.name);

        m_minWidth = m_background.rect.width;
        m_backgroundX = m_background.transform.position.x;
        Deselect();

        if (selected) Select();

        GameInput.m_framesSinceKeybindChange = 0;

    }

    private void Start()
    {
        Deselect();
    }

    private void Update()
    {
        GameInput.m_framesSinceKeybindChange++;

        // Controller connected/disconnected
        if (IsControllerConnected() != m_controllerConnectedLastFrame)
        {
            SetText(m_key.ToString());
            m_controllerConnectedLastFrame = IsControllerConnected();
        }

        // Controller input
        if (IsSelected() && IsControllerConnected())
        {
            var pressedControllerButtons = GameControl.GetJustPressed();
            foreach (var button in pressedControllerButtons)
            {
                if (IsDeselectInput(new(KeyCode.None, button))) continue; // "clicking" outside the keybind
                m_key.SetControllerButton(button);
                HandleBoundKeyChange();
            }
        }

        // Select & deselect
        bool hovering = RectTransformUtility.RectangleContainsScreenPoint(m_background, Input.mousePosition);
        bool clicked = m_cursorHandlerScript.GetVirtualMouse().IsLMBDown();
        if (!IsSelected() && hovering && clicked)
        {
            Select();
            return;
        }

        if (IsSelected() && !hovering && clicked)
        {
            Deselect();
        }
    }

    /// <summary>
    /// Keyboard and mouse input
    /// </summary>
    private void OnGUI()
    {
        if (IsControllerConnected()) return;

        Event e = Event.current;
        if (e.keyCode == KeyCode.None) return;
        if (e.type != EventType.KeyDown && e.type != EventType.MouseDown) return;

        if (!IsSelected()) return;
        if (IsDeselectInput(new(e.keyCode, GamepadButton.Start))) return;

        m_key.SetKey(e.keyCode);
        HandleBoundKeyChange();
    }

    /// <summary>
    /// Is the user attempting to deselect
    /// </summary>
    /// <param name="control">The user input</param>
    /// <returns>True if the input is the user attempting to deselect the keybind, not bind the keybind to the input</returns>
    private bool IsDeselectInput(GameControl control)
    {
        if (m_cursorHandlerScript.GetVirtualMouse().IsLMB(control))
        {
            return !RectTransformUtility.RectangleContainsScreenPoint(m_background, Input.mousePosition); // Not hovering, so we deselect
        }
        return false;
    }

    /// <summary>
    /// Update the keybind when the key is changed
    /// </summary>
    private void HandleBoundKeyChange()
    {
        PlayerPrefs.SetString("action_keybind." + gameObject.name, JsonUtility.ToJson(m_key));
        GameInput.m_keycodes[gameObject.name] = m_key;
        GameInput.m_framesSinceKeybindChange = 0;
        SetText(m_key.ToString());
    }

    public bool IsSelected()
    {
        return m_selectedOverlay.activeInHierarchy;
    }

    public void Deselect()
    {
        if (m_selectedKeybind == this) { m_selectedKeybind = null; }

        m_selectedOverlay.SetActive(false);
        SetText(m_key.ToString());
    }

    public void Select()
    {
        if (m_selectedKeybind)
        {
            m_selectedKeybind.Deselect();
        }

        m_selectedKeybind = this;
        m_selectedOverlay.SetActive(true);
        SetText("-", false);
    }

    /// <summary>
    /// Change the text of the keybind, updating size too
    /// </summary>
    /// <param name="text">The new text</param>
    /// <param name="shrinkWidth">If true, we are allowed to shrink the width, if false we'll only keep or grow the width</param>
    private void SetText(string text, bool shrinkWidth = true)
    {
        float width = Mathf.Max(m_minWidth, m_text.GetPreferredValues(text).x + m_padding.x);
        if (!shrinkWidth)
        {
            width = Mathf.Max(width, m_background.sizeDelta.x);
        }
        m_text.text = text;

        var overlayRect = m_selectedOverlay.GetComponent<RectTransform>();

        var pos = m_background.transform.position;
        pos.x = m_backgroundX;// + (width - m_minWidth) / 2;
        m_background.transform.position = pos;
        overlayRect.transform.position = pos;
        m_text.transform.position = new Vector3(pos.x, pos.y + m_padding.y, pos.z);

        m_background.sizeDelta = new Vector2(width, m_background.sizeDelta.y);
        overlayRect.sizeDelta = new Vector2(width, overlayRect.sizeDelta.y);
    }
}
