using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bed : MonoBehaviour
{
    [SerializeField] private GameObject m_UI;
    [SerializeField] private GameObject m_confirmationUI;
    [SerializeField] private CursorHandlerScript m_cursorHandler;
    private void OnMouseOver()
    {
        m_UI.SetActive(true);
    }

    private void OnMouseExit()
    {
        m_UI.SetActive(false);
    }

    private void OnMouseDown()
    {
        m_confirmationUI.SetActive(true);
        m_cursorHandler.NotifyUiOpen();
    }

    public void Confirm()
    {
        SceneManager.LoadScene("Shop");
    }

    public void NoButton()
    {
        m_confirmationUI.SetActive(false);
        m_cursorHandler.NotifyUiClosed();
    }
}
