using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class AutoDestroyScript : MonoBehaviour
{
    [SerializeField] private float m_delaySeconds = 60.0f;
    [SerializeField] private float m_fadeOutSeconds = 5.0f;
    /// <summary>
    /// Required for fading, fade out seconds must be 0 if this is null
    /// </summary>
    [SerializeField] private SpriteRenderer m_sprite;
    private float m_timeSpentFading = 0;

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsTrue((m_sprite != null) || (m_sprite == null && m_fadeOutSeconds == 0.0f)); // Cannot fade without sprite
    }

    // Update is called once per frame
    void Update()
    {
        m_delaySeconds -= Time.deltaTime;
        if (m_delaySeconds < 0)
        {
            m_timeSpentFading += Time.deltaTime;
            float opacity = 1.0f - Mathf.InverseLerp(0, m_fadeOutSeconds, m_timeSpentFading);
            if (opacity <= 0)
            {
                Destroy(gameObject);
            }
            else if (m_sprite != null)
            {
                m_sprite.color = new Color(m_sprite.color.r, m_sprite.color.g, m_sprite.color.b, opacity);
            }
        }
    }
}
