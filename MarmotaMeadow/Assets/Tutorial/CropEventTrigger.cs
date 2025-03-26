using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropEventTrigger : TriggerBase
{
    [SerializeField] private Plant plant;
    [SerializeField] private int eventChoice;
    
    public override void ActivateTrigger()
    {
        if (!plant) return;

        switch (eventChoice)
        {
            case 1:
                plant.OnTealed += CompleteTrigger;
                break;
            case 2:
                plant.OnPlanted += CompleteTrigger;
                break;
            case 3:
                plant.OnWatered += CompleteTrigger;
                break;
            case 4:
                plant.OnHarvested += CompleteTrigger;
                break;
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        if (!plant) return;

        plant.OnTealed -= CompleteTrigger;
        plant.OnPlanted -= CompleteTrigger;
        plant.OnWatered -= CompleteTrigger;
        plant.OnHarvested -= CompleteTrigger;
    }
}
