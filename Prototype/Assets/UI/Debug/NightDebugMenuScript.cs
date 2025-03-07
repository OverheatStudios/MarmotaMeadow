using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

public class NightDebugMenu : MonoBehaviour
{
    [SerializeField] private InventoryMager m_inventoryManager;
    [SerializeField] private BaseItem m_shotgun;
    [SerializeField] private BaseItem m_pistol;
    [SerializeField] private BaseItem m_rifle;
    [SerializeField] private BaseItem m_shotgunAmmo;
    [SerializeField] private BaseItem m_pistolAmmo;
    [SerializeField] private ShootScript m_shootScript;
    [SerializeField] private Toggle m_infiniteAmmoToggle;
    [SerializeField] private Toggle m_infiniteHealthToggle;
    [SerializeField] private DataScriptableObject m_data;

    private void Start()
    {
        m_infiniteAmmoToggle.isOn = PlayerPrefs.GetInt("infiniteAmmo", 0) != 0;
        m_shootScript.SetInfiniteAmmoCheat(m_infiniteAmmoToggle);

        m_infiniteHealthToggle.isOn = PlayerPrefs.GetInt("infiniteHealth", 0) != 0;
        m_data.SetInfiniteHealthCheatStatus(m_infiniteHealthToggle);
    }

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

    public void GivePistolAmmo()
    {
        m_inventoryManager.AddItem(m_pistolAmmo);
    }

    public void SetAmmo(int ammo)
    {
        m_shootScript.SetAmmo(ammo);
    }

    public void UpdateAmmoPref(Toggle toggle)
    {
        PlayerPrefs.SetInt("infiniteAmmo", toggle.isOn ? 1 : 0);
    }

    public void UpdateHealthPref(Toggle toggle)
    {
        PlayerPrefs.SetInt("infiniteHealth", toggle.isOn ? 1 : 0);
    }
}
