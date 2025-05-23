using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class InventoryActions : MonoBehaviour
{
    /// <summary>
    /// If false, player cannot open inventory and can only use tool bar
    /// </summary>
    [SerializeField] private bool m_canOpenInventory = true;

    [Header("Inventory")]
    [SerializeField] private List<InventorySlot> slots = new List<InventorySlot>(); // List of inventory slots
    [SerializeField] private int m_selectedItemIndex;
    [SerializeField] private GameObject m_inventoryUI;
    [SerializeField] private bool m_inInventory;
    [SerializeField] private InventoryMager m_inventoryManager;
    [SerializeField] private CursorHandlerScript m_cursorHandler;
    [SerializeField] private ToggleSettings m_toggleSettings;
    [SerializeField] private bool m_canSelectSlots = true;
    [SerializeField] private GameObject m_toolTip;
    [SerializeField] private RectTransform m_inventoryBackground;
    [SerializeField] private float m_timeBeforeClickingOffClosesInventory = 1.0f;
    [SerializeField] private RectTransform m_toolbarBackground;
    private float m_lateStartCalled = 0;
    private float m_secondsSinceInventoryOpen = 0;

    private void Start()
    {
        Assert.IsTrue(m_timeBeforeClickingOffClosesInventory > 0);  
        if (m_canSelectSlots)
        {
            SelectSlot(m_selectedItemIndex);
        }
        else
        {
            foreach (var slot in slots)
            {
                slot.Deselect();
            }
        }
    }

    private void LateStart()
    {
        SelectSlot(m_selectedItemIndex);
        slots[0].ShowItemName();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsInventoryOpen())
            m_secondsSinceInventoryOpen += Time.deltaTime;
        else
            m_secondsSinceInventoryOpen = 0;

        m_lateStartCalled++;
        if (m_lateStartCalled == 2)
        {
            LateStart();
            return;
        }

        if (m_canSelectSlots) HandleSlotChange();

        bool tryingToCloseInventory = GameInput.GetKeybind("Pause").GetKeyDown() && IsInventoryOpen();
        bool clickedOffInventory = m_secondsSinceInventoryOpen > m_timeBeforeClickingOffClosesInventory && 
            m_cursorHandler.GetVirtualMouse().IsLMBDown() && 
            !RectTransformUtility.RectangleContainsScreenPoint(m_toolbarBackground, Input.mousePosition) &&
            !RectTransformUtility.RectangleContainsScreenPoint(m_inventoryBackground, Input.mousePosition);
        if (GameInput.GetKeybind("OpenInventory").GetKeyDown() || tryingToCloseInventory || clickedOffInventory)
        {
            ToggleInventory();
        }
    }

    void HandleSlotChange()
    {
        bool isR1Pressed = Gamepad.current != null && Gamepad.current.rightShoulder.wasPressedThisFrame;
        bool isL1Pressed = Gamepad.current != null && Gamepad.current.leftShoulder.wasPressedThisFrame;
        if (Input.GetAxis("Mouse ScrollWheel") < 0f || isR1Pressed)
        {
            int slot = m_selectedItemIndex + 1;
            if (slot >= 9)
            {
                slot = 0;
            }
            SelectSlot(slot);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0f || isL1Pressed)
        {
            int slot = m_selectedItemIndex - 1;
            if (slot <= -1)
            {
                slot = 8;
            }
            SelectSlot(slot);
        }
        else
        {
            for (int i = 0; i < 9; ++i)
            {
                if (Input.GetKeyDown("" + (i + 1)))
                {
                    SelectSlot(i);
                    return;
                }
            }
        }
    }

    private void SelectSlot(int slot)
    {
        slots[m_selectedItemIndex].GetComponent<InventorySlot>().Deselect();
        m_selectedItemIndex = slot;
        m_inventoryManager.NotifyNewSelectedItemIndex(slot);
        slots[m_selectedItemIndex].GetComponent<InventorySlot>().Select();
    }

    public void ToggleInventory()
    {
        if (!m_canOpenInventory) return;

        if (!m_inInventory && (!m_toggleSettings || !m_toggleSettings.IsToggled()))
        {
            //UI
            m_inInventory = true;
            m_inventoryUI.SetActive(true);
            m_toolTip.SetActive(true);

            //Cursor
            m_cursorHandler.NotifyUiOpen();
        }
        else if (m_inInventory && (!m_toggleSettings || !m_toggleSettings.IsToggled()))
        {
            //UI
            m_inInventory = false;
            m_inventoryUI.SetActive(false);
            //m_toolTip.SetActive(false);

            //Cursor
            m_cursorHandler.NotifyUiClosed();
        }
    }

    public bool IsInventoryOpen()
    {
        return m_inInventory;
    }
}
