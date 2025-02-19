using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryActions : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private List<InventorySlot> slots = new List<InventorySlot>(); // List of inventory slots
    [SerializeField] private int m_selectedItemIndex;
    [SerializeField] private GameObject m_inventoryUI;
    [SerializeField] private bool m_inInventory;

    // Update is called once per frame
    void Update()
    {
        HandleSlotChange();

        ToggleInventory();
    }

    void HandleSlotChange()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            slots[m_selectedItemIndex].GetComponent<InventorySlot>().Deselect();
            m_selectedItemIndex++;
            if (m_selectedItemIndex >= 9)
            {
                m_selectedItemIndex = 0;
                slots[m_selectedItemIndex].GetComponent<InventorySlot>().Select();
            }
            else
            {
                slots[m_selectedItemIndex].GetComponent<InventorySlot>().Select();
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            slots[m_selectedItemIndex].GetComponent<InventorySlot>().Deselect();
            m_selectedItemIndex--;
            if (m_selectedItemIndex <= -1)
            {
                m_selectedItemIndex = 8;
                slots[m_selectedItemIndex].GetComponent<InventorySlot>().Select();
            }
            else
            {
                slots[m_selectedItemIndex].GetComponent<InventorySlot>().Select();
            }
        }
    }

    void ToggleInventory()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !m_inInventory)
        {
            //UI
            m_inInventory = true;
            m_inventoryUI.SetActive(true);

            //Cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && m_inInventory)
        {
            //UI
            m_inInventory = false;
            m_inventoryUI.SetActive(false);

            //Cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
