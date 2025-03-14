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
            coinManager.IncreaseCoins(crop.ReturnSellCoinsAmount());
            item.GetComponentInChildren<InventoryItem>().DecreaseAmount();
        }
    }

    public void UpgradeTool(InventorySlot item)
    {
        if (item.GetComponentInChildren<InventoryItem>().item is Tool tool)
        {
            coinManager.DecreaseCoins(tool.ReturnToolLevelCost());
            item.GetComponentInChildren<InventoryItem>().IncreaseMultiplier();
        }
    }

    public void BuyPlot(int money)
    {
        plotManager.IncreaseNumberOfPlots();
        coinManager.DecreaseCoins(money);
    }

    public void GoToNight()
    {
        SceneManager.LoadScene("NightScene");
    }

    public void DestroyGameObject(GameObject objectToDestroy)
    {
        Destroy(objectToDestroy);
    }
}
