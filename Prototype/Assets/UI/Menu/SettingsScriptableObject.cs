using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Timers;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public class Settings
{
    [SerializeField] private float m_musicVolume = 0.5f;
    [SerializeField] private bool m_dynamicCrosshair = true;

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

    public void ValidateSettings()
    {
        if (m_musicVolume < 0) SetMusicVolume(0);
        if (m_musicVolume > 1) SetMusicVolume(1);

        // don't need to validate dynamic crosshair
    }

    public void Reset()
    {
        SetMusicVolume(0.5f);
        SetDynamicCrosshair(true);
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

    private bool IsGameRunning()
    {
#if UNITY_EDITOR
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return true;
        }
#endif
        return Application.isPlaying;
    }

    void OnEnable()
    {
        if (!IsGameRunning())
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
        if (!IsGameRunning()) return;

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
