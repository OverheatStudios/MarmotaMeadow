using UnityEngine;

public class MovementScript : MonoBehaviour
{
    public static GameObject PlayerInstance;

    /// <summary>
    /// Player camera
    /// </summary>
    [SerializeField] private GameObject m_camera;

    /// <summary>
    /// Player movement speed
    /// </summary>
    [SerializeField] private float m_speed = 8.0f;

    /// <summary>
    /// Rigid body of the player
    /// </summary>
    [SerializeField] private Rigidbody m_rigidbody;

    void Start()
    {
        PlayerInstance = gameObject;
    }

    void FixedUpdate()
    {
        // Which direction is player trying to move
        Vector3 movement = Vector2.zero;
        if (Input.GetKey(Keybind.GetKeyCode("WalkForward")))
            movement.x += 1;
        if (Input.GetKey(Keybind.GetKeyCode("StrafeLeft")))
            movement.z -= 1;
        if (Input.GetKey(Keybind.GetKeyCode("WalkBackwards")))
            movement.x -= 1;
        if (Input.GetKey(Keybind.GetKeyCode("StrafeRight")))
            movement.z += 1;

        // Scale movement vector
        movement = movement.normalized * (Time.deltaTime * m_speed);

        // Where is the camera facing
        Vector3 forward = m_camera.transform.forward;
        forward.y = 0;
        forward = forward.normalized;

        Vector3 right = m_camera.transform.right;
        right.y = 0;
        right = right.normalized;

        // Move
        m_rigidbody.velocity = forward * movement.x + right * movement.z;
    }
}
