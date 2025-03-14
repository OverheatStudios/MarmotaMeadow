using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

public class CrosshairScript : MonoBehaviour
{
    [SerializeField] private Image m_image;
    [SerializeField] private Camera m_camera;
    /// <summary>
    /// How many pixels away from the center of the screen should we capture? doing the exact center will just capture the crosshair which isn't great
    /// </summary>
    [SerializeField] private Vector2 m_capturePositionOffset = new Vector2(-15, -15);
    /// <summary>
    /// <1 prefers dark crosshair and >1 prefers light crosshair
    /// Maths is (object brightness)^(light strength)
    /// I thought it looked weird without it because it was always either the same shade of gray or pure white (in the night scene)
    /// </summary>
    [SerializeField][Range(0, 2)] private float m_lightStrength = 0.5f;
    /// <summary>
    /// Our eyes don't see RGB light the same! how much does each colour of light contribute to the <b>perceived</b> brightness? 
    /// https://alienryderflex.com/hsp.html
    /// </summary>
    [SerializeField] private Vector3 m_brightnessContributions = new Vector3(.299f, .587f, .114f);
    private float m_maxBrightness;
    private float m_inverseMaxBrightness;
    private Texture2D m_screenTexture;
    [SerializeField] private SettingsScriptableObject m_settings;
    [Tooltip("If dynamic crosshair is disabled, pretend screen is this colour")]
    [SerializeField] private Color m_staticScreenColor = new(0.33f, 0.33f, 0.33f);

    void OnEnable()
    {
        m_maxBrightness = m_brightnessContributions.magnitude + 0.01f;
        m_inverseMaxBrightness = m_maxBrightness == 0 ? 1 : 1.0f / m_maxBrightness;

        m_screenTexture = new Texture2D(1, 1, TextureFormat.RGB24, false);
    }

    void Update()
    {
        if (m_settings.GetSettings().IsDynamicCrosshair())
        {
            StartCoroutine(CalculateOptimalLightness());
        }
        else
        {
            UpdateCrosshairColor(m_staticScreenColor);
        }
    }

    void OnDisable()
    {
        Destroy(m_screenTexture);
    }

    private System.Collections.IEnumerator CalculateOptimalLightness()
    {
        // https://docs.unity3d.com/6000.0/Documentation/ScriptReference/ScreenCapture.CaptureScreenshotAsTexture.html

        yield return new WaitForEndOfFrame();

        Vector2 capturePosition = new Vector2(Screen.width / 2, Screen.height / 2) + m_capturePositionOffset;

        // Copies from current rt, on update that should be the final screen of the last frame unless im dumb
        // Way better than taking a screenshot every frame
        m_screenTexture.ReadPixels(new Rect(capturePosition.x, capturePosition.y, 1, 1), 0, 0);

        Color color = m_screenTexture.GetPixel(0, 0);
        UpdateCrosshairColor(color);
    }

    private void UpdateCrosshairColor(Color screenColor)
    {
        // magnitude of (contributions * colour)
        float brightness = Mathf.Sqrt(Mathf.Pow(m_brightnessContributions.x * screenColor.r, 2)
            + Mathf.Pow(m_brightnessContributions.y * screenColor.g, 2)
            + Mathf.Pow(m_brightnessContributions.z * screenColor.b, 2));

        // Apply light strength offset 
        brightness = Mathf.Min(m_maxBrightness, Mathf.Pow(brightness, m_lightStrength));

        // Get ideal colour from brightness
        Color color = Color.white * (m_maxBrightness - brightness) * m_inverseMaxBrightness;
        color.a = 1;
        m_image.color = color;
    }
}
