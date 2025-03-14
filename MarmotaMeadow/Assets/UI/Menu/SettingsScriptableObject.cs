using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Timers;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public class Settings
{
    [SerializeField] private float m_musicVolume = 0.5f; // 0 to 1 range
    [SerializeField] private bool m_dynamicCrosshair = true;
    [SerializeField] private float m_cameraSensitivity = 0.5f; // 0.1 to 1 range

    private bool m_dirty;

    public float GetMusicVolume()
    {
        return m_musicVolume;
    }

    public void SetMusicVolume(float value)
    {
        if (value == m_musicVolume) return;
        m_musicVolume = value;
        m_dirty = true;
        ValidateSettings();
    }

    public bool IsDynamicCrosshair()
    {
        return m_dynamicCrosshair;
    }

    public void SetDynamicCrosshair(bool enabled)
    {
        if (enabled ==  m_dynamicCrosshair) return; 
        m_dynamicCrosshair = enabled;
        m_dirty = true;
        ValidateSettings();
    }

    public float GetCameraSensitivity()
    {
        return m_cameraSensitivity;
    }

    public void SetCameraSensitivity(float sens)
    {
        if (sens == m_cameraSensitivity) return;    
        m_cameraSensitivity = sens;
        m_dirty = true;
        ValidateSettings();
    }

    public void ValidateSettings()
    {
        if (m_musicVolume < 0) SetMusicVolume(0);
        if (m_musicVolume > 1) SetMusicVolume(1);

        // don't need to validate dynamic crosshair

        if (m_cameraSensitivity < 0) SetCameraSensitivity(0);   
        if (m_cameraSensitivity > 1) SetCameraSensitivity(1);
    }

    public void Reset()
    {
        SetMusicVolume(0.5f);
        SetDynamicCrosshair(true);
        SetCameraSensitivity(0.5f);
    }

    public void MarkClean()
    {
        m_dirty = false;
    }

    public bool IsDirty()
    {
        return m_dirty;
    }
}

[CreateAssetMenu(fileName = "SettingsScriptableObject", menuName = "Scriptable Objects/SettingsScriptableObject")]
public class SettingsScriptableObject : ScriptableObject
{
    [Tooltip("How often are the settings saved to disk? Saving is done async and isn't done if theres nothing changed, so this can be low interval if required")]
    [Range(1, 3600)]
    [SerializeField] private float m_settingsAutosaveIntervalSeconds = 60;
    private Timer m_autosaveTimer;

    [SerializeField] private string m_settingsFile = "settings.json";
    private string m_settingsFilePath;

    private Settings m_settings = new Settings();
    private bool m_isUiDirty = true;

    void OnEnable()
    {
        if (!GameRunning.IsGameRunning())
        {
            m_settings = null;
            return;
        }

        Assert.IsTrue(m_settingsFile.Length > 0);
        m_settingsFilePath = Path.Combine(Application.persistentDataPath, m_settingsFile);
        LoadSettings();

        // We save forever to also save debug menu settings
        if (m_autosaveTimer == null)
        {
            m_autosaveTimer = new Timer(m_settingsAutosaveIntervalSeconds * 1000);
            m_autosaveTimer.Elapsed += (sender, e) => _ = SaveSettings();
            m_autosaveTimer.AutoReset = true;
            m_autosaveTimer.Enabled = true;
        }
    }

    void OnDisable()
    {
        if (!GameRunning.IsGameRunning()) return;

        _ = SaveSettings();
    }

    /// <summary>
    /// Save current settings to JSON
    /// </summary>
    private async Task SaveSettings()
    {
        if (m_settings == null || !m_settings.IsDirty()) return;

        try
        {
            string json = JsonUtility.ToJson(m_settings);
            using (StreamWriter writer = new StreamWriter(m_settingsFilePath, false))
            {
                await writer.WriteAsync(json);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to save settings to file: {ex.Message}");
        }

        m_settings.MarkClean();
    }

    /// <summary>
    ///  Load current settings from JSON, resets any settings the file is missing
    /// </summary>
    private async void LoadSettings()
    {
        if (!File.Exists(m_settingsFilePath))
        {
            if (m_settings == null)
            {
                m_settings = new Settings();
            }

            m_settings.Reset();
            m_isUiDirty = true;
            await SaveSettings();
            return;
        }

        try
        {
            string json;
            using (StreamReader reader = new StreamReader(m_settingsFilePath))
            {
                json = await reader.ReadToEndAsync();
            }

            m_settings = JsonUtility.FromJson<Settings>(json);
            Assert.IsNotNull(m_settings);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to load settings from file: {ex.Message}");
        }

        m_settings.MarkClean();
        m_settings.ValidateSettings();
        m_isUiDirty = true;
    }

    public Settings GetSettings()
    {
        return m_settings;
    }

    public bool IsUiDirty()
    {
        return m_isUiDirty;
    }
}
