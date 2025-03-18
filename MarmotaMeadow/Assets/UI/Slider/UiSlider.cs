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

        if (Input.GetMouseButton(0) && m_startedClickOnBar)
        {
            SetValue(GetValueFromPosition(Input.mousePosition.x));
        }
    }

    private float GetValueFromPosition(float x)
    {
        Vector2 localMousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(m_bar.rectTransform, Input.mousePosition, null, out localMousePos);

        float barHalfWidth = m_bar.rectTransform.sizeDelta.x * 0.5f;
        float handleHalfWidth = m_handleRect.sizeDelta.x * 0.5f;

        float minX = -barHalfWidth + handleHalfWidth;
        float maxX = barHalfWidth - handleHalfWidth;

        return Mathf.InverseLerp(minX, maxX, localMousePos.x);
    }

    private void UpdateHandleSprite(float newValue)
    {
        m_handle.sprite = newValue == 0 ? m_handleSpriteMinValue : m_handleSprite;
    }

    public void SetValue(float value)
    {
        UpdateHandleSprite(value);
        m_value = Mathf.Clamp01(value);
        m_onValueChanged.Invoke(m_value);

        float barHalfWidth = m_bar.rectTransform.sizeDelta.x * 0.5f;
        float handleHalfWidth = m_handleRect.sizeDelta.x * 0.5f;

        float minX = m_bar.rectTransform.anchoredPosition.x - barHalfWidth + handleHalfWidth;
        float maxX = m_bar.rectTransform.anchoredPosition.x + barHalfWidth - handleHalfWidth;

        float newX = Mathf.Lerp(minX, maxX, m_value);

        Vector3 pos = m_handle.rectTransform.anchoredPosition;
        pos.x = newX;
        m_handle.rectTransform.anchoredPosition = pos;
    }

    public float GetValue() { return m_value; }

    public UnityEvent<float> GetOnValueChanged() { return m_onValueChanged; }
}
