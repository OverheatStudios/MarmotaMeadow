using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotAButtonNotifier : MonoBehaviour
{
    [SerializeField] private CursorHandlerScript m_cursorHandler;
    [SerializeField] private GameObject m_toShow;
    [SerializeField] private RectTransform m_clickArea;

    void Update()
    {
        if (!RectTransformUtility.RectangleContainsScreenPoint(m_clickArea, Input.mousePosition))
        {
            m_toShow.SetActive(false);
            return;
        }


        if (m_cursorHandler.GetVirtualMouse().IsLMBDown())
        {
            m_toShow.SetActive(true);
        }
    }
}
