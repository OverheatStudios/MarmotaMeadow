using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellButtonTrigger : TriggerBase
{
    [SerializeField] private bool active = false;
    [SerializeField] private GameObject Slot;
    [SerializeField] private BaseItem item;
    public override void ActivateTrigger()
    {
        active = true;
    }


    public void ButtonClick()
    {
        if (active)
        {
            if (Slot.transform.childCount > 0 && Slot.transform.GetChild(0).GetComponent<InventoryItem>().item == item)
            {
                CompleteTrigger();
                active = false;
            }
        }
    }
}
