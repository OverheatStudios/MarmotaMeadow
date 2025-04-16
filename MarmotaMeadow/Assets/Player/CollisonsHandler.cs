using System;
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
    [SerializeField] private AudioClip m_pickupItemSfx;
    [SerializeField] private Collider m_playerCollider;
    [SerializeField] private SettingsScriptableObject m_settings;
    [Tooltip("Any collider below player y + this value is considered floor")]
    [SerializeField] private float m_floorLeniency = 0.1f;
    private FloorType m_lastCollidedFloorType = FloorType.None;
    private int m_lastGroundedFrame = -1;

    // Update is called once per frame
    void Update()
    {

    }

    public FloorType GetLastCollidedFloorType()
    {
        return m_lastCollidedFloorType;
    }

    private List<ContactPoint> m_contacts = new();
    private void OnCollisionEnter(Collision other)
    {
        if (!isActiveAndEnabled) return;

        // Get floor type
        FloorTypeScript floorTypeScript;
        other.gameObject.TryGetComponent<FloorTypeScript>(out floorTypeScript);

        m_contacts.Clear();
        other.GetContacts(m_contacts);
        if (floorTypeScript)
        {
            foreach (ContactPoint contactPoint in m_contacts)
            {
                if (contactPoint.point.y - m_floorLeniency > m_playerCollider.bounds.min.y) continue;
                m_lastCollidedFloorType = floorTypeScript.GetFloorType();
                break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActiveAndEnabled) return;

        if (other.gameObject.CompareTag("Crop"))
        {
            AudioSource.PlayClipAtPoint(m_pickupItemSfx, other.transform.position, m_settings.GetSettings().GetGameVolume());
            m_inventory.AddItem(other.gameObject.GetComponent<SpawnedItem>().ReturnItem());
            m_ObjectPooling.PutObjectBack("Crop", other.gameObject);
            //Destroy(other.gameObject);
            OnPlayerCollision?.Invoke();
        }
        else if (other.gameObject.CompareTag("Fertilizer"))
        {
            m_inventory.AddItem(other.gameObject.GetComponent<SpawnedItem>().ReturnItem());
            Destroy(other.gameObject);
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (!isActiveAndEnabled || m_lastGroundedFrame == Time.frameCount) return;

        if (true)
        {
            m_contacts.Clear();
            other.GetContacts(m_contacts);
            foreach (ContactPoint contactPoint in m_contacts)
            {
                if (contactPoint.point.y - m_floorLeniency > m_playerCollider.bounds.min.y) continue;
                m_lastGroundedFrame = Time.frameCount;
                return;
            }
        }
    }

    public bool IsProbablyGrounded()
    {
        return m_lastGroundedFrame + 10 >= Time.frameCount;
    }
}
