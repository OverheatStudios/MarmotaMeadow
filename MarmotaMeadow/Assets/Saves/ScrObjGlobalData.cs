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
    public int NightCounter = -1;
    public int CurrentHealth = ScrObjGlobalData.MAX_HEALTH;
}

[CreateAssetMenu(fileName = "DataScriptableObject", menuName = "Scriptable Objects/DataScriptableObject")]
public class ScrObjGlobalData : ScriptableObject
{
    public const int MAX_HEALTH = 100;

    private GlobalData m_globalData;
    private bool m_isInfiniteHealthCheatEnabled;
    [SerializeField] private SaveManager m_saveManager;

    public GlobalData GetData()
    {
        return m_globalData;
    }

    private void OnEnable()
    {
        m_globalData = new();
        m_isInfiniteHealthCheatEnabled = false;
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

    public void SetInfiniteHealthCheatStatus(Toggle toggleUi)
    {
        m_isInfiniteHealthCheatEnabled = toggleUi.isOn;
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
