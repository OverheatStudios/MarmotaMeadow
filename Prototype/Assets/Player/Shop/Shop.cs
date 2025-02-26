using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Shop : MonoBehaviour
{
    [SerializeField] InventoryMager inventoryMager;
    [SerializeField] private TextMeshProUGUI CoinsText;
    [SerializeField] private PlotManager plotManager;

    public void BuyItem(BaseItem item)
    {
        inventoryMager.AddItem(item);
        inventoryMager.DecreaseCoins(item.ReturnBuyCoinsAmount());
        CoinsText.text = "Coins: " + inventoryMager.GetCoins();
    }

    public void SellItem(InventorySlot item)
    {
        if (item.GetComponentInChildren<InventoryItem>().item is Crops crop)
        {
            inventoryMager.IncreaseCoins(crop.ReturnSellCoinsAmount());
            item.GetComponentInChildren<InventoryItem>().DecreaseAmount();
            CoinsText.text = "Coins: " + inventoryMager.GetCoins();
        }
    }

    public void UpgradeTool(InventorySlot item)
    {
        if (item.GetComponentInChildren<InventoryItem>().item is Tool tool)
        {
            inventoryMager.DecreaseCoins(tool.ReturnToolLevelCost());
            item.GetComponentInChildren<InventoryItem>().IncreaseMultiplier();
            CoinsText.text = "Coins: " + inventoryMager.GetCoins();
        }
    }

    public void BuyPlot(int money)
    {
        plotManager.IncreaseNumberOfPlots();
        inventoryMager.DecreaseCoins(money);
        CoinsText.text = "Coins: " + inventoryMager.GetCoins();
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
