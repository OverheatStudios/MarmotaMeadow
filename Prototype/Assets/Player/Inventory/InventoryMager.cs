using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.IO;

[System.Serializable]
public class InventoryData
{
    public String slotName;
    public int amount;
    public float multiplier;
    public BaseItem item;
}

public class InventoryMager : MonoBehaviour
{
    public InventorySlot[] inventorySlots;
    public GameObject inventoryItemPrefab;
    public BaseItem item;
    public BaseItem item2;
    public float coins;
    
    private string filePath;
    void Start()
    {
        filePath = Application.dataPath + "/playerData.json";
        LoadInventory();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            //AddItem(item);
            AddItem(item2);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SaveInventory();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            LoadInventory();
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
                inventoryItem.IncreaseAmount();
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

    private void SaveInventory()
    {
        List<InventoryData> inventoryDataList = new List<InventoryData>();

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].GetComponentInChildren<InventoryItem>())
            {
                InventoryData inventoryData = new InventoryData
                {
                    slotName = inventorySlots[i].gameObject.name, // Save the slot name
                    item = inventorySlots[i].GetComponentInChildren<InventoryItem>().item, // Save the item name
                    amount = inventorySlots[i].GetComponentInChildren<InventoryItem>().ReturnAmount(),
                    multiplier = inventorySlots[i].GetComponentInChildren<InventoryItem>().ReturnMultiplier()
                };

                inventoryDataList.Add(inventoryData);
            }
        }
        
        // Wrap the list in a wrapper class
        InventoryWrapper wrapper = new InventoryWrapper { inventoryDataList = inventoryDataList };

        // Serialize the wrapper to JSON
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(filePath, json);
    }
    
    private void LoadInventory()
    {
        if (File.Exists(filePath))
        {
            // Read the JSON file
            string json = File.ReadAllText(filePath);

            // Deserialize the JSON back into the wrapper class
            InventoryWrapper wrapper = JsonUtility.FromJson<InventoryWrapper>(json);

            for (int i = 0; i < wrapper.inventoryDataList.Count; i++)
            {
                for (int j = 0; j < inventorySlots.Length; j++)
                {
                    if (wrapper.inventoryDataList[i].slotName == inventorySlots[j].gameObject.name)
                    {
                        GameObject newItem = Instantiate(inventoryItemPrefab, inventorySlots[j].transform);
                        InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
                        inventoryItem.InitializeItem(wrapper.inventoryDataList[i].item);
                        inventoryItem.SetAmount(wrapper.inventoryDataList[i].amount);
                        inventoryItem.SetMultiplier(wrapper.inventoryDataList[i].multiplier);
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("Inventory file not found at " + filePath);
        }
    }

    public void IncreaseCoins(float amount)
    {
        coins += amount;
    }

    public void DecreaseCoins(float amount)
    {
        coins -= amount;
    }

    public float GetCoins()
    {
        return coins;
    }
    
}

[System.Serializable]
public class InventoryWrapper
{
    public List<InventoryData> inventoryDataList;
}
