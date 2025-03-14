using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UiSlider : MonoBehaviour
{
    [SerializeField] private Image m_bar;
    [SerializeField] private Image m_handle;
    [SerializeField] private RectTransform m_handleRect;
    [SerializeField] private RectTransform m_collider;
    [SerializeField] private UnityEvent<float> m_onValueChanged = new UnityEvent<float>();
    [Range(0f, 1f)]
    [SerializeField] private float m_value = 0;
    [Tooltip("Sprite to use for handle")]
    [SerializeField] private Sprite m_handleSprite;
    [Tooltip("Sprite to use for handle when value is 0, optional")]
    [SerializeField] private Sprite m_handleSpriteMinValue;
    [SerializeField] private float m_handleRightOffset = 0;
    private bool m_startedClickOnBar = false;

    private void Start()
    {
        if (m_handleSpriteMinValue == null)
        {
            m_handleSpriteMinValue = m_handleSprite;
        }

        SetValue(m_value);
    }

    private void Update()
    {
        bool overBar = RectTransformUtility.RectangleContainsScreenPoint(m_collider, Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            m_startedClickOnBar = overBar;
        }

        if (Input.GetMouseButton(0)  && m_startedClickOnBar)
        {
            SetValue(GetValueFromPosition(Input.mousePosition.x));
        }
    }

    private float GetValueFromPosition(float x)
    {
        return Mathf.InverseLerp(
            m_bar.rectTransform.position.x - 0.5f * (m_bar.rectTransform.sizeDelta.x + 2 * m_handleRect.sizeDelta.x),
            m_bar.rectTransform.position.x + 0.5f * (m_bar.rectTransform.sizeDelta.x + 2 * m_handleRect.sizeDelta.x),
            x
            );
    }

    private void UpdateHandleSprite(float newValue)
    {
        m_handle.sprite = newValue == 0 ? m_handleSpriteMinValue : m_handleSprite;
    }

    public void SetValue(float value)
    {
        UpdateHandleSprite(value);
        m_value = value;
        m_onValueChanged.Invoke(m_value);

        Vector3 pos = m_handle.transform.position;
        pos.x = m_bar.rectTransform.position.x + (m_value - 0.5f) * (m_bar.rectTransform.sizeDelta.x + 2 * m_handleRect.sizeDelta.x);
        pos.x += Mathf.Lerp(0, m_handleRightOffset, m_value);
        m_handle.transform.position = pos;
    }

    public float GetValue() { return m_value; }

    public UnityEvent<float> GetOnValueChanged() { return m_onValueChanged; }
}
