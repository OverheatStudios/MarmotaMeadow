using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayDebugMenu : MonoBehaviour
{
    [SerializeField] private InventoryMager m_inventoryManager;
    [SerializeField] private BaseItem m_cornSeeds;

    public void GiveCornSeeds()
    {
        m_inventoryManager.AddItem(m_cornSeeds);
    }
}
