using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.IO;
using UnityEngine.Assertions;
using System.Linq;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.SceneManagement;

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
    public const int TOOLBAR_START_INDEX = 0;
    public const int TOOLBAR_SIZE = 9;
    public InventorySlot[] inventorySlots; // The players inventory
    public GameObject inventoryItemPrefab;
    public BaseItem item;
    public BaseItem item2;
    [SerializeField] private GameObject inventoryItem;
    [SerializeField] private BaseItem[] items; // Every registered item type
    [SerializeField] private GameObject pistolButton;
    [SerializeField] private GameObject shotgunButton;
    [SerializeField] private SaveManager m_saveManager;

    [SerializeField] private string m_saveLocation;

    private int m_selectedItemIndex = -1;
    private bool m_isLoaded = false;
    
    [SerializeField] private GameObject m_upgradeslot;
    [SerializeField] private GameObject m_sellSlot;

    void OnEnable()
    {
        if (!m_isLoaded)
        {
            Assert.IsNotNull(m_saveLocation);
            LoadInventoryFromFile();
            m_isLoaded = true;
        }

        if (pistolButton)
        {
            if (HasBoughtPistol())
            {
                Destroy(pistolButton);
            }
        }

        if (shotgunButton)
        {
            if (HasBoughtShotgun())
            {
                Destroy(shotgunButton);
            }
        }
    }

    private void OnDestroy()
    {
        SaveInventoryToFile();
    }

    // Update is called once per frame
    void Update()
    {
        if (inventoryItem)
        {
            inventoryItem.transform.position = Input.mousePosition;
        }
    }

    public BaseItem[] GetItemTypes()
    {
        return items;
    }

    public bool IsInventoryLoadedYet()
    {
        return m_isLoaded;
    }

    /// <summary>
    /// Can be used to check if the player has an item of a specific category, e.g Seeds, Gun
    /// </summary>
    /// <typeparam name="T">Item category</typeparam>
    /// <returns>True if the player has at least 1 of the category</returns>
    public bool HasItemCategory<T>() where T : BaseItem
    {
        return inventorySlots.Any(slot =>
        {
            InventoryItem slotItem = slot.GetComponentInChildren<InventoryItem>();
            if (slotItem == null) return false;
            return slotItem.item is T;
        });
    }

    public InventorySlot GetUpgradeSlot()
    {
        return m_upgradeslot?.GetComponent<InventorySlot>();
    }

    public InventorySlot GetSellSlot()
    {
        return m_sellSlot?.GetComponent<InventorySlot>();
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

    /// <summary>
    /// Check how many of an item the player owns
    /// </summary>
    /// <param name="item">The item</param>
    /// <returns>The amount of the item in the players inventory, sums all stacks</returns>
    public int CountItemsOwned(BaseItem item)
    {
        int quanity = 0;
        foreach (InventorySlot slot in inventorySlots)
        {
            InventoryItem slotItem = slot.GetComponentInChildren<InventoryItem>();
            if (slotItem == null || !slotItem.IsThisType(item)) continue;
            quanity += slotItem.count;
        }
        return quanity;
    }

    /// <summary>
    /// Remove items from the players inventory, will remove items even if the player has less than `count` items (e.g if you remove 5 apples and player has 3 apples, player will end up with 0 apples)
    /// </summary>
    /// <param name="item">Item</param>
    /// <param name="count">Amount to remove, must be greater than 0</param>
    /// <returns>True if player had enough items, false if they didn't</returns>
    public bool RemoveItems(BaseItem item, int count)
    {
        Assert.IsTrue(count > 0);

        foreach (InventorySlot slot in inventorySlots)
        {
            InventoryItem slotItem = slot.GetComponentInChildren<InventoryItem>();
            if (slotItem == null || !slotItem.IsThisType(item)) continue;

            if (count >= slotItem.count)
            {
                // Remove the whole stack
                count -= slotItem.count;
                slotItem.DecreaseAmount(slotItem.count);
            } else
            {
                // The stack contains more items than we want to remove
                slotItem.DecreaseAmount(count);
                slotItem.RefreshCount();
                return true;
            }
        }

        Assert.IsTrue(count == 0);
        return count <= 0;
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
        
        
        //Checking if there is an item on the upgrade slot
        if (m_upgradeslot && m_upgradeslot.transform.childCount >= 0)
        {
            if (m_upgradeslot.GetComponentInChildren<InventoryItem>())
            {
                InventoryData inventoryData = new InventoryData
                {
                    slotName = ReturnFirstEmptySpace(), // Save the slot name
                    item = m_upgradeslot.GetComponentInChildren<InventoryItem>().item.name, // Save the item name
                    amount = m_upgradeslot.GetComponentInChildren<InventoryItem>().ReturnAmount(),
                    multiplier = m_upgradeslot.GetComponentInChildren<InventoryItem>().ReturnMultiplier()
                };
                inventoryDataList.Add(inventoryData);
                AddItem(m_upgradeslot.GetComponentInChildren<InventoryItem>().item);
            }
        }
        
        //Checking if there is an item on the sell slot
        if (m_sellSlot && m_sellSlot.transform.childCount >= 0)
        {
            if (m_sellSlot.GetComponentInChildren<InventoryItem>())
            {
                InventoryData inventoryData = new InventoryData
                {
                    slotName = ReturnFirstEmptySpace(), // Save the slot name
                    item = m_sellSlot.GetComponentInChildren<InventoryItem>().item.name, // Save the item name
                    amount = m_sellSlot.GetComponentInChildren<InventoryItem>().ReturnAmount(),
                    multiplier = m_sellSlot.GetComponentInChildren<InventoryItem>().ReturnMultiplier()
                };
                inventoryDataList.Add(inventoryData);
                AddItem(m_sellSlot.GetComponentInChildren<InventoryItem>().item);
            }
        }
        

        // Wrap the list in a wrapper class
        InventoryWrapper wrapper = new InventoryWrapper { inventoryDataList = inventoryDataList };

        // Serialize the wrapper to JSON
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(m_saveManager.GetFilePath(m_saveLocation), json);
    }

    private void LoadInventoryFromFile()
    {
        if (File.Exists(m_saveManager.GetFilePath(m_saveLocation)))
        {
            // Read the JSON file
            string json = File.ReadAllText(m_saveManager.GetFilePath(m_saveLocation));

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
                            if (wrapper.inventoryDataList[i].item == items[k].name)
                            {
                                inventoryItem.InitializeItem(items[k]);
                                break;
                            }
                        }
                        inventoryItem.SetAmount(wrapper.inventoryDataList[i].amount);
                        inventoryItem.SetMultiplier(wrapper.inventoryDataList[i].multiplier);
                        inventoryItem.SetInventory(this);
                        break;
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("Inventory file not found at " + m_saveManager.GetFilePath(m_saveLocation));
        }
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

    /// <summary>
    /// Get number of slots in inventory
    /// </summary>
    /// <returns>The size</returns>
    public int GetSize()
    {
        return inventorySlots.Length;
    }

    private bool HasBoughtPistol()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].transform.childCount > 0)
            {
                if(inventorySlots[i].GetComponentInChildren<InventoryItem>().item.name == "ItemPistol")
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool HasBoughtShotgun()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {   
            if (inventorySlots[i].transform.childCount > 0)
            {
                if (inventorySlots[i].GetComponentInChildren<InventoryItem>().item.name == "Shotgun")
                {
                    return true;
                }
            }
        }
        return false;
    }

    private string ReturnFirstEmptySpace()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].transform.childCount <= 0)
            {
                return inventorySlots[i].gameObject.name;
            }
        }
        return null;
    }
}

[System.Serializable]
public class InventoryWrapper
{
    public List<InventoryData> inventoryDataList;
}
