using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    class TooltipData
    {
        public GameObject Tooltip;
        public float Seconds;
        public Dictionary<TextMeshProUGUI, Color> m_texts;
        public Dictionary<Image, Color> m_images;

        public TooltipData(GameObject prefab, float seconds, Transform parent)
        {
            Tooltip = Instantiate(prefab);
            Tooltip.transform.SetParent(parent, false);
            Tooltip.SetActive(false);
            Seconds = seconds;
            m_texts = new();
            m_images = new();

            var texts = Tooltip.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (var text in texts)
            {
                m_texts.Add(text, text.color);
            }

            var images = Tooltip.GetComponentsInChildren<Image>();
            foreach (var image in images)
            {
                m_images.Add(image, image.color);
            }
        }
    }

    [SerializeField] private float m_fadeSeconds = 1.5f;
    [SerializeField] private float m_padding = 20;
    [SerializeField] private float m_moveSpeed = 150.0f;
    [SerializeField] private Vector2 m_offset = new Vector2(10, 10);
    private List<TooltipData> m_tooltips = new();
    private float m_yOffset = 0;
    private static TooltipManager thisInstance = null;

    void Start()
    {
        thisInstance = this;
        Assert.IsTrue(m_fadeSeconds >= 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_tooltips.Count < 1) m_yOffset = 0;
        if (m_yOffset > 0) m_yOffset -= Time.deltaTime * m_moveSpeed;
        if (m_yOffset < 0) m_yOffset = 0;

        for (int i = 0; i < m_tooltips.Count; i++)
        {
            m_tooltips[i].Seconds -= Time.deltaTime;

            // Handle fading out
            if (m_tooltips[i].Seconds < m_fadeSeconds)
            {
                float alpha = m_fadeSeconds == 0 ? 0 : Mathf.InverseLerp(0, m_fadeSeconds, m_tooltips[i].Seconds);
                if (alpha <= 0)
                {
                    var rect = m_tooltips[i].Tooltip.GetComponent<RectTransform>();
                    m_yOffset += rect.sizeDelta.y + m_padding;
                    m_tooltips.RemoveAt(i);
                    --i;
                    continue;
                }

                foreach (var pair in m_tooltips[i].m_images)
                {
                    Color color = pair.Key.color;
                    color.a = alpha * pair.Value.a;
                    pair.Key.color = color;
                }

                foreach (var pair in m_tooltips[i].m_texts)
                {
                    Color color = pair.Key.color;
                    color.a = alpha * pair.Value.a;
                    pair.Key.color = color;
                }

            }
        }

        // Set positions
        float y = m_yOffset;
        foreach (var tooltip in m_tooltips)
        {
            var rect = tooltip.Tooltip.GetComponent<RectTransform>();
            var pos = rect.position;
            pos.x = m_offset.x + rect.sizeDelta.x / 2;
            pos.y = y + m_offset.y + rect.sizeDelta.y / 2;
            rect.position = pos;
            y += m_padding + rect.sizeDelta.y;

            if (!tooltip.Tooltip.activeInHierarchy)
            {
                tooltip.Tooltip.SetActive(true);
            }
        }
    }

    public void ShowTooltip(GameObject tooltip, float time = 5)
    {
        m_tooltips.Add(new(tooltip, time, transform));
    }

    public static TooltipManager Get()
    {
        return thisInstance;
    }
}
