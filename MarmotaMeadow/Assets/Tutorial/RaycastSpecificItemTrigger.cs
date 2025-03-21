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
    
    
    public override void ActivateTrigger()
    {
        StartCoroutine(WaitForPlayerToLookAndClick());
    }

    private IEnumerator WaitForPlayerToLookAndClick()
    {
        while (true)
        {
            if (GameInput.GetKeybind("Interact").GetKeyDown())
            {
                InventoryItem heldItem = inventoryManager.GetHeldInventoryItem();
                RaycastHit hit;
                Ray ray = new(cameraObject.transform.position, cameraObject.transform.forward);
        
                if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
                {
            
                    if(heldItem.item != specificItem)
                        yield return null;
                    else
                        CompleteTrigger();
                }
            }
            yield return null;
        }
    }
}
