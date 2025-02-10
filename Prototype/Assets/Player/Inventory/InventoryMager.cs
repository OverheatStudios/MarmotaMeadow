using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryMager : MonoBehaviour
{
    public InventorySlot[] inventorySlots;
    public GameObject inventoryItemPrefab;
    public BaseItem item;
    public BaseItem item2;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            AddItem(item);
            AddItem(item2);
        }
    }

    public bool AddItem(BaseItem item)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem inventoryItem = slot.GetComponentInChildren<InventoryItem>();
            if (inventoryItem && inventoryItem.item == item && inventoryItem.count <= inventoryItem.item.ReturnMaxAmount() && inventoryItem.item.IsStackable())
            {
                inventoryItem.count++;
                inventoryItem.RefreshCount();   
                return true;
            }
        }
        
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem inventoryItem = slot.GetComponentInChildren<InventoryItem>();
            if (!inventoryItem)
            {
                SpawnNewItem(item, slot);
                return true;
            }
        }
        return false;
    }

    private void SpawnNewItem(BaseItem item, InventorySlot slot)
    {
        GameObject newItem = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
        inventoryItem.InitializeItem(item);
    }
}
