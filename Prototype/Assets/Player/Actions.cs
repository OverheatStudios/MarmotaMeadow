using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
<<<<<<< HEAD
=======
using UnityEngine.SceneManagement;
using UnityEngine.UI;
>>>>>>> origin/plant

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
        Debug.Log(isDay);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            slots[m_selectedItemIndex].GetComponent<InventorySlot>().Deselect();
            Debug.Log(slots[m_selectedItemIndex].name);
            m_selectedItemIndex++;
            if (m_selectedItemIndex >= 9)
            {
                m_selectedItemIndex = 0;
                slots[m_selectedItemIndex].GetComponent<InventorySlot>().Select();
            }
            else
            {
                slots[m_selectedItemIndex].GetComponent<InventorySlot>().Select();
            }
        }else if (Input.GetKeyDown(KeyCode.Q))
        {
            slots[m_selectedItemIndex].GetComponent<InventorySlot>().Deselect();
            m_selectedItemIndex--;
            if (m_selectedItemIndex <= -1)
            {
                m_selectedItemIndex = 8;
                slots[m_selectedItemIndex].GetComponent<InventorySlot>().Select();
            }
            else
            {
                slots[m_selectedItemIndex].GetComponent<InventorySlot>().Select();
            }
        }
        InteractWithPlot();
        
        
        ToggleInventory();

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

    void ToggleInventory()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !m_inInventory)
        {
            //UI
            m_inInventory = true;
            m_inventoryUI.SetActive(true);
            
            //Cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }else if (Input.GetKeyDown(KeyCode.Escape) && m_inInventory)
        {
            //UI
            m_inInventory = false;
            m_inventoryUI.SetActive(false);
            
            //Cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void IncreaseMultiplierTest()
    {
        slots[m_selectedItemIndex].GetComponentInChildren<InventoryItem>().IncreaseMultiplier();
    }
}
