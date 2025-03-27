using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtonScript : MonoBehaviour
{
    [SerializeField] private GameObject m_mainMenu;
    [SerializeField] private GameObject m_mainSettings;
    [SerializeField] private GameObject m_audioSettings;
    [SerializeField] private GameObject m_videoSettings;
    [SerializeField] private GameObject m_controlsSettings;
    [SerializeField] private GameObject m_credits;
    [SerializeField] private SettingsScriptableObject m_settings;
    [SerializeField] private GameObject m_saves;
    [SerializeField] private GameObject m_newSave;

    [Header("Audio Settings")]
    [SerializeField] private UiSlider m_musicVolumeSlider;
    [SerializeField] private UiSlider m_gameVolumeSlider;

    [Header("Video Settings")]
    [SerializeField] private Toggle m_dynamicCrosshairToggle;

    [Header("Controls Settings")]
    [SerializeField] private UiSlider m_cameraSensitivitySlider;

    private void Start()
    {
        m_musicVolumeSlider.GetOnValueChanged().AddListener(val => m_settings.GetSettings().SetMusicVolume(val));
        m_gameVolumeSlider.GetOnValueChanged().AddListener(val => m_settings.GetSettings().SetGameVolume(val));

        m_dynamicCrosshairToggle.onValueChanged.AddListener(val => m_settings.GetSettings().SetDynamicCrosshair(val));

        m_cameraSensitivitySlider.GetOnValueChanged().AddListener(val => m_settings.GetSettings().SetCameraSensitivity(val));
    }

    private void Update()
    {
        if (!m_settings.IsUiDirty()) return;

        m_musicVolumeSlider.SetValue(m_settings.GetSettings().GetMusicVolume());
        m_gameVolumeSlider.SetValue(m_settings.GetSettings().GetGameVolume());

        m_dynamicCrosshairToggle.isOn = m_settings.GetSettings().IsDynamicCrosshair();

        m_cameraSensitivitySlider.SetValue(m_settings.GetSettings().GetCameraSensitivity());
    }

    public static void PlayGame()
    {
        SceneManager.LoadScene("Day Scene", LoadSceneMode.Single);
    }

    private void DisableAllObjects()
    {
        if(m_mainMenu)
            m_mainMenu.SetActive(false);
        if(m_mainSettings)
            m_mainSettings.SetActive(false);
        if(m_audioSettings)
            m_audioSettings.SetActive(false);
        if(m_videoSettings)
            m_videoSettings.SetActive(false);
        if(m_controlsSettings)
            m_controlsSettings.SetActive(false);
        if(m_credits)
            m_credits.SetActive(false);
        if(m_saves)
            m_saves.SetActive(false);
        if(m_newSave)
            m_newSave.SetActive(false);
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

    public void OpenSaves()
    {
        DisableAllObjects();
        m_saves.SetActive(true);
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

    public void OpenNewSave()
    {
        DisableAllObjects();
        m_newSave.SetActive(true);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
    }

    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }
}
