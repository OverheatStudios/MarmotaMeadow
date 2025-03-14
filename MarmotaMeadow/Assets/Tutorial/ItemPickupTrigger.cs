using System.Collections;
using UnityEngine;

public class ItemPickupTrigger : TriggerBase
{
    [Header("Detection Settings")]
    public string itemTag = "Pickup";
    public string playerTag = "Player";

    [SerializeField] bool isActive = true;
    [SerializeField] private GameObject[] pickups;

    public override void ActivateTrigger()
    {
        isActive = true;
        StartCoroutine(CheckIfPlayerPickUpItem());
    }

    private IEnumerator CheckIfPlayerPickUpItem()
    {
        while (isActive)
        {
            pickups = GameObject.FindGameObjectsWithTag(itemTag);

            for (int i = 0; i < pickups.Length; i++)
            {
                Collider itemCollider = pickups[i].GetComponent<Collider>();
                if (itemCollider != null)
                {
                    CheckPlayerCollision(itemCollider);
                }
            }
            yield return null;
        }

        yield return null;
    }

    private bool CheckPlayerCollision(Collider itemCollider)
    {
        Collider[] hitColliders = Physics.OverlapBox(
            itemCollider.bounds.center,
            itemCollider.bounds.extents / 2f
        );
        

        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].CompareTag(playerTag))
            {
                Debug.Log("Player collision");
                CompleteTrigger();
                return true;
            }
        }

        return false;
    }
}