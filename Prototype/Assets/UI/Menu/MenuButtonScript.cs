using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonScript : MonoBehaviour
{

    public void OpenSettings()
    {
        SceneManager.LoadScene("SettingsScene", LoadSceneMode.Single);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Day Scene", LoadSceneMode.Single);
    }

    public void OpenMainMenu()
    {
        SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenAudioSettings()
    {
        SceneManager.LoadScene("AudioSettingsScene", LoadSceneMode.Single);
    }

    public void OpenVideoSettings()
    {
        SceneManager.LoadScene("VideoSettingsScene", LoadSceneMode.Single);
    }

    public void OpenControlsSettings()
    {
        SceneManager.LoadScene("ControlsSettingsScene", LoadSceneMode.Single);
    }
}
