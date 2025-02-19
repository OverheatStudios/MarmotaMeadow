using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles showing the item model in the hand (child of camera)
/// </summary>
public class FirstPersonHeldItemScript : MonoBehaviour
{
    [SerializeField] private InventoryMager m_inventoryManager;
    private GameObject m_currentUsedPrefab;
    private GameObject m_heldItemModel;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        BaseItem heldItem = m_inventoryManager.GetHeldItem();
        SetHeldItemModel(heldItem == null ? null : heldItem.GetHandModel());

    }

    private void SetHeldItemModel(GameObject model)
    {
        if (model == null)
        {
            RemoveHeldItemModel(); 
            return;
        }

        if (m_currentUsedPrefab == null || m_currentUsedPrefab.GetInstanceID() != model.GetInstanceID())
        {
            RemoveHeldItemModel();
            m_heldItemModel = Instantiate(model);
            m_heldItemModel.transform.SetParent(transform, false);
            m_currentUsedPrefab = model;
        }
    }

    private void RemoveHeldItemModel()
    {
        if (m_heldItemModel == null) return;
        Destroy(m_heldItemModel);
        m_heldItemModel = null;
        m_currentUsedPrefab = null;
    }
}
