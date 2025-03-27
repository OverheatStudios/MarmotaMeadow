using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "CursorHandlerScript", menuName = "Scriptable Objects/CursorHandlerScript")]
public class CursorHandlerScript : ScriptableObject
{
    [SerializeField] private GameObject m_virtualMousePrefab;

    private bool m_isUiOpen;
    private VirtualMouse m_virtualMouse;

    private void OnEnable()
    {
        m_isUiOpen = false;
        GetVirtualMouse();
    }

    private void OnDisable()
    {
        if (m_virtualMouse != null)
        {
            DestroyImmediate(m_virtualMouse.gameObject);
        }
    }

    public VirtualMouse GetVirtualMouse()
    {
        if (m_virtualMouse == null)
        {
            m_virtualMouse = Instantiate(m_virtualMousePrefab).GetComponent<VirtualMouse>();
            try
            {
                DontDestroyOnLoad(m_virtualMouse.gameObject);
            }
            catch (InvalidOperationException)
            {
                // in editor mode
                DestroyImmediate(m_virtualMouse.gameObject);
                return null;
            }
        }
        return m_virtualMouse;
    }

    // TODO: Might be worth making this a stack so it supports multiple ui open at once (e.g if 2 ui open and 1 close, cursor should still be locked)

    /// <summary>
    /// Let the cursor handler know that the ui is open and cursor should not be locked
    /// </summary>
    public void NotifyUiOpen()
    {
        m_isUiOpen = true;

        m_virtualMouse.Unlock();
        Cursor.visible = true;
    }

    /// <summary>
    /// Let the cursor handler know that the ui is closed and cursor should be locked
    /// </summary>
    public void NotifyUiClosed()
    {
        m_isUiOpen = false;

        m_virtualMouse.Lock();
        Cursor.visible = false;

    }

    /// <summary>
    /// Check if ui is marked as open
    /// </summary>
    /// <returns>True if ui open</returns>
    public bool IsUiOpen()
    {
        return m_isUiOpen;
    }
}
