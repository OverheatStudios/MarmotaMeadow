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
        //putting an item on an empty slot
        if (transform.childCount == 0)
        {
            inventory.ReturnInventoryItem().transform.SetParent(transform);
            inventory.ReturnInventoryItem().GetComponent<InventoryItem>().SetImageRaycast(true);
            inventory.SetInventoryItem();
        }
        //trading places with another item
        else if(transform.childCount >= 1 && inventory.ReturnInventoryItem())
        {
            inventory.ReturnInventoryItem().transform.SetParent(transform);
            inventory.ReturnInventoryItem().GetComponent<InventoryItem>().SetImageRaycast(true);
            inventory.SetInventoryItem(transform.GetChild(0).gameObject);
            inventory.ReturnInventoryItem().transform.SetParent(transform.root);
            inventory.ReturnInventoryItem().GetComponent<InventoryItem>().SetImageRaycast(false);
        }
        //taking an item away from a slot
        else if (transform.childCount >= 1 && !inventory.ReturnInventoryItem())
        {
            inventory.SetInventoryItem(transform.GetChild(0).gameObject);
            inventory.ReturnInventoryItem().transform.SetParent(transform.root);
            inventory.ReturnInventoryItem().GetComponent<InventoryItem>().SetImageRaycast(false);
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