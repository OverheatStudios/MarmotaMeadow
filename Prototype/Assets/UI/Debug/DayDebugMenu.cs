using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayDebugMenu : MonoBehaviour
{
    [SerializeField] private InventoryMager m_inventoryManager;
    [SerializeField] private BaseItem m_cornSeeds;
    private bool m_isInfiniteDay = false;

    private void Start()
    {
        m_isInfiniteDay = PlayerPrefs.GetInt("infiniteDay", 0) == 1;
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
