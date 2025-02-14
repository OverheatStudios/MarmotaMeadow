using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bed : MonoBehaviour
{
    [SerializeField] private GameObject m_UI;
    [SerializeField] private GameObject m_confirmationUI;
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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Confirm()
    {
        SceneManager.LoadScene("NightScene");
    }

    public void NoButton()
    {
        m_confirmationUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
