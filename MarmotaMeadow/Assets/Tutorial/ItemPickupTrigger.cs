using System.Collections;
using UnityEngine;

public class ItemPickupTrigger : TriggerBase
{
    [Header("Detection Settings")]
    public string itemTag = "Crop";
    public string playerTag = "Player";

    [SerializeField] private CollisonsHandler playerCollision;

    public override void ActivateTrigger()
    {
        playerCollision.OnPlayerCollision += CompleteTrigger;
    }
}