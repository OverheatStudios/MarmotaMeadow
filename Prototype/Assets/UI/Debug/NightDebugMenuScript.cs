using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightDebugMenu : MonoBehaviour
{
    [SerializeField] private InventoryMager m_inventoryManager;
    [SerializeField] private BaseItem m_shotgunPrefab;
    [SerializeField] private BaseItem m_pistolPrefab;
    [SerializeField] private BaseItem m_riflePrefab;

    public void GiveShotgun()
    {
        m_inventoryManager.AddItem(m_shotgunPrefab);
    }

    public void GivePistol()
    {
        m_inventoryManager.AddItem(m_pistolPrefab);
    }

    public void GiveRifle()
    {
        m_inventoryManager.AddItem(m_riflePrefab);
    }
}
