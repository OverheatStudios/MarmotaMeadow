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
    [SerializeField] private Vector3 m_minecraftHeldItemPos = new Vector3(0.6f, -0.3f, 1);
    [SerializeField] private Vector3 m_minecraftHeldItemRot = new Vector3(180f, 270f, 180f);
    [SerializeField] private Vector3 m_minecraftHeldItemScale = new Vector3(0.9f, 0.9f, 0.8f);

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        BaseItem heldItem = m_inventoryManager.GetHeldItem();
        if (heldItem != null)
            SetHeldItemModel(heldItem.GetHandModel(), heldItem.IsMinecraftModel());
        else
            SetHeldItemModel(null, false);
    }

    private void SetHeldItemModel(GameObject model, bool useMinecraftTransform)
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
            if (useMinecraftTransform)
            {
                m_heldItemModel.transform.localEulerAngles += m_minecraftHeldItemRot;//= Quaternion.Euler(m_minecraftHeldItemRot);
                m_heldItemModel.transform.localPosition = m_minecraftHeldItemPos;
                m_heldItemModel.transform.localScale = new Vector3(
                    m_minecraftHeldItemScale.x * m_heldItemModel.transform.localScale.x,
                    m_minecraftHeldItemScale.y * m_heldItemModel.transform.localScale.y,
                    m_minecraftHeldItemScale.z * m_heldItemModel.transform.localScale.z
                    );
            }

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
