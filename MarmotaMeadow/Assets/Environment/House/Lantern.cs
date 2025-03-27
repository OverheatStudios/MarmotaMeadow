using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class Lantern : MonoBehaviour
{
    [SerializeField] private float m_minFlickerTime = 0.25f;
    [SerializeField] private float m_maxFlickerTime = 0.5f;
    [SerializeField] private List<Color> m_colors = new List<Color>();
    [SerializeField] private Light m_light;
    private int m_lastIndex = 0;
    private int m_nextIndex = 1;
    private float m_secondsSinceIndexChange = 0;
    private float m_thisFlickerDuration = 0;

    private void Start()
    {
        Assert.IsTrue(m_colors.Count > 1);
        Assert.IsTrue(m_minFlickerTime > 0);
        Assert.IsTrue(m_minFlickerTime <= m_maxFlickerTime);
    }

    private void Update()
    {
        m_secondsSinceIndexChange += Time.deltaTime;
        if (m_secondsSinceIndexChange >= m_thisFlickerDuration)
        {
            m_secondsSinceIndexChange = 0;
            m_thisFlickerDuration = Random.Range(m_minFlickerTime, m_maxFlickerTime);   
            m_lastIndex = m_nextIndex;
            m_nextIndex = Random.Range(0, m_colors.Count-1);
            if (m_lastIndex == m_nextIndex) m_nextIndex = (m_lastIndex+1) % m_colors.Count;
        }

        float t = Mathf.InverseLerp(0, m_thisFlickerDuration, m_secondsSinceIndexChange);
        Color color = Color.Lerp(m_colors[m_lastIndex], m_colors[m_nextIndex], t);
        color.a = 1;
        m_light.color = color;
    }
}
