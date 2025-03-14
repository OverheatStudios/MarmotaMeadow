using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                InventoryItem heldItem = inventoryManager.GetHeldInventoryItem();
                RaycastHit hit;
                Ray ray = new Ray(cameraObject.transform.position, cameraObject.transform.forward);
        
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
