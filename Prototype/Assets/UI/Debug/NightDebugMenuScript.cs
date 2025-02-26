using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightDebugMenu : MonoBehaviour
{
    [SerializeField] private InventoryMager m_inventoryManager;
    [SerializeField] private BaseItem m_shotgun;
    [SerializeField] private BaseItem m_pistol;
    [SerializeField] private BaseItem m_rifle;
    [SerializeField] private BaseItem m_shotgunAmmo;
    [SerializeField] private ShootScript m_shootScript;

    public void GiveShotgun()
    {
        m_inventoryManager.AddItem(m_shotgun);
    }

    public void GivePistol()
    {
        m_inventoryManager.AddItem(m_pistol);
    }

    public void GiveRifle()
    {
        m_inventoryManager.AddItem(m_rifle);
    }

    public void GiveShotgunAmmo()
    {
        m_inventoryManager.AddItem(m_shotgunAmmo);
    }

    public void SetAmmo(int ammo)
    {
        m_shootScript.SetAmmo(ammo);
    }
}
