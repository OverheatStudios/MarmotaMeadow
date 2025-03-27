using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadGroundHog : MonoBehaviour
{
    [SerializeField] private GameObject fertilizerPrefab;
    [SerializeField] private BaseItem fertilizerItem;
    
    private void OnMouseDown()
    {
        GameObject fertilizer = Instantiate(fertilizerPrefab, transform.position, Quaternion.identity);
        fertilizer.GetComponent<SpawnedItem>().SetItem(fertilizerItem);
        
        Destroy(gameObject);
    }
}
