using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[System.Serializable]
public class CameraShakeSettings
{
    [SerializeField]
    [Tooltip("Duration in seconds")]
    public float Duration = 0f;

    [SerializeField]
    [Tooltip("How much does the camera shake? Can be any value, but 1 is recommended")]
    public float Intensity = 1f;

    [SerializeField]
    [Tooltip("How fast does the camera shake? Can be any value, but 1 is recommended")]
    public float Frequency = 1f;

    [SerializeField]
    [Tooltip("Duration in seconds of the start transition")]
    public float TransitionInDuration = 0.1f;

    [SerializeField]
    [Tooltip("Duration in seconds of the stop transition")]
    public float TransitionOutDuration = 0.3f;
}

public class CameraShake : MonoBehaviour
{
    [SerializeField] private Transform m_cameraParent;
    [SerializeField] private Camera m_camera;

    private CameraShakeSettings m_shakeSettings = new CameraShakeSettings();
    private float m_timeSinceShakeStart = 0;

    void Update()
    {
        if (!IsShaking()) return;
        m_timeSinceShakeStart += Time.deltaTime;

        // Transition for starting and stopping shake
        float transition = 1.0f;
        if (m_shakeSettings.TransitionOutDuration >= m_shakeSettings.Duration - m_timeSinceShakeStart)
        {
            transition = Mathf.InverseLerp(0, m_shakeSettings.TransitionOutDuration, m_shakeSettings.Duration - m_timeSinceShakeStart);
        }
        else if (m_shakeSettings.TransitionInDuration >= m_timeSinceShakeStart)
        {
            transition = Mathf.InverseLerp(0, m_shakeSettings.TransitionInDuration, m_timeSinceShakeStart);
        }

        transform.localPosition =
            m_shakeSettings.Intensity / 20 * transition * Mathf.Sin(m_timeSinceShakeStart * m_shakeSettings.Frequency * 50) * m_camera.transform.up +
            m_shakeSettings.Intensity / 20 * transition * Mathf.Sin(m_timeSinceShakeStart * m_shakeSettings.Frequency * 50 * 0.96f + 1.5f) * m_camera.transform.right;
    }

    /// <summary>
    /// Camera shake
    /// <br><param name="settings">Camera shake settings</param></br>
    /// <br><param name="overwrite">Should we overwrite any existing camera shake</param></br>
    /// </summary>
    public void ShakeFor(CameraShakeSettings settings, bool overwrite = true)
    {
        if (!overwrite && IsShaking()) return;
        if (settings.Duration <= 0)
        {
            Debug.LogWarning("Camera shake duration was " + settings.Duration + " seconds");
        }

        m_shakeSettings = settings;
        m_timeSinceShakeStart = 0;
    }

    public bool IsShaking()
    {
        return m_timeSinceShakeStart < m_shakeSettings.Duration;
    }
}