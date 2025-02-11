using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI")]
    public Image image;
    public BaseItem item;
    public TextMeshProUGUI countText;
    public int count = 0;
    [SerializeField] private float multiplier;
    public Transform parentAfterDrag;
    
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

    public void RefreshCount()
    {
        countText.text = count.ToString();
    }

    public void OnBeginDrag(PointerEventData eventData)
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
    }

    public float ReturnMultiplier()
    {
        return multiplier;
    }

    public void IncreaseMultiplier()
    {
        multiplier++;
    }
}
