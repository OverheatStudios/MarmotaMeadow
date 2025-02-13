using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bed : MonoBehaviour
{
    [SerializeField] private GameObject m_UI;
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
        SceneManager.LoadScene("NightScene");
    }
}
