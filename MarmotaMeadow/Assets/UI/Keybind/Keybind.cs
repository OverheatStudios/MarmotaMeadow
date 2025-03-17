using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public class Keybind : MonoBehaviour
{
    private static Keybind m_selectedKeybind;
    private static Dictionary<string, KeyCode> m_defaultKeys = new Dictionary<string, KeyCode>
    {
        { "WalkForward", KeyCode.W },
        { "WalkBackwards", KeyCode.S },
        { "StrafeLeft", KeyCode.A },
        { "StrafeRight", KeyCode.D },
        { "Interact", KeyCode.Mouse0 }
    };
    private static Dictionary<string, KeyCode> m_keycodes = new Dictionary<string, KeyCode>();
    private static float m_framesSinceKeybindChange = 0;
    public static bool DidKeybindsChange()
    {
        return m_framesSinceKeybindChange <= 2;
    }

    public static KeyCode GetKeyCode(string actionName)
    {
        if (m_keycodes.ContainsKey(actionName)) return m_keycodes[actionName];
        else
        {
            KeyCode key;
            string storedKey = PlayerPrefs.GetString("keybind." + actionName);
            if (!System.Enum.TryParse<KeyCode>(storedKey, out key))
            {
                if (m_defaultKeys.ContainsKey(actionName)) return m_defaultKeys[actionName];
                Debug.LogError("Missing key code for action: " + actionName);
                return KeyCode.None;
            }
            m_keycodes[actionName] = key;
            return key;
        }
    }

    [SerializeField] private GameObject m_selectedOverlay;
    [SerializeField] private RectTransform m_background;
    [SerializeField] private TextMeshProUGUI m_text;
    [SerializeField] private Vector2 m_padding = new Vector2(20, -25);
    private KeyCode m_key;
    private float m_minWidth;
    private float m_backgroundX;

    private void Awake()
    {
        Assert.IsNotNull(m_selectedOverlay.GetComponent<RectTransform>());

        string storedKey = PlayerPrefs.GetString("keybind." + gameObject.name);
        if (!System.Enum.TryParse<KeyCode>(storedKey, out m_key))
        {
            m_key = m_defaultKeys[gameObject.name];
        }
        m_keycodes[gameObject.name] = m_key;

        m_minWidth = m_background.rect.width;
        m_backgroundX = m_background.transform.position.x;
        Deselect();
    }

    private void Update()
    {
        m_framesSinceKeybindChange++;

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
        if (!IsSelected()) return;

        Event e = Event.current;
        if (e.type != EventType.KeyDown && e.type != EventType.MouseDown) return;
        if (e.keyCode == KeyCode.None) return;

        if (e.keyCode == KeyCode.Mouse0)
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(m_background, Input.mousePosition)) return; // Not hovering
        }

        m_key = e.keyCode;
        PlayerPrefs.SetString("keybind." + gameObject.name, m_key.ToString());
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
