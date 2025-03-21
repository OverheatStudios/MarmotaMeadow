using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RaycastSpecificItemTrigger : TriggerBase
{
    [SerializeField] private GameObject cameraObject;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private InventoryMager inventoryManager;
    [SerializeField] private float maxDistance;
    [SerializeField] private BaseItem specificItem;
    [SerializeField] private bool yes = false;
    
    
    public override void ActivateTrigger()
    {
        yes = true;
    }


    private void Update()
    {
        if (GameInput.GetKeybind("Interact").GetKeyDown() && yes)
        {
            InventoryItem heldItem = inventoryManager.GetHeldInventoryItem();
            RaycastHit hit;
            Ray ray = new(cameraObject.transform.position, cameraObject.transform.forward);

            if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
            {
                if (heldItem.item == specificItem)
                {
                    CompleteTrigger();
                    yes = false;
                }
            }
        }
    }
}
