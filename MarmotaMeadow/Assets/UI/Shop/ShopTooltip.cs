using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class ShopTooltip : MonoBehaviour
{
    private static bool m_isAnyIconHovered = false;
    private static ShopTooltip m_latestScript;
    [SerializeField] private RectTransform m_itemIcon;
    [SerializeField] private GameObject m_tooltip;
    [TextArea]
    [SerializeField] private string m_tooltipText;
    private TextMeshProUGUI m_tooltipTextComponent;

    private void Start()
    {
        m_tooltipTextComponent = m_tooltip.GetComponentInChildren<TextMeshProUGUI>();
        Assert.IsNotNull(m_tooltipTextComponent);

        m_latestScript = this;
    }

    private void Update()
    {
        //print(m_itemIcon.position + ", " + m_itemIcon.sizeDelta);
        //print(Input.mousePosition);

        if (m_isAnyIconHovered) return; // Some other icon already is hovered

        // Are we hovered
        Vector2 mousePos = Input.mousePosition;
        Vector2 min = new Vector2(m_itemIcon.position.x, m_itemIcon.position.y) - m_itemIcon.sizeDelta / 2;
        Vector2 max = min + m_itemIcon.sizeDelta;
        if (mousePos.x < min.x || mousePos.y < min.y || mousePos.x > max.x || mousePos.y > max.y) return; // Not hovering

        m_isAnyIconHovered = true;
        m_tooltip.transform.position = m_itemIcon.position + new Vector3(50, 50, 0);
        m_tooltipTextComponent.text = m_tooltipText;
    }

    private void LateUpdate()
    {
        Assert.IsNotNull(m_latestScript);
        if (this != m_latestScript) return; // Only a single script should do this per frame
        m_tooltip.SetActive(m_isAnyIconHovered);
        m_isAnyIconHovered = false;
    }
}
