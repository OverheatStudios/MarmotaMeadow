using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] ChangeToNight m_changeToNight;

    [Header("Interact with not harvestable")]
    [Tooltip("Things on this layer go red when you attempt to harvest or hoe them")]
    [SerializeField] private LayerMask m_notHarvestableLayer;
    [Tooltip("In seconds how long should they be read for")]
    [SerializeField] private float m_redDuration = 1.5f;
    [Tooltip("How long should they take to fade back to original colour")]
    [SerializeField] private float m_redFadeDuration = 0.8f;
    [Tooltip("When at 100% \"redness\", original colour is multiplied by this")] 
    [SerializeField] private Color m_redColor = new Color(0.5f, 0, 0);
    [Tooltip("How many times should we change the colour when fading? 100 means we modify the colour in 1/100*abs(originalColour*redColor-originalColour) decrements/increments")]
    [SerializeField] private int m_numColorChanges = 100;
    [Tooltip("When a not harvestable is red, we'll put it on this layer temporarily")]
    [SerializeField] private int m_defaultLayer;

    private void Start()
    {
    }

    private void Update()
    {
        InteractWithPlot();
    }

    void InteractWithPlot()
    {
        if (GameInput.GetKeybind("Interact").GetKeyDown() )
        {

            InventoryItem heldItem = m_inventoryManager.GetHeldInventoryItem();

            if (heldItem == null) return;
            if (m_changeToNight.ReturnIsNight() && m_inventoryManager.GetHeldInventoryItem().item.name == "hoe")
            {
                return;
            }

            RaycastHit hit;
            Ray ray = new Ray(m_camera.transform.position, m_camera.transform.forward);

            LayerMask checkMask = m_plantLayerMask;
            bool farmToolHeld = m_inventoryManager.GetHeldInventoryItem().item.name == "harvesting tool" || m_inventoryManager.GetHeldInventoryItem().item.name == "hoe";
            if (farmToolHeld) checkMask |= m_notHarvestableLayer;

            if (Physics.Raycast(ray, out hit, m_maxDistance, checkMask, QueryTriggerInteraction.Collide))
            {
                if ((m_notHarvestableLayer & (1 << hit.collider.gameObject.layer)) != 0)
                {
                    if (!farmToolHeld) return;
                    GameObject obj = hit.collider.gameObject;
                    StartCoroutine(ShowRedOverlay(obj));
                    return;
                }

                if (heldItem.transform.childCount > 0)
                {
                    if (hit.collider.GetComponent<Plant>().ChangeState(heldItem)
                        && heldItem.item.IsStackable())
                    {
                        heldItem.DecreaseAmount();
                    }
                }
            }
        }
    }

    IEnumerator ShowRedOverlay(GameObject obj)
    {
        var meshes = obj.GetComponentsInChildren<MeshRenderer>();
        if (meshes.Length > 0)
        {

            // Set colour
            obj.layer = m_defaultLayer;
            Coroutine[] coroutines = new Coroutine[meshes.Length];
            for (int i = 0; i < meshes.Length; i++)
            {
                coroutines[i] = StartCoroutine(ShowRedOverlayForMesh(meshes[i]));
            }

            // Wait for colour to go back to original
            foreach (var coroutine in coroutines)
            {
                yield return coroutine;
            }

            // Reset the layer
            for (int whatLayer = 0; whatLayer < 32; ++whatLayer)
            {
                if ((m_notHarvestableLayer & (1 << whatLayer)) == 0) continue;
                obj.layer = whatLayer;
                break;
            }
        }
    }

    IEnumerator ShowRedOverlayForMesh(MeshRenderer mesh)
    {
        Color originalColor = mesh.material.color;
        mesh.material.color *= m_redColor;
        float dt = 1.0f / (float)m_numColorChanges;
        float delay = m_redFadeDuration / (float)m_numColorChanges;
        Color modifiedColor = mesh.material.color;
        yield return new WaitForSeconds(m_redDuration);

        for (float t = 0; t < 1; t += dt)
        {
            mesh.material.color = Color.Lerp(modifiedColor, originalColor, t);
            yield return new WaitForSeconds(delay);
        }
        mesh.material.color = originalColor;
    }
}