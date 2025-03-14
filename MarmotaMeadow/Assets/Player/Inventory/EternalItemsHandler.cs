using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Give the player one of each item on start unless they already have the item
/// </summary>
public class EternalItemsHandler : MonoBehaviour
{
    [SerializeField] private InventoryMager m_inventoryManager;
    [SerializeField] private List<BaseItem> m_eternalItems;

    private void Start()
    {
        Assert.IsTrue(m_inventoryManager.IsInventoryLoadedYet());

        foreach (var item in m_eternalItems)
        {
            GiveItemIfMissing(item);
        }
    }

    private void GiveItemIfMissing(BaseItem item)
    {
        // Check if player has item
        for (int i = 0; i < m_inventoryManager.GetSize(); ++i)
        {
            BaseItem baseItem = m_inventoryManager.GetItem(i);
            if (baseItem != null && baseItem.name == item.name)
            {
                return;
            }
        }

        m_inventoryManager.AddItem(item);

    }
}
