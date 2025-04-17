using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class HurtUI : MonoBehaviour
{
    private static HurtUI m_instance;

    [SerializeField] private Image m_image;
    [Tooltip("How opaque can the hurt UI overlay get")]
    [SerializeField] private float m_maximumOpacity = 0.75f;
    [Tooltip("How long does the hurt animation take")]
    [SerializeField] private float m_duration = 0.5f;

    private float m_secondsSinceAnimationStarted = -1;

    void Start()
    {
        m_instance = this;
    }

    void Update()
    {
        if (m_secondsSinceAnimationStarted >=0)
        {
            m_secondsSinceAnimationStarted += Time.deltaTime;
        }
        
        float t = Mathf.InverseLerp(0, m_duration, m_secondsSinceAnimationStarted);
        t = Easing.OutQuad(t);
        if (t > 0.5f) t = 1.0f - t;
        t *= 2;

        var color = m_image.color;
        color.a = t * m_maximumOpacity;
        m_image.color = color;
    }

    public static void PlayAnimation()
    {
        if (m_instance == null) return;
        if (m_instance.m_secondsSinceAnimationStarted < 0 || m_instance.m_secondsSinceAnimationStarted > m_instance.m_duration)
        {
            m_instance.m_secondsSinceAnimationStarted = 0;
        }
    }
}
