using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI")]
    public Image image;
    public BaseItem item;
    public TextMeshProUGUI countText;
    public int count = 0;
    [SerializeField] private float multiplier;
    public Transform parentAfterDrag;
    [SerializeField] private float coins;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private bool isPickedUp;

    [Header("ToolTip")]
    [SerializeField] private GameObject toolTip;
    [SerializeField] private TextMeshProUGUI multiplierText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private int level;

    [Header("References")]
    [SerializeField] private InventoryMager inventory;
    [SerializeField] private GameObject toolTipLocation;
    [SerializeField] private TextMeshProUGUI[] toolTipText;

    // Start is called before the first frame update
    void Start()
    {
        toolTip = GameObject.FindGameObjectWithTag("ToolTip");
        toolTipText = toolTip.GetComponentsInChildren<TextMeshProUGUI>();
        RefreshCount();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitializeItem(BaseItem newItem)
    {
        item = newItem;
        image.sprite = newItem.ReturnImage();
        RefreshCount();
    }

    public void IsPickedUp()
    {
        isPickedUp = true;
    }

    public void IsNotPickedUp()
    {
        isPickedUp = false;
    }
    

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(isPickedUp) return;
        
        if (item is Tool)
        {
            toolTip.SetActive(true);
            toolTip.transform.position = toolTipLocation.transform.position;
            toolTipText[0].text = item.GetItemName() + "<br>" +"Lvl: " + multiplier;
        }
        else
        {
            toolTip.SetActive(true);
            toolTip.transform.position = toolTipLocation.transform.position;
            toolTipText[0].text = item.GetItemName();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.transform.position = new Vector3(0, 0, -100000);
    }

    public void RefreshCount()
    {
        countText.text = item.IsStackable() ? count.ToString() : "";
    }

    public float ReturnMultiplier()
    {
        return multiplier;
    }

    public int ReturnAmount()
    {
        return count;
    }

    public void IncreaseMultiplier(int amount = 1)
    {
        multiplier+=amount;
    }

    public void SetMultiplier(float mult)
    {
        this.multiplier = mult;
    }

    public void SetAmount(int amount)
    {
        count = amount; 
        if (count <= 0)
            Destroy(gameObject, 0.01f);
        else
            countText.text = count.ToString();
    }

    public void IncreaseAmount(int amount = 1)
    {
        count += amount;
        countText.text = count.ToString();
    }

    public void DecreaseAmount(int amount = 1)
    {
        count -= amount;
        if (count <= 0)
            Destroy(gameObject, 0.01f);
        else
            countText.text = count.ToString();
    }

    public void SetInventory(InventoryMager inventory)
    {
        this.inventory = inventory;
    }

    public void SetImageRaycast(bool raycast)
    {
        image.raycastTarget = raycast;
    }

    public bool IsThisType(BaseItem type)
    {
        return item != null && item.name == type.name;
    }
}
