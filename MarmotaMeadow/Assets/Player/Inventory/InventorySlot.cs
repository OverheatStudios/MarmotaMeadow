using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour//, IPointerClickHandler
{
    public Image image;
    public Color32 normalColor;
    public Color32 selectedColor;
    [SerializeField] InventoryMager inventory;
    [SerializeField] private Sprite normalInventoryItem;
    [SerializeField] private Sprite selectedInventoryItem;
    [SerializeField] private TextMeshProUGUI m_heldItemText;
    [SerializeField] private CursorHandlerScript m_cursorHandlerScript;
    private static InventorySlot m_selectedSlot;

    void Start()
    {
    }

    private void Update()
    {
        if (!m_cursorHandlerScript.GetVirtualMouse().IsLMBDown()) return;
        if (!RectTransformUtility.RectangleContainsScreenPoint(image.rectTransform, Input.mousePosition)) return;

        OnPointerClick();
    }

    // public void OnPointerClick(PointerEventData eventData)
    private void OnPointerClick()
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
        InventoryItem item = GetComponentInChildren<InventoryItem>();
        m_heldItemText.text = item == null ? "" : item.item.GetItemName();

        m_heldItemText.rectTransform.sizeDelta = new Vector2(m_heldItemText.GetPreferredValues().x, m_heldItemText.rectTransform.sizeDelta.y);
        m_heldItemText.rectTransform.position = transform.position + 2 * image.rectTransform.sizeDelta.y * Vector3.up + 2 * image.rectTransform.sizeDelta.x * Vector3.left;
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