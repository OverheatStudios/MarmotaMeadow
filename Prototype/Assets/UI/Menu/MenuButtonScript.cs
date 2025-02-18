using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonScript : MonoBehaviour
{
    [SerializeField] private GameObject m_mainMenu;
    [SerializeField] private GameObject m_mainSettings;
    [SerializeField] private GameObject m_audioSettings;
    [SerializeField] private GameObject m_videoSettings;
    [SerializeField] private GameObject m_controlsSettings;
    [SerializeField] private GameObject m_credits;

    public void PlayGame()
    {
        SceneManager.LoadScene("Day Scene", LoadSceneMode.Single);
    }

    private void DisableAllObjects()
    {
        m_mainMenu.SetActive(false);
        m_mainSettings.SetActive(false);
        m_audioSettings.SetActive(false);
        m_videoSettings.SetActive(false);
        m_controlsSettings.SetActive(false);
        m_credits.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenMainSettings()
    {
        DisableAllObjects();
        m_mainSettings.SetActive(true);
    }

    public void OpenMainMenu()
    {
        DisableAllObjects();
        m_mainMenu.SetActive(true);
    }

    public void OpenAudioSettings()
    {
        DisableAllObjects();
        m_audioSettings.SetActive(true);
    }

    public void OpenVideoSettings()
    {
        DisableAllObjects();
        m_videoSettings.SetActive(true);
    }

    public void OpenControlsSettings()
    {
        DisableAllObjects();
        m_controlsSettings.SetActive(true);
    }

    public void OpenCredits()
    {
        DisableAllObjects();
        m_credits.SetActive(true);
    }
}
