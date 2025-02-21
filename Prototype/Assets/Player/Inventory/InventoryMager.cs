using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.IO;
using UnityEngine.Assertions;
using System.Linq;

[System.Serializable]
public class InventoryData
{
    public String slotName;
    public int amount;
    public float multiplier;
    public string item;
}

public class InventoryMager : MonoBehaviour
{
    public InventorySlot[] inventorySlots;
    public GameObject inventoryItemPrefab;
    public BaseItem item;
    public BaseItem item2;
    public float coins;
    [SerializeField] private GameObject inventoryItem;
    [SerializeField] private BaseItem[] items;
    
    private string filePath;

    [SerializeField] private string m_saveLocation;

    private int m_selectedItemIndex = -1;

    void Start()
    {
        Assert.IsNotNull(m_saveLocation);
        filePath = Application.dataPath + "/" + m_saveLocation;
        LoadInventoryFromFile();
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
            SaveInventoryToFile();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            LoadInventoryFromFile();
        }

        if (inventoryItem)
        {
            inventoryItem.transform.position = Input.mousePosition;
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

    /// <summary>
    /// Get the item at an index, may return null
    /// </summary>
    /// <param name="index">Index, must be valid index</param>
    /// <returns>The BaseItem or null</returns>
    public BaseItem GetItem(int index)
    {
        Assert.IsTrue(index >= 0 && index < inventorySlots.Count());
        InventorySlot slot = inventorySlots[index];
        InventoryItem item = slot.GetComponentInChildren<InventoryItem>();
        if (item == null) return null;
        return item.item;
    }

    public InventoryItem GetInventoryItem(int index)
    {
        Assert.IsTrue(index >= 0 && index < inventorySlots.Count());
        Debug.Log(inventorySlots[index]);
        InventorySlot slot = inventorySlots[index];
        InventoryItem item = inventorySlots[index].GetComponentInChildren<InventoryItem>();
        if (item == null) return null;
        return item;
    }

    /// <summary>
    /// Get the item the player is holding
    /// </summary>
    /// <returns>The item, or null</returns>
    public BaseItem GetHeldItem()
    {
        return GetItem(m_selectedItemIndex);
    }

    public InventoryItem GetHeldInventoryItem()
    {
        return GetInventoryItem(m_selectedItemIndex);
    }

    private void SpawnNewItem(BaseItem item, InventorySlot slot)
    {
        GameObject newItem = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
        inventoryItem.InitializeItem(item);
        inventoryItem.SetInventory(this);
    }

    private void SaveInventoryToFile()
    {
        List<InventoryData> inventoryDataList = new List<InventoryData>();

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].GetComponentInChildren<InventoryItem>())
            {
                InventoryData inventoryData = new InventoryData
                {
                    slotName = inventorySlots[i].gameObject.name, // Save the slot name
                    item = inventorySlots[i].GetComponentInChildren<InventoryItem>().item.name, // Save the item name
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
    
    private void LoadInventoryFromFile()
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
                        for (int k = 0; k < items.Length; k++)
                        {
                            if (wrapper.inventoryDataList[i].item == items[i].name)
                            {
                                inventoryItem.InitializeItem(items[i]);
                            }
                        }
                        inventoryItem.SetAmount(wrapper.inventoryDataList[i].amount);
                        inventoryItem.SetMultiplier(wrapper.inventoryDataList[i].multiplier);
                        inventoryItem.SetInventory(this);
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

    public void SetCoins(float amount)
    {
        coins = amount;
    }

    public void SetInventoryItem(GameObject itemToDrag)
    {
        inventoryItem = itemToDrag;
        itemToDrag.transform.parent = transform.root;
    }
    
    public void SetInventoryItem()
    {
        inventoryItem = null;
    }

    public GameObject ReturnInventoryItem()
    {
        return inventoryItem;
    }
    
    /// <summary>
    /// Let the inventory manager know that the player changed held item slot
    /// </summary>
    /// <param name="index">New index</param>
    public void NotifyNewSelectedItemIndex(int index)
    {
        m_selectedItemIndex = index;
    }

    public int GetSelectedItemIndex()
    {
        return m_selectedItemIndex;
    }
}

[System.Serializable]
public class InventoryWrapper
{
    public List<InventoryData> inventoryDataList;
}
