using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[System.Serializable]
public enum ShopUpgradeType
{
    Ammo, Speed, BulletCount
}

public class ShopUpgrade : MonoBehaviour
{
    [SerializeField] private GunUpgrades m_gunUpgrades;
    [SerializeField] private CoinManager m_coinManager;
    [SerializeField] private TextMeshProUGUI m_text;
    [SerializeField] private Button m_button;
    [SerializeField] private string m_upgradeName;
    [SerializeField] private ShopUpgradeType m_upgradeType;
    [Tooltip("Upgrade cost per level, element 0 is first upgrade")]
    [SerializeField] private List<int> m_upgradeCosts;

    private void Start()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        int level = GetLevel(m_upgradeType);
        if (m_upgradeCosts.Count > level)
        {
            m_text.text = m_upgradeName + " | LVL: " + level +" | Price: " + m_upgradeCosts[level];
        }
        else
        {
            m_text.text = m_upgradeName + " (Max Level)";
        }
    }

    private int GetLevel(ShopUpgradeType upgradeType)
    {
        GunUpgradeData data = m_gunUpgrades.GetRawData();
        switch (upgradeType)
        {
            case ShopUpgradeType.Ammo:
                return data.NoAmmoLevel;
            case ShopUpgradeType.Speed:
                return data.SpeedLevel;
            case ShopUpgradeType.BulletCount:
                return data.ExtraBulletsLevel;
            default:
                Assert.IsTrue(false);
                return 0;
        }
    }

    private void SetLevel(ShopUpgradeType upgradeType, int level)
    {
        GunUpgradeData data = m_gunUpgrades.GetRawData();
        switch (upgradeType)
        {
            case ShopUpgradeType.Ammo:
                data.NoAmmoLevel = level;
                return;
            case ShopUpgradeType.Speed:
                data.SpeedLevel = level;
                return;
            case ShopUpgradeType.BulletCount:
                data.ExtraBulletsLevel = level;
                return;
            default:
                Assert.IsTrue(false);
                return;
        }
    }

    public void TryBuy()
    {
        int level = GetLevel(m_upgradeType);
        if (m_upgradeCosts.Count > level)
        {
            int price = m_upgradeCosts[level];
            if (price <= m_coinManager.GetCoins())
            {
                m_coinManager.DecreaseCoins(price);
                SetLevel(m_upgradeType, level + 1);
            }
        }

        UpdateText();
    }
}
