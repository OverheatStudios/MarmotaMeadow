using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugItemSpawner : MonoBehaviour
{
    [SerializeField] private InventoryMager m_inventoryManager;
    [SerializeField] private BaseItem m_shotgun;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            m_inventoryManager.AddItem(m_shotgun);
        }
    }

}
