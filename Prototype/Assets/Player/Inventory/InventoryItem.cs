using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour,IPointerEnterHandler , IPointerExitHandler
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

    [Header("ToolTip")]
    [SerializeField] private GameObject toolTip;
    [SerializeField] private TextMeshProUGUI multiplierText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private int level;

    [Header("References")] 
    [SerializeField] private InventoryMager inventory;
    
    // Start is called before the first frame update
    void Start()
    {
        
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
    
    /*public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;
        transform.SetParent(parentAfterDrag);
    }*/

    public void OnPointerEnter(PointerEventData eventData)
    {
        toolTip.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.SetActive(false);
    }
    
    public void RefreshCount()
    {
        countText.text = count.ToString();
    }

    public float ReturnMultiplier()
    {
        return multiplier;
    }

    public int ReturnAmount()
    {
        return count;
    }

    public void IncreaseMultiplier()
    {
        multiplier++;
        multiplierText.text = "Multiplier: " + multiplier.ToString();
    }

    public void SetMultiplier(float mult)
    {
        this.multiplier = mult;
    }

    public void SetAmount(int amount)
    {
        count = amount;
        countText.text = count.ToString();
    }

    public void IncreaseAmount()
    {
        count++;
        countText.text = count.ToString();
    }
    
    public void DecreaseAmount()
    {
        count--;
        if (count <= 0)
            Destroy(gameObject);
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
}
