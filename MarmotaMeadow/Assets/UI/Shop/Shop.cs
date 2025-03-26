using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class Shop : MonoBehaviour
{
    [SerializeField] InventoryMager inventoryMager;
    [SerializeField] private TextMeshProUGUI CoinsText;
    [SerializeField] private PlotManager plotManager;
    [SerializeField] private CoinManager coinManager;
    [SerializeField] private TextMeshProUGUI m_upgradePriceText;
    [SerializeField] private GameObject m_upgradePriceIcon;
    [SerializeField] private TextMeshProUGUI m_sellPriceText;
    [SerializeField] private GameObject m_sellPriceIcon;
    [SerializeField] private GameObject m_cannotSellText;
    [SerializeField] private GameObject m_cannotUpgradeText;

    public void BuyItem(BaseItem item)
    {
        Assert.IsTrue(item != null);
        Assert.IsTrue(item.ReturnBuyCoinsAmount() > 0);
        if (coinManager.GetCoins() < item.ReturnBuyCoinsAmount())
        {
            return;
        }
        inventoryMager.AddItem(item);
        coinManager.DecreaseCoins(item.ReturnBuyCoinsAmount());
    }

    private void Start()
    {
        m_upgradePriceText.gameObject.SetActive(false);
        m_upgradePriceIcon.SetActive(false);
        m_sellPriceText.gameObject.SetActive(false);
        m_sellPriceIcon.SetActive(false);
    }

    public void Update()
    {
        ShowPrice(inventoryMager.GetUpgradeSlot(), m_upgradePriceIcon, m_upgradePriceText, m_cannotUpgradeText, true);
        ShowPrice(inventoryMager.GetSellSlot(), m_sellPriceIcon, m_sellPriceText, m_cannotSellText, false);
    }

    private void ShowPrice(InventorySlot inventorySlot, GameObject priceIcon, TextMeshProUGUI priceText, GameObject cannotText, bool isUpgrade)
    {
        InventoryItem item = inventorySlot?.GetComponentInChildren<InventoryItem>();
        if (item == null || item.ReturnAmount() == 0)
        {
            priceIcon.SetActive(false);
            priceText.gameObject.SetActive(false);
            cannotText.SetActive(false);
        }
        else
        {
            float price = -1;
            if (isUpgrade && item.item is Tool tool)
            {
                price = tool.ReturnToolLevelCost();
            }
            else if (isUpgrade && item.item is Gun gun)
            {
                price = gun.GetUpgradeCost();
            }
            else if (!isUpgrade && item.item is Crops crops)
            {
                price = crops.ReturnSellCoinsAmount() * item.ReturnAmount();
            }

            if (price == -1)
            {
                cannotText.SetActive(true); return;
            }

            priceIcon.SetActive(true);
            priceText.gameObject.SetActive(true);
            priceText.text = ((int)price).ToString();
            cannotText.SetActive(false);
        }
    }

    public void AddCoins(float coins)
    {
        coinManager.IncreaseCoins(coins);
    }

    public void SetCoins(float coins)
    {
        coinManager.SetCoins(coins);
    }

    public void SellItem(InventorySlot item)
    {
        if (item.GetComponentInChildren<InventoryItem>().item is Crops crop)
        {
            coinManager.IncreaseCoins(crop.ReturnSellCoinsAmount() * item.GetComponentInChildren<InventoryItem>().ReturnAmount());
            item.GetComponentInChildren<InventoryItem>().SetAmount(0);
        }
    }

    public void UpgradeTool(InventorySlot item)
    {
        if (item.GetComponentInChildren<InventoryItem>().item is Tool tool)
        {
            if (coinManager.GetCoins() < tool.ReturnToolLevelCost()) return;

            coinManager.DecreaseCoins(tool.ReturnToolLevelCost());
            item.GetComponentInChildren<InventoryItem>().IncreaseMultiplier();
        }
        else if (item.GetComponentInChildren<InventoryItem>().item is Gun gun)
        {
            if (coinManager.GetCoins() < gun.GetUpgradeCost()) return;

            coinManager.DecreaseCoins(gun.GetUpgradeCost());
            item.GetComponentInChildren<InventoryItem>().IncreaseMultiplier();
        }
    }

    public void BuyPlot(int money)
    {
        plotManager.IncreaseNumberOfPlots();
        coinManager.DecreaseCoins(money);
    }

    public void DestroyGameObject(GameObject objectToDestroy)
    {
        Destroy(objectToDestroy);
    }
}
