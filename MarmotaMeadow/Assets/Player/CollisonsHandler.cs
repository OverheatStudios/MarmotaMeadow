using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisonsHandler : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Inventory")]
    [SerializeField] private InventoryMager m_inventory;
    public System.Action OnPlayerCollision; 
    [SerializeField] private ObjectPooling m_ObjectPooling;
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
            m_ObjectPooling.PutObjectBack("Crop", other.gameObject);
            //Destroy(other.gameObject);
            OnPlayerCollision?.Invoke();
        }else if (other.gameObject.CompareTag("Fertilizer"))
        {
            m_inventory.AddItem(other.gameObject.GetComponent<SpawnedItem>().ReturnItem());
            Destroy(other.gameObject);
        }
    }
}
