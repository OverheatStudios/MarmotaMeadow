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
    [SerializeField] private Toggle m_night1Toggle;
    [SerializeField] private ScrObjGlobalData m_data;

    private void Start()
    {
        if (DebugMenuScript.ForceDisableCheats)
        {
            m_infiniteAmmoToggle.isOn = false;
            m_shootScript.SetInfiniteAmmoCheat(m_infiniteAmmoToggle);

            m_infiniteHealthToggle.isOn = false;
            m_data.SetInfiniteHealthCheatStatus(m_infiniteHealthToggle);

            m_night1Toggle.isOn = false;
            m_data.SetNight1CheatStatus(m_night1Toggle);
        }
        else
        {
            m_infiniteAmmoToggle.isOn = PlayerPrefs.GetInt("infiniteAmmo", 0) != 0;
            m_shootScript.SetInfiniteAmmoCheat(m_infiniteAmmoToggle);

            m_infiniteHealthToggle.isOn = PlayerPrefs.GetInt("infiniteHealth", 0) != 0;
            m_data.SetInfiniteHealthCheatStatus(m_infiniteHealthToggle);

            m_night1Toggle.isOn = PlayerPrefs.GetInt("alwaysNight1", 0) != 0;
            m_data.SetNight1CheatStatus(m_night1Toggle);
        }
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

    public void UpdateNight1Pref(Toggle toggle)
    {
        PlayerPrefs.SetInt("alwaysNight1", toggle.isOn ? 1 : 0);
    }
}
