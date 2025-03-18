using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public Image image;
    public Color32 normalColor;
    public Color32 selectedColor;
    [SerializeField] InventoryMager inventory;
    [SerializeField] private Sprite normalInventoryItem;
    [SerializeField] private Sprite selectedInventoryItem;
    [SerializeField] private TextMeshProUGUI m_heldItemText;
    private static InventorySlot m_selectedSlot;

    void Start()
    {
    }

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
        else if (transform.childCount >= 1 && inventory.ReturnInventoryItem())
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

        if (m_selectedSlot != null)
        {
            m_selectedSlot.Select();
        }
    }

    public void Select()
    {
        m_selectedSlot = this;
        image.sprite = selectedInventoryItem;
        m_heldItemText.rectTransform.position = transform.position + Vector3.up * 250.0f + 4 * image.rectTransform.sizeDelta.x * Vector3.left;
        InventoryItem item = GetComponentInChildren<InventoryItem>();
        m_heldItemText.text = item == null ? "" : item.item.GetItemName();
    }

    public void Deselect()
    {
        if (m_selectedSlot == this) m_selectedSlot = null;

        image.sprite = normalInventoryItem;
    }

    public void ShowItemName()
    {
        m_heldItemText.gameObject.SetActive(true);
    }
}