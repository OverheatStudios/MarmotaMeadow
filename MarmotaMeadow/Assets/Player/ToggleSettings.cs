using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleSettings : MonoBehaviour
{
    [SerializeField] private ScrObjGlobalData m_globalData;
    [SerializeField] private GameObject settings;
    [SerializeField] private GameObject mainSettings;
    [SerializeField] private GameObject[] m_settingsSubcategories;
    [SerializeField] private GameObject countDownUI;
    [SerializeField] private bool toggle;
    [SerializeField] private CursorHandlerScript m_cursorHandler;
    [SerializeField] private GameObject[] plants;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject m_mainInventoryUi;
    [SerializeField] private GameObject m_bedConfirmUi;
    [SerializeField] private GameObject[] m_disableWhenInSettings;
    [SerializeField] private InventoryActions m_inventoryActions;

    // Update is called once per frame
    void Update()
    {
        Toggler();
    }

    void Start()
    {
        Invoke(nameof(FindPlants), 0.5f);
    }

    void Toggler()
    {
        if (m_inventoryActions.IsInventoryOpen()) return;

        if (GameInput.GetKeybind("Pause").GetKeyDown() && !toggle)
        {
            PlantToggler(toggle);
            toggle = true;
            m_globalData.m_isSettingsOpen = true;
            settings.SetActive(true);
            mainSettings.SetActive(true);
            if (m_bedConfirmUi) m_bedConfirmUi.SetActive(false);
            m_mainInventoryUi.SetActive(false);
            foreach (GameObject go in m_settingsSubcategories)
            {
                go.SetActive(false);
            }
            foreach (GameObject go in m_disableWhenInSettings)
            {
                go.SetActive(false);
            }
            m_cursorHandler.NotifyUiOpen();
        }
        else if (GameInput.GetKeybind("Pause").GetKeyDown() && toggle)
        {
            PlantToggler(toggle);
            toggle = false;
            m_globalData.m_isSettingsOpen = false;
            settings.SetActive(false);
            mainSettings.SetActive(true);
            foreach (GameObject go in m_disableWhenInSettings)
            {
                go.SetActive(true);
            }
            m_cursorHandler.NotifyUiClosed();
        }
    }

    public void Back()
    {
        PlantToggler(toggle);
        toggle = false;
        settings.SetActive(false);
        mainSettings.SetActive(true);
        m_cursorHandler.NotifyUiClosed();
    }

    void PlantToggler(bool toggle)
    {
        for (int i = 0; i < plants.Length; i++)
        {
            plants[i].SetActive(toggle);
        }
        countDownUI.SetActive(toggle);
    }

    void FindPlants()
    {
        plants = GameObject.FindGameObjectsWithTag("Plant");
    }

    public bool IsToggled()
    {
        return toggle;
    }
}
