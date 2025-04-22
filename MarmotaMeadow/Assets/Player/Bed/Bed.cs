using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField] private MeshRenderer[] m_highlightMeshes;
    [SerializeField] private Color m_highlightColor;
    [SerializeField] private Image m_blackImage;
    [SerializeField] private float m_fadeTime;
    private float m_secondsFaded = -1;

    private void Start()
    {
        foreach (var mesh in m_highlightMeshes)
        {
            mesh.material.color = m_highlightColor;
        }
    }

    private void Update()
    {
        if (!tutorialManager.ReturnIsTutorialFinished())
        {
            foreach (var mesh in m_highlightMeshes) mesh.gameObject.SetActive(false);
            return;
        }

        if (m_secondsFaded >= 0)
        {
            m_secondsFaded += Time.deltaTime;
        }
        var color = m_blackImage.color;
        color.a = Mathf.InverseLerp(0, m_fadeTime, m_secondsFaded);
        m_blackImage.color = color;
        m_blackImage.gameObject.SetActive(color.a > 0);
        if (color.a >= 1)
        {
            GameObject[] notPickedUpCrops = GameObject.FindGameObjectsWithTag("Crop");

            for (int i = 0; i < notPickedUpCrops.Length; i++)
            {
                inventoryMager.AddItem(notPickedUpCrops[i].GetComponent<SpawnedItem>().ReturnItem());
            }
            SceneManager.LoadScene("Shop");
            return;
        }

        m_text.text = "Go to sleep? (" + GameInput.GetKeybind("Interact") + ")";

        // Is within interaction distance?
        float distanceSquared = (m_player.transform.position - transform.position).sqrMagnitude;
        if (distanceSquared > Mathf.Pow(CameraScript.GetMaxInteractDistance(), 2))
        {
            m_UI.SetActive(false);
            m_confirmationUI.SetActive(false);
            foreach (var mesh in m_highlightMeshes) mesh.gameObject.SetActive(false);
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
                foreach (var mesh in m_highlightMeshes) mesh.gameObject.SetActive(false);
                m_confirmationUI.SetActive(false);
                return; // Hit something else
            }

            if (!m_clicked) m_UI.SetActive(true);
            foreach (var mesh in m_highlightMeshes) mesh.gameObject.SetActive(true);

            if (GameInput.GetKeybind("Interact").GetKeyDown() )
            {
                isInBed = true;
                m_confirmationUI.SetActive(true);
                m_cursorHandler.NotifyUiOpen();
                m_clicked = true;
                m_UI.SetActive(false);
            }
        }
    }

    public void Confirm()
    {
        if (m_secondsFaded < 0) { 
            m_secondsFaded = 0;
        }
    }

    public void NoButton()
    {
        if (m_secondsFaded >= 0) return;
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
