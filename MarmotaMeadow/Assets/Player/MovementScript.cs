using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;
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
    [SerializeField] private CollisonsHandler m_collisionsHandler;
    [SerializeField] private List<Pair<FloorType, AudioClip>> m_walkingSfx;
    [Tooltip("How long will we spend fading the walking footsteps sfx when the player stops walking")]
    [SerializeField] private float m_walkingSfxFadeOutDuration = 0.2f;
    private AudioSource m_walkingSource;
    private FloorType m_lastCollidedFloorLastFrame = FloorType.None;

    private float m_cameraStartingY;
    private bool m_isCrouching = false;
    private float m_secondsSinceCrouchStateChange;
    private float m_colliderCenterStartingY;
    private float m_colliderHeightStarting;
    private float m_secondsSinceStoppedWalking = 0;
    private bool m_isWalking = false;

    void Start()
    {
        PlayerInstance = gameObject;
        m_colliderCenterStartingY = m_playerCollider.center.y;
        m_colliderHeightStarting = m_playerCollider.height;
        m_cameraStartingY = m_camera.transform.localPosition.y;
        m_secondsSinceCrouchStateChange = m_crouchAnimationDuration;
    }

    private void HandleWalkingSfx()
    {
        // Do we need to play another?
        if (m_lastCollidedFloorLastFrame == m_collisionsHandler.GetLastCollidedFloorType()) return;
        m_lastCollidedFloorLastFrame = m_collisionsHandler.GetLastCollidedFloorType();

        // Stop the last walking sound effect
        if (m_walkingSource != null)
        {
            m_walkingSource.loop = false;
            StartCoroutine(DestroyIn(m_walkingSource.clip.length, m_walkingSource.gameObject));
            m_walkingSource = null;
        }

        // Start new one
        m_walkingSource = new GameObject().AddComponent<AudioSource>();
        m_walkingSource.loop = true;
        m_walkingSource.transform.SetParent(transform, false);
        foreach (var pair in m_walkingSfx)
        {
            if (pair.First != m_lastCollidedFloorLastFrame) continue;
            m_walkingSource.clip = pair.Second;
        }
        m_walkingSource.volume = 0; // will be set in UpdateWalkingSfxVolume
        m_walkingSource.Play();
    }

    private void UpdateWalkingSfxVolume()
    {
        if (!m_isWalking) m_secondsSinceStoppedWalking += Time.deltaTime;
        else m_secondsSinceStoppedWalking = 0;

        if (m_walkingSource == null) return;

        float volume = Mathf.InverseLerp(m_walkingSfxFadeOutDuration, 0, m_secondsSinceStoppedWalking);
        m_walkingSource.volume = volume;
    }

    private IEnumerator DestroyIn(float seconds, GameObject obj)
    {
        yield return new WaitForSeconds(seconds);
        if (obj) Destroy(obj);
        yield return null;
    }

    private void Update()
    {
        HandleWalkingSfx();
        UpdateWalkingSfxVolume();

        if (!m_isCrouchingEnabled) return;

        m_secondsSinceCrouchStateChange += Time.deltaTime;

        // Crouch state changes
        if (m_isCrouching && !Input.GetKey(KeyCode.DownArrow))
        {
            m_isCrouching = false;
            m_secondsSinceCrouchStateChange = m_crouchAnimationDuration - m_secondsSinceCrouchStateChange;
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
        Vector3 movement = Vector3.zero;

        // Get input and convert from 2d to 3d
        Vector2 input = GameInput.GetPlayerMovementInputDirection();
        if (input != Vector2.zero)
        {
            m_isWalking = true;
            movement.x = input.y;
            movement.z = input.x;
            movement = movement.normalized;

            // Scale movement vector
            movement *= (Time.deltaTime * m_speed * (IsCrouching() ? m_crouchingSpeedMultiplier : 1));

            // Where is the camera facing
            Vector3 forward = m_camera.transform.forward;
            forward.y = 0;
            forward = forward.normalized;

            Vector3 right = m_camera.transform.right;
            right.y = 0;
            right = right.normalized;

            // Move
            m_rigidbody.velocity = forward * movement.x + right * movement.z;
        } else
        {
            m_isWalking = false;
            m_rigidbody.velocity = Vector3.zero;
        }
    }
    public bool IsCrouching()
    {
        return m_isCrouching;
    }
}
