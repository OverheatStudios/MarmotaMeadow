using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private RectTransform m_progressMask;
    [SerializeField] private Image m_background;
    [SerializeField] private Image m_progress;

    private float m_originalWidth;

    void Start()
    {
        m_originalWidth = m_progressMask.sizeDelta.x;
    }

    public void SetProgress(float value, float min, float max)
    {
        float progress = Mathf.InverseLerp(min, max, value);
        SetProgress01(progress);
    }

    public void SetProgress01(float value)
    {
        m_progressMask.sizeDelta = new Vector2(m_originalWidth * Mathf.Clamp01(value), m_progressMask.sizeDelta.y);
    }

    public void SetVisible(bool visible)
    {
        m_background.color = visible ? Color.white : Color.clear;
        m_progress.color = visible ? Color.white : Color.clear;
    }
}
