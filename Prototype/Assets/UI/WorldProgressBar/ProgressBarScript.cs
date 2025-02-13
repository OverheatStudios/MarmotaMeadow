using UnityEngine;
using UnityEngine.Assertions;

public class ProgressBarScript : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_fillSprite;
    [SerializeField] private Color m_fullColor = Color.green;
    [SerializeField] private Color m_emptyColor = Color.red;

    private const float UNSPECIFIED = float.MinValue;
    private float m_maxValue = 1.0f;
    private float m_minValue = 0.0f;
    private float m_value = 1.0f;

    /// <summary>
    /// Set the progress bar current progress, clamps to min and max value
    /// </summary>
    /// <param name="value">Current progress</param>
    /// <param name="maxValue">Maximum progress, starts at 1, use unspecified to leave unchanged</param>
    /// <param name="minValue">Minimum progress, starts 0, use unspecified to leave unchanged</param>
    public void SetProgress(float value, float maxValue = UNSPECIFIED, float minValue = UNSPECIFIED)
    {
        // Range
        if (minValue != UNSPECIFIED)
            m_minValue = minValue;
        if (maxValue != UNSPECIFIED)
            m_maxValue = maxValue;
        Assert.IsTrue(m_maxValue >= m_minValue);

        // Value
        m_value = value;
        if (m_value > m_maxValue) m_value = maxValue;
        if (m_value < m_minValue) m_value = minValue;

        // Bar
        float progress = Mathf.InverseLerp(m_minValue, m_maxValue, value);
        m_fillSprite.transform.localScale = new Vector3(progress, m_fillSprite.transform.localScale.y, m_fillSprite.transform.localScale.z);
        m_fillSprite.color = Color.Lerp(m_emptyColor, m_fullColor, progress);
    }
}
