using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class VirtualMouse : MonoBehaviour
{
    [Tooltip("How many pixels per second should controller move the mouse")]
    [SerializeField] private float m_controllerMouseSpeed = 500f;
    [Tooltip("Maximum acceleration of the mouse when controller is used, 3 = 3x")]
    [SerializeField] private float m_maxControllerMouseAcceleration = 3.0f;
    [Tooltip("How long to reach max acceleration in seconds")]
    [SerializeField] private float m_maxControllerMouseAccelerationAfter = 2.5f;
    [Tooltip("Button on controller which causes a LMB click")]
    [SerializeField] private GamepadButton m_mouseClickButton = GamepadButton.A;

    [SerializeField] private Texture2D m_cursorTexture;
    [SerializeField] private Texture2D m_cursorTexturePressed;
    [SerializeField] private Vector2 m_cursorHotspot = new(118, 58);

    private Vector2 m_mousePos;
    private Vector2 m_lastMousePos;
    private float m_secondsControllerHasBeenMovingMouse = 0;

    void Start()
    {
        m_mousePos = new Vector2(Screen.width, Screen.height) / 2;
        Mouse.current?.WarpCursorPosition(m_mousePos);
        m_lastMousePos = m_mousePos; 
        UpdateCursorTex();
    }

    private void Update()
    {
        HandleMouseClickUI();

        if (IsLMBDown() || IsLMBUp()) UpdateCursorTex();
    }

    private void UpdateCursorTex()
    {
        Cursor.SetCursor(IsLMB() ? m_cursorTexturePressed : m_cursorTexture, m_cursorHotspot, CursorMode.Auto);
    }

    void LateUpdate()
    {
        StartCoroutine(HandleInput(Time.deltaTime));
    }

    private IEnumerator HandleInput(float deltaTime)
    {
        yield return new WaitForEndOfFrame();

        HandleMouseMovement(deltaTime);
    }

    private void HandleMouseClickUI()
    {
        if (!IsLMBDown()) return;
        if (Input.GetMouseButtonDown(0)) return; // was a normal click so the on click will be triggered by unity

        var raycastResults = RaycastMouse();
        foreach (var result in raycastResults)
        {
            GameObject obj = result.gameObject;

            var buttons = obj.GetComponentsInChildren<Button>();
            foreach (var button in buttons)
            {
                button.onClick.Invoke();
            }
        }
    }

    // https://discussions.unity.com/t/detect-canvas-object-under-mouse-because-only-some-canvases-should-block-mouse/144125/3
    private List<RaycastResult> RaycastMouse()
    {

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            pointerId = -1,
        };

        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        return results;
    }

    private void HandleMouseMovement(float deltaTime)
    {
        m_lastMousePos = m_mousePos;

        if (!IsControllerInput())
        {
            m_secondsControllerHasBeenMovingMouse = 0;
        }

        if (IsControllerInput())
        {
            // acceleration
            var rightStick = Gamepad.current.rightStick.value.normalized;
            // todo reset acceleration if angle changes too much
            if (rightStick == Vector2.zero) m_secondsControllerHasBeenMovingMouse = 0;
            else m_secondsControllerHasBeenMovingMouse += deltaTime;
            float acceleration = Mathf.Lerp(1, m_maxControllerMouseAcceleration, Mathf.InverseLerp(0, m_maxControllerMouseAccelerationAfter, m_secondsControllerHasBeenMovingMouse));

            // move mouse
            m_mousePos += acceleration * deltaTime * m_controllerMouseSpeed * rightStick;

            if (Cursor.lockState == CursorLockMode.None)
            {
                Mouse.current?.WarpCursorPosition(m_mousePos);
            }
        }
        else if (Mouse.current != null)
        {
            m_mousePos = Mouse.current.position.value;
        }
    }

    public bool IsControllerInput()
    {
        return Gamepad.current != null;
    }

    public Vector2 GetMouseDelta()
    {
        return IsControllerInput() ? m_mousePos - m_lastMousePos : (Mouse.current != null ? Mouse.current.delta.value : Vector2.zero);
    }

    public bool IsLMBDown()
    {
        return Input.GetMouseButtonDown(0) || (IsControllerInput() && Gamepad.current[m_mouseClickButton].wasPressedThisFrame);
    }

    public bool IsLMBUp()
    {
        return Input.GetMouseButtonUp(0) || (IsControllerInput() && Gamepad.current[m_mouseClickButton].wasPressedThisFrame);
    }

    public bool IsLMB()
    {
        return Input.GetMouseButton(0) || (IsControllerInput() && Gamepad.current[m_mouseClickButton].wasPressedThisFrame);
    }

    public bool IsLMB(GameControl control)
    {
        return Gamepad.current == null ? control.GetKeyCode() == KeyCode.Mouse0 : control.GetControllerButton() == m_mouseClickButton;
    }

    public void Lock()
    {

        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Unlock()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}
