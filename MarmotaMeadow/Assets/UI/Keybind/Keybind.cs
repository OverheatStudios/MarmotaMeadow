using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class Keybind : MonoBehaviour
{
    private static Keybind m_selectedKeybind;
    private static Dictionary<string, GameControl> m_defaultKeys = new Dictionary<string, GameControl>
    {
        { "WalkForward", new(KeyCode.W) },
        { "WalkBackwards", new(KeyCode.S) },
        { "StrafeLeft", new(KeyCode.A )},
        { "StrafeRight",new( KeyCode.D) },
        { "Interact", new(KeyCode.Mouse0) }
    };
    private static Dictionary<string, GameControl> m_keycodes = new Dictionary<string, GameControl>();
    private static float m_framesSinceKeybindChange = 0;
    public static bool DidKeybindsChange()
    {
        return m_framesSinceKeybindChange <= 2;
    }

    public static GameControl GetKeyCode(string actionName)
    {
        if (m_keycodes.ContainsKey(actionName)) return m_keycodes[actionName];
        else
        {
            GameControl key = null;
            try
            {
                key = JsonUtility.FromJson<GameControl>(PlayerPrefs.GetString("keybind." + actionName));
            }
            catch (Exception) { };
            if (key == null)
            {
                if (m_defaultKeys.ContainsKey(actionName)) return m_defaultKeys[actionName];
                Debug.LogError("Missing key code for action: " + actionName);
                return new(KeyCode.None);
            }
            m_keycodes[actionName] = key;
            return key;
        }
    }

    [SerializeField] private GameObject m_selectedOverlay;
    [SerializeField] private RectTransform m_background;
    [SerializeField] private TextMeshProUGUI m_text;
    [SerializeField] private Vector2 m_padding = new Vector2(20, -25);
    private GameControl m_key;
    private float m_minWidth;
    private float m_backgroundX;

    private void Awake()
    {
        Assert.IsNotNull(m_selectedOverlay.GetComponent<RectTransform>());

        m_key = null;
        try
        {
            m_key = JsonUtility.FromJson<GameControl>(PlayerPrefs.GetString("keybind." + gameObject.name));
        }
        catch (Exception) { };
        if (m_key == null)
        {
            if (m_defaultKeys.ContainsKey(gameObject.name)) m_key = m_defaultKeys[gameObject.name];
            else
            {
                Debug.LogError("Missing key code for action: " + gameObject.name);
                m_key = new(KeyCode.None);
            }
        }
        m_keycodes[gameObject.name] = m_key;

        m_minWidth = m_background.rect.width;
        m_backgroundX = m_background.transform.position.x;
        Deselect();
    }

    private void Start()
    {
        Deselect();
    }

    private void Update()
    {
        m_framesSinceKeybindChange++;

        if (IsSelected())
        {
            var pressedControllerButtons = GameControl.GetJustPressed();
            foreach (var button in pressedControllerButtons)
            {
                HandlePress(button);
            }
        }

        bool hovering = RectTransformUtility.RectangleContainsScreenPoint(m_background, Input.mousePosition);
        bool clicked = Input.GetMouseButtonDown(0);
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

    private void OnGUI()
    {
        Event e = Event.current;
        if (e.keyCode == KeyCode.None) return;
        if (e.type != EventType.KeyDown && e.type != EventType.MouseDown) return;

        if (!IsSelected()) return;

        if (e.keyCode == KeyCode.Mouse0)
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(m_background, Input.mousePosition)) return; // Not hovering, so we deselect
        }

        HandlePress(new(e.keyCode));
    }

    private void HandlePress(GameControl control)
    {
        m_key = control;
        PlayerPrefs.SetString("keybind." + gameObject.name, JsonUtility.ToJson(m_key));
        m_keycodes[gameObject.name] = m_key;
        m_framesSinceKeybindChange = 0;
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
