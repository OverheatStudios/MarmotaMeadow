using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[System.Serializable]
public class Multiplier
{
    public Image Image;
    public int MultiplierAmount;
}

public class BuyMultiplier : MonoBehaviour
{
   [SerializeField] private Multiplier[] m_multipliers;
    [SerializeField] private CursorHandlerScript m_cursorHandler;

    private int m_activeMultiplierIndex = -1;

    void Start()
    {
        Assert.IsTrue(m_multipliers.Length > 0);
        foreach (var multiplier in m_multipliers)
        {
            Assert.IsTrue(multiplier.MultiplierAmount > 0);
        }

        SetActiveMultiplier(0);
    }

    void Update()
    {
        if (!m_cursorHandler.GetVirtualMouse().IsLMBDown()) return;

        int i = 0;
        foreach (var multiplier in m_multipliers)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(multiplier.Image.rectTransform, Input.mousePosition))
            {
                SetActiveMultiplier(i);
                return;
            }
            i++;
        }
    }

    public int GetCurrentMultiplier()
    {
        return m_multipliers[m_activeMultiplierIndex].MultiplierAmount;
    }

    private void SetActiveMultiplier(int index)
    {
        if (index == m_activeMultiplierIndex) return;
        Assert.IsTrue(index >= 0);
        Assert.IsTrue(index < m_multipliers.Length);
        m_activeMultiplierIndex = index;

        for (int i = 0; i < m_multipliers.Length; i++)
        {
            if (i == m_activeMultiplierIndex)
            {
                m_multipliers[i].Image.color = Color.red;
            } else
            {
                m_multipliers[i].Image.color = Color.white;
            }
        }
    }
}
