using System.Collections;
using UnityEngine;

public class ItemPickupTrigger : TriggerBase
{
    [Header("Detection Settings")]
    public string itemTag = "Crop";
    public string playerTag = "Player";

    [SerializeField] bool isActive = true;
    [SerializeField] private CollisonsHandler playerCollision;

    public override void ActivateTrigger()
    {
        isActive = true;
        playerCollision.OnPlayerCollision += CompleteTrigger;
    }
}