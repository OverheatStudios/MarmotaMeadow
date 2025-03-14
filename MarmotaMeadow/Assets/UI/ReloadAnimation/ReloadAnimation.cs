using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class ReloadAnimation : MonoBehaviour
{
    [SerializeField] private List<Sprite> m_sprites = new List<Sprite>();
    [SerializeField] private Image m_display;
    private float m_secondsPerFrame = 0;
    private float m_secondsSinceAnimationStart = 0;
    private bool m_running = false;
    private bool m_visible = true;

    private void Start()
    {
        Assert.IsTrue(m_sprites.Count > 0);
        m_display.color = Color.clear;
    }

    private void Update()
    {
        if (!m_running)
        {
            m_display.color = Color.clear;
            return;
        }

        m_secondsSinceAnimationStart += Time.deltaTime;

        int index = (int)(m_secondsSinceAnimationStart / m_secondsPerFrame);
        if (index >= m_sprites.Count)
        {
            m_running = false;
            return;
        }

        m_display.sprite = m_sprites[index];
        if (m_visible) m_display.color = Color.white;
        else m_display.color = Color.clear;
    }

    public void StartProgress(float seconds)
    {
        m_secondsSinceAnimationStart = 0;
        m_secondsPerFrame = seconds / (float)m_sprites.Count;
        m_running = true;
    }

    public bool IsRunning()
    {
        return m_running;
    }

    public void SetVisible(bool visible)
    {
        m_visible = visible;
        if (!visible)
        {
            m_display.color = Color.clear;
        }
    }
}
