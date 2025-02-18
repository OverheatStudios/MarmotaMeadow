using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] InventoryMager inventoryMager;
    [SerializeField] private TextMeshProUGUI CoinsText;

    public void BuyItem(Seeds item)
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
}
