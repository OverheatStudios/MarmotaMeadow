using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;


[System.Serializable]
public class CoinData
{
    public float numberOfCoins = 1;
}

public class CoinManager : MonoBehaviour
{
    [SerializeField] private float coins;

    [Header("Saving")]
    [SerializeField] private string m_saveLocation;
    [SerializeField] private TextMeshProUGUI CoinsText;
    private string filePath;
    [SerializeField] private CoinData coinData;
    [SerializeField] private SaveManager m_saveManager;
    [SerializeField] private AudioClip m_gainCoinSfx;
    [SerializeField] private SettingsScriptableObject m_settings;
    [SerializeField] private GameObject m_coinDisplay;
    [SerializeField] private float m_coinAnimationDuration = 0.25f;
    [SerializeField] private float m_coinAnimationIntensity = 0.2f;
    private Vector3 m_coinDisplayStartingScale;
    private float m_secondsSinceCoinAnimationStarted = -1;

    void Start()
    {
        filePath = m_saveManager.GetFilePath(m_saveLocation);
        Load();
        m_coinDisplayStartingScale = m_coinDisplay.transform.localScale;
    }

    private void Load()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            CoinData data = JsonUtility.FromJson<CoinData>(json);

            if (data != null)
            {
                coins = data.numberOfCoins;
                if (CoinsText) CoinsText.text = GetCoins().ToString();
            }
        }
    }

    private void Update()
    {
        if (m_secondsSinceCoinAnimationStarted >= 0)
        {
            m_secondsSinceCoinAnimationStarted += Time.deltaTime;
            float t = Mathf.InverseLerp(0, m_coinAnimationDuration, m_secondsSinceCoinAnimationStarted);
            if (t > 0.5f) t = 1.0f - t;
            t *= 2;
            Assert.IsTrue(t >= 0 && t <= 1);
            m_coinDisplay.transform.localScale = Vector3.Lerp(m_coinDisplayStartingScale, m_coinDisplayStartingScale + Vector3.one * m_coinAnimationIntensity, t);
        }
    }

    public void Save()
    {
        coinData.numberOfCoins = coins;
        string json = JsonUtility.ToJson(coinData, true);
        File.WriteAllText(filePath, json);
    }

    public void IncreaseCoins(float amount)
    {
        coins += amount;
        AudioSource.PlayClipAtPoint(m_gainCoinSfx, Camera.main.transform.position, m_settings.GetSettings().GetGameVolume());
        m_secondsSinceCoinAnimationStarted = 0;
        if (CoinsText) CoinsText.text = GetCoins().ToString();
    }

    public void SetCoins(float amount)
    {
        coins = amount;
        if (CoinsText) CoinsText.text = GetCoins().ToString();
    }

    public void DecreaseCoins(float amount)
    {
        coins -= amount;
        m_secondsSinceCoinAnimationStarted = 0;
        CoinsText.text = GetCoins().ToString();
    }

    public float GetCoins()
    {
        return coins;
    }

    public void OnDestroy()
    {
        Save();
    }
}
