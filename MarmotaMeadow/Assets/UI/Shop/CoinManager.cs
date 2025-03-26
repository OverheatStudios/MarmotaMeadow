using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;


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
    // Start is called before the first frame update

    void Start()
    {
        filePath = m_saveManager.GetFilePath(m_saveLocation);
        Load();
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

    public void Save()
    {
        coinData.numberOfCoins = coins;
        string json = JsonUtility.ToJson(coinData, true);
        File.WriteAllText(filePath, json);
    }

    public void IncreaseCoins(float amount)
    {
        coins += amount;
        AudioSource.PlayClipAtPoint(m_gainCoinSfx, Camera.main.transform.position);
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
