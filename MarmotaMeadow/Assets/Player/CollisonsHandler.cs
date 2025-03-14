using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisonsHandler : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Inventory")]
    [SerializeField] private InventoryMager m_inventory;
    public System.Action OnPlayerCollision; 
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision other)
    {
        if (!isActiveAndEnabled) return;
        if (other.gameObject.CompareTag("Crop"))
        {
            m_inventory.AddItem(other.gameObject.GetComponent<SpawnedItem>().ReturnItem());
            Destroy(other.gameObject);
            OnPlayerCollision?.Invoke();
        }
    }
}
