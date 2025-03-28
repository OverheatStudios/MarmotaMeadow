using System.IO;
using System.Runtime.CompilerServices;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[System.Serializable]
public class GlobalData
{
    public int GroundhogsSpawned = 0;
    public int GroundhogsKilled = 0;
    [SerializeField] private int NightCounter = -1;
    public int CurrentHealth = ScrObjGlobalData.MAX_HEALTH;

    // Current night counter, this is incremented when you go to the shop
    // first day = 0
    // first shop = 0
    // first night = 0
    // second day = 0
    // second shop = 1
    // second night = 1
    public int GetNightCounter()
    {
        return Mathf.Max(0, NightCounter);
    }

    // Current night counter, this is -1 if its the first day, this is incremented when you go to the shop
    // first day = -1
    // first shop = 0
    // first night = 0
    // second day = 0
    // second shop = 1
    // second night = 1
    public int GetNightCounterPossiblyNegative()
    {
        return NightCounter;
    }

    public void SetNightCounter(int night)
    {
        NightCounter = night;
    }
}

[CreateAssetMenu(fileName = "DataScriptableObject", menuName = "Scriptable Objects/DataScriptableObject")]
public class ScrObjGlobalData : ScriptableObject
{
    public const int MAX_HEALTH = 100;

    private GlobalData m_globalData;
    private bool m_isInfiniteHealthCheatEnabled;
    private bool m_isNight1CheatEnabled;
    public bool m_isSettingsOpen = false;
    [SerializeField] private SaveManager m_saveManager;

    public GlobalData GetData()
    {
        return m_globalData;
    }

    private void OnEnable()
    {
        m_globalData = new();
        m_isInfiniteHealthCheatEnabled = false;
        m_isNight1CheatEnabled = false;
    }

    public void Load()
    {
        if (!Directory.Exists(m_saveManager.GetCurrentSavePath())) return;

        string filePath = m_saveManager.GetFilePath("globalData.json");

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            m_globalData = JsonUtility.FromJson<GlobalData>(json);
        }
    }

    public void Save()
    {
        if (!Directory.Exists(m_saveManager.GetCurrentSavePath())) return;

        string filePath = m_saveManager.GetFilePath("globalData.json");

        string json = JsonUtility.ToJson(m_globalData, true);
        File.WriteAllText(filePath, json);
    }

    public void IncrementNightCounter()
    {
        if (m_isNight1CheatEnabled && m_globalData.GetNightCounterPossiblyNegative() >= 0) return;
        m_globalData.SetNightCounter(m_globalData.GetNightCounterPossiblyNegative() + 1);
    }

    public void SetInfiniteHealthCheatStatus(Toggle toggleUi)
    {
        m_isInfiniteHealthCheatEnabled = toggleUi.isOn;
    }

    public void SetNight1CheatStatus(Toggle toggleUi)
    {
        m_isNight1CheatEnabled = toggleUi.isOn;
    }

    /// <summary>
    /// Decrements current health
    /// </summary>
    /// <param name="damage">Amount of damage</param>
    public void Damage(int damage)
    {
        if (m_isInfiniteHealthCheatEnabled) return;
        m_globalData.CurrentHealth -= damage;
    }
}
