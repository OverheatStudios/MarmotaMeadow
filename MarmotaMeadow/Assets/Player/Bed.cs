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
    [SerializeField] private bool m_clicked = false;
    [SerializeField] private TutorialManager tutorialManager;
    [SerializeField] private bool isInBed;
    private void OnMouseOver()
    {
        if(!m_clicked)
            m_UI.SetActive(true);
        
        if (Input.GetKeyDown(KeyCode.Q) && tutorialManager.ReturnIsTutorialFinished())
        {
            isInBed = true;
            m_confirmationUI.SetActive(true);
            m_cursorHandler.NotifyUiOpen();
            m_clicked = true;
            m_UI.SetActive(false);
        }
    }

    private void OnMouseExit()
    {
        m_UI.SetActive(false);
    }
    

    public void Confirm()
    {
        SceneManager.LoadScene("Shop");
    }

    public void NoButton()
    {
        isInBed = false;
        m_confirmationUI.SetActive(false);
        m_cursorHandler.NotifyUiClosed();
        m_clicked = false;
    }

    public bool ReturnIsInBed()
    {
        return isInBed;
    }
}
