using UnityEngine;
using UnityEngine.InputSystem;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private float m_sensitivity;

    private float pitch = 0f;

    void Start()
    {
    }

    void LateUpdate()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * m_sensitivity;

        pitch -= mouseDelta.y;
        pitch = Mathf.Clamp(pitch, -89f, 89f);

        transform.localRotation = Quaternion.Euler(pitch, transform.localRotation.eulerAngles.y + mouseDelta.x, 0);

        //Mouse.current.WarpCursorPosition(new Vector2(Screen.width, Screen.height) / 2);
    }
}