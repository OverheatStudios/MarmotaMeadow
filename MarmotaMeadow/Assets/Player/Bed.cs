using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bed : MonoBehaviour
{
    [SerializeField] private GameObject m_UI;
    [SerializeField] private GameObject m_confirmationUI;
    [SerializeField] private CursorHandlerScript m_cursorHandler;
    [SerializeField] private bool m_clicked = false;
    [SerializeField] private TutorialManager tutorialManager;
    [SerializeField] private bool isInBed;
    [SerializeField] private TextMeshProUGUI m_text;
    [SerializeField] private InventoryMager inventoryMager;
    [SerializeField] private GameObject m_player;
    [SerializeField] private LayerMask m_bedLayer;
    [SerializeField] private Camera m_camera;
    private void Update()
    {
        m_text.text = "Go to sleep? (" + GameInput.GetKeybind("Interact") + ")";

        // Is within interaction distance?
        float distanceSquared = (m_player.transform.position - transform.position).sqrMagnitude;
        if (distanceSquared > Mathf.Pow(CameraScript.GetMaxInteractDistance(), 2))
        {
            m_UI.SetActive(false);
            m_confirmationUI.SetActive(false);
            return;
        }

        // Anything in way?
        Ray ray = new Ray(m_camera.transform.position, m_camera.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, CameraScript.GetMaxInteractDistance(), m_bedLayer))
        {
            if (hit.collider.gameObject != gameObject)
            {
                m_UI.SetActive(false);
                return; // Hit something else
            }
        }

            if (!m_clicked)
            m_UI.SetActive(true);

        if (GameInput.GetKeybind("Interact").GetKeyDown() && tutorialManager.ReturnIsTutorialFinished())
        {
            isInBed = true;
            m_confirmationUI.SetActive(true);
            m_cursorHandler.NotifyUiOpen();
            m_clicked = true;
            m_UI.SetActive(false);
        }
    }

    public void Confirm()
    {
        GameObject[] notPikedUpCrops = GameObject.FindGameObjectsWithTag("Crop");

        for (int i = 0; i < notPikedUpCrops.Length; i++)
        {
            inventoryMager.AddItem(notPikedUpCrops[i].GetComponent<SpawnedItem>().ReturnItem());
        }
        SceneManager.LoadScene("Shop");
    }

    public void NoButton()
    {
        isInBed = false;
        m_confirmationUI.SetActive(false);
        m_cursorHandler.NotifyUiClosed();
        m_clicked = false;
    }

    public bool ReturnIsInBed()
    {
        return isInBed;
    }
}
