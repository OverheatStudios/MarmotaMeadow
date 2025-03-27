using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayDebugMenu : MonoBehaviour
{
    [SerializeField] private InventoryMager m_inventoryManager;
    [SerializeField] private BaseItem m_cornSeeds;
    [SerializeField] private Toggle m_infiniteDayToggle;
    private bool m_isInfiniteDay = false;

    private void Start()
    {
        if (DebugMenuScript.ForceDisableCheats)
        {
            m_isInfiniteDay = false;
        }
        else
        {
            m_isInfiniteDay = PlayerPrefs.GetInt("infiniteDay", 0) == 1;
        }
        m_infiniteDayToggle.isOn = m_isInfiniteDay;
    }

    public bool IsInfiniteDay()
    {
        return m_isInfiniteDay;
    }

    public void GiveCornSeeds()
    {
        m_inventoryManager.AddItem(m_cornSeeds);
    }

    public void ToggleInfiniteDay(Toggle toggle)
    {
        m_isInfiniteDay = toggle.isOn;
        PlayerPrefs.SetInt("infiniteDay", toggle.isOn ? 1 : 0);
    }
}
