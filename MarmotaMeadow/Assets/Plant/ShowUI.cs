using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowUI : MonoBehaviour
{
    [SerializeField] private GameObject m_UI;
    [SerializeField] private bool m_interactedWithSoil;
    [SerializeField] private bool m_interactedWithUI;


    private void OnMouseOver()
    {
        m_interactedWithSoil = true;
        m_UI.SetActive(true);
    }

    private void OnMouseExit()
    {
        m_interactedWithSoil = false;
        Invoke(nameof(DisableUI), 1f);
    }

    public void InteractedWithUI()
    {
        m_interactedWithUI = true;
    }
    
    public void StopInteractionWithUI()
    {
        m_interactedWithUI = false;
        Invoke(nameof(DisableUI), 1f);
    }

    private void DisableUI()
    {
        if (!m_interactedWithSoil & !m_interactedWithUI)
        {
            m_UI.SetActive(false);
        }
    }
}
