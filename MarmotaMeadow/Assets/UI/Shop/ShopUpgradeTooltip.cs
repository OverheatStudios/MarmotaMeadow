using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUpgradeTooltip : MonoBehaviour
{
    [SerializeField] private ShopTooltip m_shopTooltip;
    [SerializeField] private InventorySlot m_inventorySlot;
    [SerializeField] private string m_toolText = "Increases Crop Harvest Yield";
    [SerializeField] private string m_gunText = "Increases Bullet Damage";

    void Update()
    {
        InventoryItem inventoryItem = m_inventorySlot.GetComponentInChildren<InventoryItem>();
        if (inventoryItem == null || inventoryItem.item == null)
        {
            m_shopTooltip.SetVisible(false);
            return;
        }
        print("item: " + inventoryItem);

        m_shopTooltip.SetText(inventoryItem.item is Gun ? m_gunText : m_toolText);
        m_shopTooltip.SetVisible(true);
    }
}
