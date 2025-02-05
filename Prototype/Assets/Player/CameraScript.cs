using UnityEngine;
using UnityEngine.InputSystem;

public class CameraScript : MonoBehaviour
{
    /// <summary>
    /// How much does the camera move when player moves mouse
    /// </summary>
    [SerializeField] private float m_sensitivity = 1;


    private float pitch = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        // Move camera based on how much mouse moved this frame
        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * m_sensitivity;

        pitch -= mouseDelta.y;
        pitch = Mathf.Clamp(pitch, -89f, 89f);

        transform.localRotation = Quaternion.Euler(pitch, transform.localRotation.eulerAngles.y + mouseDelta.x, 0);
    }
}