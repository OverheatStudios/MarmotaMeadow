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
    [SerializeField] private BuyMultiplier m_buyMultiplier;

    public void BuyItem(BaseItem item)
    {
        int multiplier = item.IsStackable() ? m_buyMultiplier.GetCurrentMultiplier() : 1;
        float price = item.ReturnBuyCoinsAmount() * multiplier;
        Assert.IsTrue(item != null);
        Assert.IsTrue(price > 0);
        if (coinManager.GetCoins() < price)
        {
            return;
        }
        inventoryMager.AddItem(item, multiplier);
        coinManager.DecreaseCoins(price);
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
            float price = 0;
            if (isUpgrade && item.item is Tool tool)
            {
                price = tool.ReturnToolLevelCost() * m_buyMultiplier.GetCurrentMultiplier();
            }
            else if (isUpgrade && item.item is Gun gun)
            {
                price = gun.GetUpgradeCost() * m_buyMultiplier.GetCurrentMultiplier();
            }
            else if (!isUpgrade && item.item is Crops crops)
            {
                price = crops.ReturnSellCoinsAmount() * item.ReturnAmount() * m_buyMultiplier.GetCurrentMultiplier();
            }
            else if (!isUpgrade && item.item is Seeds seeds)
            {
                price = seeds.GetSellPrice() * item.ReturnAmount();
            }

            if (price == 0)
            {
                cannotText.SetActive(true);
                return;
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
        if (item == null || item.GetComponentInChildren<InventoryItem>() == null || item.GetComponentInChildren<InventoryItem>().item == null) return;

        if (item.GetComponentInChildren<InventoryItem>().item is Crops crop)
        {
            float price = crop.ReturnSellCoinsAmount() * item.GetComponentInChildren<InventoryItem>().ReturnAmount();
            if (price == 0) return;
            coinManager.IncreaseCoins(price);
            item.GetComponentInChildren<InventoryItem>().SetAmount(0);
        }
        else if (item.GetComponentInChildren<InventoryItem>().item is Seeds seeds)
        {
            float price = seeds.GetSellPrice() * item.GetComponentInChildren<InventoryItem>().ReturnAmount();
            if (price == 0) return;
            coinManager.IncreaseCoins(price);
            item.GetComponentInChildren<InventoryItem>().SetAmount(0);
        }
    }

    public void UpgradeTool(InventorySlot item)
    {
        int multiplier = m_buyMultiplier.GetCurrentMultiplier();
        if (item == null || item.GetComponentInChildren<InventoryItem>() == null || item.GetComponentInChildren<InventoryItem>().item == null) return;
        if (item.GetComponentInChildren<InventoryItem>().item is Tool tool)
        {
            if (coinManager.GetCoins() < tool.ReturnToolLevelCost() * multiplier || tool.ReturnToolLevelCost() == 0) return;

            coinManager.DecreaseCoins(tool.ReturnToolLevelCost()* multiplier);
            item.GetComponentInChildren<InventoryItem>().IncreaseMultiplier(multiplier);
        }
        else if (item.GetComponentInChildren<InventoryItem>().item is Gun gun)
        {
            if (coinManager.GetCoins() < gun.GetUpgradeCost()* multiplier || gun.GetUpgradeCost() == 0) return;

            coinManager.DecreaseCoins(gun.GetUpgradeCost()* multiplier);
            item.GetComponentInChildren<InventoryItem>().IncreaseMultiplier(multiplier);
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
