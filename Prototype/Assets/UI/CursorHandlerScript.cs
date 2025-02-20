using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[CreateAssetMenu(fileName = "CursorHandlerScript", menuName = "Scriptable Objects/CursorHandlerScript")]
public class CursorHandlerScript : ScriptableObject
{
    private bool m_isUiOpen;

    private void OnEnable()
    {
        m_isUiOpen = false;
    }

    // TODO: Might be worth making this a stack so it supports multiple ui open at once (e.g if 2 ui open and 1 close, cursor should still be locked)

    /// <summary>
    /// Let the cursor handler know that the ui is open and cursor should not be locked
    /// </summary>
    public void NotifyUiOpen()
    {
        m_isUiOpen = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Let the cursor handler know that the ui is closed and cursor should be locked
    /// </summary>
    public void NotifyUiClosed()
    {
        m_isUiOpen = false;

        Cursor.lockState = CursorLockMode.Locked;
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
