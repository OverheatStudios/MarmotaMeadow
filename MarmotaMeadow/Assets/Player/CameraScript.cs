using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class CameraScript : MonoBehaviour
{
    [Tooltip("How much does the camera move when player moves mouse, multiplied by camera sensitivity setting")]
    [SerializeField] private float m_sensitivity = 1;

    [SerializeField] private CursorHandlerScript m_cursorHandler;
    [SerializeField] private SettingsScriptableObject m_settings;
    [SerializeField] private ScrObjGlobalData m_data;

    [Header("Aim Sway")]
    [Tooltip("How far should the aim sway? 1 is a good number")]
    [SerializeField] private float m_swayMinMagnitude = 0;
    [Tooltip("How far should the aim sway? 3 is a good number")]
    [SerializeField] private float m_swayMaxMagnitude = 0;

    [Tooltip("How fast should the aim sway? Takes magnitude/speed seconds to move magnitude units. 0.1 is a good number")]
    [SerializeField] private float m_swayMinSpeed = 0;
    [Tooltip("How fast should the aim sway? Takes magnitude/speed seconds to move magnitude units. 0.2 is a good number")]
    [SerializeField] private float m_swayMaxSpeed = 0;

    [Tooltip("Pauses between sways, 0 is a good number")]
    [SerializeField] private float m_swayMinPause = 0;
    [Tooltip("Pauses between sways, 1.25 is a good number")]
    [SerializeField] private float m_swayMaxPause = 1.25f;

    [Tooltip("Multiply aim sway (speed and magnitude, also divides pause time) by this value depending on current night")]
    [SerializeField] private float[] m_nightAimSwayMultipliers;

    [Tooltip("Is night aim sway enabled? (disable in the day scene if you want)")]
    [SerializeField] private bool m_isNightSwayMultipliersEnabled = false;

    [Header("Recoil")]
    [Tooltip("Aim is adjusted by m_recoilVeloMultiplier * current velocity per frame, having this at 1 makes physical sense but it feels weird")]
    [SerializeField] private float m_recoilVeloMultiplier = 0.1f;
    [Tooltip("Recoil velocity is multiplied by this value every fixed frame")]
    [SerializeField] private float m_recoilDeveloMultiplier = 0.9f;

    private float m_pitch = 0f;
    private Vector2 m_currentSwayVector = Vector2.zero;
    private Vector2 m_lastSwayVector = Vector2.zero;
    private float m_swayProgress = 1;
    private float m_currentSwaySpeed = 0;
    private Vector2 m_swaySinceLateUpdate = Vector2.zero;
    private float m_pauseTimeRemaining = 0;
    private float m_recoilVelocity = 0;

    void Start()
    {
        m_cursorHandler.NotifyUiClosed();

        Assert.IsTrue(m_swayMinMagnitude <= m_swayMaxMagnitude);
        Assert.IsTrue(m_swayMinMagnitude >= 0);

        Assert.IsTrue(m_swayMinSpeed <= m_swayMaxSpeed);
        Assert.IsTrue(m_swayMinSpeed >= 0);

        Assert.IsTrue(m_swayMinPause <= m_swayMaxPause);
        Assert.IsTrue(m_swayMinPause >= 0);

        Assert.IsTrue(m_nightAimSwayMultipliers.Length > 0);
    }

    private void Update()
    {
        if (m_cursorHandler.IsUiOpen()) return;

        if (m_swayProgress >= 1)
        {
            // Finished this sway, pause and pick a new direction
            m_pauseTimeRemaining -= Time.deltaTime * GetNightSwayMultiplier();

            if (m_pauseTimeRemaining <= 0)
            {
                m_pauseTimeRemaining = Random.Range(m_swayMinPause, m_swayMaxPause);
                PickNewSwayDirection();
            }
        }
        else
        {
            // Swaying
            float progressThisFrame = Time.deltaTime * m_currentSwaySpeed;
            m_swaySinceLateUpdate += m_currentSwayVector * progressThisFrame;
            m_swayProgress += progressThisFrame;
        }
    }

    private float GetNightSwayMultiplier()
    {
        if (!m_isNightSwayMultipliersEnabled) return 1;
        int index = Mathf.Min(m_data.GetData().GetNightCounter(), m_nightAimSwayMultipliers.Length - 1);
        return m_nightAimSwayMultipliers[index];
    }

    /// <summary>
    /// Randomly picks a new direction and speed for the aim to sway in.
    /// This function does some fancy stuff to make sure the camera stays roughly in the same position. (we'll add the negative current sway direction with the new sway direction)
    /// </summary>
    private void PickNewSwayDirection()
    {
        Vector2 last = m_lastSwayVector;
        m_lastSwayVector = m_currentSwayVector + m_lastSwayVector;

        // Go back to original position
        m_currentSwayVector += last; // Add last last sway vector, this prevents the vector magnitude from increasing every time we pick a new direction
        m_currentSwayVector *= -m_swayProgress; // Invert the sway direction, this uses progress instead of -1 because there is a chance we overshot 1 depending on delta time
        m_swayProgress = 0;

        // New direction
        Vector2 newDirection = new Vector2(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)).normalized;
        m_currentSwayVector += newDirection * Random.Range(m_swayMinMagnitude, m_swayMaxMagnitude) * GetNightSwayMultiplier();
        m_currentSwaySpeed = Random.Range(m_swayMinSpeed, m_swayMaxSpeed) * GetNightSwayMultiplier();
    }

    void LateUpdate()
    {
        if (m_cursorHandler.IsUiOpen()) return;

        // Move camera based on how much mouse moved this frame
        Vector2 mouseDelta = m_sensitivity * m_settings.GetSettings().GetCameraSensitivity() * m_cursorHandler.GetVirtualMouse().GetMouseDelta();

        mouseDelta += m_swaySinceLateUpdate;
        m_swaySinceLateUpdate = Vector2.zero;

        m_pitch -= mouseDelta.y;
        m_pitch = Mathf.Clamp(m_pitch, -89f, 89f);

        transform.localRotation = Quaternion.Euler(m_pitch, transform.localRotation.eulerAngles.y + mouseDelta.x, 0);
    }

    private void FixedUpdate()
    {
        // Recoil
        m_swaySinceLateUpdate.y += m_recoilVelocity * m_recoilVeloMultiplier;
        m_recoilVelocity *= m_recoilDeveloMultiplier;
        if (m_recoilVelocity < 0.01f)
        {
            m_recoilVelocity = 0;
        }
    }

    public void ApplyRecoil(float force)
    {
        m_recoilVelocity += force;
    }
}