using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class ActionTooltip : MonoBehaviour
{
    [SerializeField] private Sprite[] m_sprites;
    [SerializeField] private float m_duration = 1.0f;
    [SerializeField] private float m_speed = 25.0f;
    [SerializeField] private Image m_image;
    [SerializeField] private Canvas m_canvas;
    [SerializeField] private float m_minScale = 5.0f;
    [SerializeField] private float m_maxScale = 10.0f;
    private float m_seconds = 0;

    void Start()
    {
        Assert.IsTrue(m_sprites.Length > 0);
        m_image.sprite = m_sprites[Random.Range(0, m_sprites.Length)];
        m_image.preserveAspect = true;
        m_image.rectTransform.sizeDelta = m_image.sprite.rect.size;
        float scale = Random.Range(m_minScale, m_maxScale);
        m_canvas.transform.localScale = new Vector3(scale / m_canvas.GetComponent<RectTransform>().sizeDelta.x, scale / m_canvas.GetComponent<RectTransform>().sizeDelta.y, 1.0f);
    }

    void Update()
    {
        m_canvas.transform.LookAt(m_canvas.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);

        m_seconds += Time.deltaTime;

        transform.position += Vector3.up * m_speed * Time.deltaTime;

        var color = m_image.color;
        color.a = 1 - Mathf.InverseLerp(0, m_duration, m_seconds);
        m_image.color = color;

        if (color.a <= 0)
        {
            Destroy(gameObject);
        }
    }
}
