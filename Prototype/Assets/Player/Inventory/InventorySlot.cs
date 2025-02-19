using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public Image image;
    public Color32 normalColor;
    public Color32 selectedColor;
    [SerializeField] InventoryMager inventory;

    public void OnPointerClick(PointerEventData eventData)
    {
        
        if (transform.childCount == 0)
        {
            inventory.ReturnInventoryItem().transform.SetParent(transform);
            inventory.SetInventoryItem();
        }
        else if(transform.childCount >= 1 && inventory.ReturnInventoryItem())
        {
            inventory.ReturnInventoryItem().transform.SetParent(transform);
            inventory.SetInventoryItem(transform.GetChild(0).gameObject);
            inventory.ReturnInventoryItem().transform.SetParent(transform.root);
        }else if (transform.childCount >= 1 && !inventory.ReturnInventoryItem())
        {
            inventory.SetInventoryItem(transform.GetChild(0).gameObject);
            inventory.ReturnInventoryItem().transform.SetParent(transform.root);
        }
    }
    
    public void Select()
    {
        image.color = Color.blue;
    }

    public void Deselect()
    {
        image.color = Color.white;
    }
}