using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Actions : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private List<InventorySlot> slots = new List<InventorySlot>(); // List of inventory slots
    [SerializeField] private int m_selectedItemIndex;
    [SerializeField] private GameObject m_inventoryUI;
    [SerializeField] private bool m_inInventory;
    
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
        

        if (Input.GetKeyDown(KeyCode.Space))
        {
            IncreaseMultiplierTest();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Yes");
            SceneManager.LoadScene("NightScene");
        }
    }

    void InteractWithPlot()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit hit;
            Ray ray = new Ray(m_camera.transform.position, m_camera.transform.forward);
            
            if (Physics.Raycast(ray, out hit, m_maxDistance, m_plantLayerMask))
            {
                if (hit.collider.CompareTag("Plant"))
                {
                    if (slots[m_selectedItemIndex].transform.childCount > 0) 
                    {   
                        if (hit.collider.GetComponent<Plant>().ChangeState(slots[m_selectedItemIndex].GetComponentInChildren<InventoryItem>()) 
                            & slots[m_selectedItemIndex].GetComponentInChildren<InventoryItem>().item.IsStackable())
                        {
                            slots[m_selectedItemIndex].GetComponentInChildren<InventoryItem>().count--;
                            if (slots[m_selectedItemIndex].GetComponentInChildren<InventoryItem>().count <= 0)
                            {
                                Destroy(slots[m_selectedItemIndex].transform.GetChild(0).gameObject);
                            }
                        }
                    }
                }
            }
        }
    }


    void IncreaseMultiplierTest()
    {
        slots[m_selectedItemIndex].GetComponentInChildren<InventoryItem>().IncreaseMultiplier();
    }
}
