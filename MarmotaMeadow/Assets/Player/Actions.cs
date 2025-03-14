using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Actions : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private GameObject m_inventoryUI;
    [SerializeField] private bool m_inInventory;
    [SerializeField] private InventoryMager m_inventoryManager;
    
    [Header("Interactions")]
    [SerializeField] private GameObject m_camera;
    [SerializeField] private float m_maxDistance;
    [SerializeField] private LayerMask m_plantLayerMask;
    [SerializeField] private GameObject m_intereactedPlant;

    [Header("Scene")] 
    [SerializeField] bool isDay;

    private void Start()
    {
        isDay = SceneManager.GetActiveScene().name == "Day Scene";
    }

    private void Update()
    {        
        InteractWithPlot();
    }

    void InteractWithPlot()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit hit;
            Ray ray = new Ray(m_camera.transform.position, m_camera.transform.forward);
            
            if (Physics.Raycast(ray, out hit, m_maxDistance, m_plantLayerMask))
            {
                InventoryItem heldItem = m_inventoryManager.GetHeldInventoryItem();
                    
                if (heldItem.transform.childCount > 0) 
                {   
                    if (hit.collider.GetComponent<Plant>().ChangeState(heldItem) 
                        & heldItem.item.IsStackable())
                    { 
                        heldItem.DecreaseAmount();
                    }
                }
            }
        }
    } 
}
