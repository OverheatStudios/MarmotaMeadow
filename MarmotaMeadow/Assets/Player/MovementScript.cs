using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

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
    [SerializeField] private CapsuleCollider m_playerCollider;

    [Tooltip("How long does the crouch animation last in seconds?")]
    [SerializeField] private float m_crouchAnimationDuration = 0.75f;
    [Tooltip("How far should the players camera be moved down when crouching?")]
    [SerializeField] private float m_crouchYOffset;
    [Tooltip("When moving and crouching, players speed ")]
    [SerializeField] private float m_crouchingSpeedMultiplier = 0.3f;
    [Tooltip("Height of players collider when crouching is crouch y offset * reduction multiplier")]
    [SerializeField] private float m_extraColliderReductionMultiplier = 1.25f;
    [SerializeField] private bool m_isCrouchingEnabled = true;

    private float m_cameraStartingY;
    private bool m_isCrouching = false;
    private float m_secondsSinceCrouchStateChange;
    private float m_colliderCenterStartingY;
    private float m_colliderHeightStarting;

    void Start()
    {
        PlayerInstance = gameObject;
        m_colliderCenterStartingY = m_playerCollider.center.y;
        m_colliderHeightStarting = m_playerCollider.height;
        m_cameraStartingY = m_camera.transform.localPosition.y;
        m_secondsSinceCrouchStateChange = m_crouchAnimationDuration;
    }

    private void Update()
    {
        if (!m_isCrouchingEnabled) return;

        m_secondsSinceCrouchStateChange += Time.deltaTime;

        // Crouch state changes
        if (m_isCrouching && !Input.GetKey(KeyCode.DownArrow))
        {
            m_isCrouching = false;
            m_secondsSinceCrouchStateChange =   m_crouchAnimationDuration - m_secondsSinceCrouchStateChange;
        }
        else if (!m_isCrouching && Input.GetKey(KeyCode.DownArrow))
        {
            m_isCrouching = true;
            m_secondsSinceCrouchStateChange = m_crouchAnimationDuration - m_secondsSinceCrouchStateChange;
        }
        if (m_secondsSinceCrouchStateChange < 0) m_secondsSinceCrouchStateChange = 0;

        // Animation
        float t = Mathf.InverseLerp(0, m_crouchAnimationDuration, m_secondsSinceCrouchStateChange);
        t = Easing.OutCubic(t);
        var camPos = m_camera.transform.localPosition;
        if (!m_isCrouching) t = 1 - t;
        float positionChange = t * m_crouchYOffset;
        camPos.y = m_cameraStartingY - positionChange;
        m_camera.transform.localPosition = camPos;

        // Lower collider
        positionChange *= m_extraColliderReductionMultiplier;
        var colliderCenter = m_playerCollider.center;
        colliderCenter.y = m_colliderCenterStartingY - positionChange * 0.5f;
        m_playerCollider.center = colliderCenter;
        m_playerCollider.height = m_colliderHeightStarting - positionChange;
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
        movement = movement.normalized * (Time.deltaTime * m_speed * (IsCrouching() ? m_crouchingSpeedMultiplier : 1));

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
    public bool IsCrouching()
    {
        return m_isCrouching;
    }
}
