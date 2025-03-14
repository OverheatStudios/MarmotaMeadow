using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryActions : MonoBehaviour
{
    /// <summary>
    /// If false, player cannot open inventory and can only use tool bar
    /// </summary>
    [SerializeField] private bool m_canOpenInventory = true;

    [Header("Inventory")]
    [SerializeField] private List<InventorySlot> slots = new List<InventorySlot>(); // List of inventory slots
    [SerializeField] private int m_selectedItemIndex;
    [SerializeField] private GameObject m_inventoryUI;
    [SerializeField] private bool m_inInventory;
    [SerializeField] private InventoryMager m_inventoryManager;
    [SerializeField] private CursorHandlerScript m_cursorHandler;
    [SerializeField] private ToggleSettings m_toggleSettings;

    private void Start()
    {
        m_inventoryManager.NotifyNewSelectedItemIndex(m_selectedItemIndex);
    }

    // Update is called once per frame
    void Update()
    {
        HandleSlotChange();

        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleInventory();
        }
    }

    void HandleSlotChange()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            int slot = m_selectedItemIndex + 1;
            if (slot >= 9)
            {
                slot = 0;
            }
            SelectSlot(slot);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            int slot = m_selectedItemIndex - 1;
            if (slot <= -1)
            {
                slot = 8;
            }
            SelectSlot(slot);
        }
        else
        {
            for (int i = 0; i < 9; ++i)
            {
                if (Input.GetKeyDown("" + (i + 1)))
                {
                    SelectSlot(i);
                    return;
                }
            }
        }
    }

    private void SelectSlot(int slot)
    {
        slots[m_selectedItemIndex].GetComponent<InventorySlot>().Deselect();
        m_selectedItemIndex = slot;
        m_inventoryManager.NotifyNewSelectedItemIndex(slot);
        slots[m_selectedItemIndex].GetComponent<InventorySlot>().Select();
    }

    public void ToggleInventory()
    {
        if (!m_canOpenInventory) return;

        if (!m_inInventory && (!m_toggleSettings || !m_toggleSettings.IsToggled()))
        {
            //UI
            m_inInventory = true;
            m_inventoryUI.SetActive(true);

            //Cursor
            m_cursorHandler.NotifyUiOpen();
        }
        else if (m_inInventory && (!m_toggleSettings || !m_toggleSettings.IsToggled()))
        {
            //UI
            m_inInventory = false;
            m_inventoryUI.SetActive(false);

            //Cursor
            m_cursorHandler.NotifyUiClosed();
        }
    }
}
