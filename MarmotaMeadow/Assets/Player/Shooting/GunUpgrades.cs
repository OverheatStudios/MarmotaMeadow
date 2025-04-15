using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class GunUpgradeData
{
    public int ExtraBulletsLevel = 0;
    public int NoAmmoLevel = 0;
    public int SpeedLevel = 0;
}

public class GunUpgrades : MonoBehaviour
{
    [SerializeField] private SaveManager m_saveManager;
    private GunUpgradeData m_gunUpgradeData = null;

    private void Awake()
    {
        string path = GetSaveFilePath();
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            m_gunUpgradeData = JsonUtility.FromJson<GunUpgradeData>(json);
        }

        m_gunUpgradeData ??= new();
    }

    private void OnDestroy()
    {
        File.WriteAllText(GetSaveFilePath(), JsonUtility.ToJson(m_gunUpgradeData));
    }

    private string GetSaveFilePath()
    {
        return m_saveManager.GetFilePath("ShopUpgrades.json");
    }

    public GunUpgradeData GetRawData()
    {
        return m_gunUpgradeData;
    }

    public int GetExtraBullets()
    {
        return GetRawData().ExtraBulletsLevel;
    }

    public bool ShouldConsumeAmmo()
    {
        float ammoNoConsumeChance = Mathf.Clamp01(GetRawData().NoAmmoLevel / 10.0f);
        return Random.Range(0.0f, 1.0f) > ammoNoConsumeChance;
    }

    public float GetShootSpeedScalar()
    {
        return Mathf.InverseLerp(1.0f, 0.1f, GetRawData().SpeedLevel / 10.0f);
    }
}
