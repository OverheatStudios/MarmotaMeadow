using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMenuScript : MonoBehaviour
{
    /// <summary>
    /// If true, debug menu can be opened with ctrl+d, else it can't be opened
    /// </summary>
    [SerializeField] private bool m_canOpenDebugMenu = true;

    [SerializeField] private CursorHandlerScript m_cursorHandler;

    private bool m_isDebugMenuOpen = false;

    void Update()
    {
        bool isCtrlDown = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        if (Input.GetKeyDown(KeyCode.D) && isCtrlDown)
        {
            if (m_isDebugMenuOpen) CloseDebugMenu();
            else OpenDebugMenu();
        }
    }

    void OpenDebugMenu()
    {
        if (m_cursorHandler.IsUiOpen()) return;

        m_isDebugMenuOpen = true;
        m_cursorHandler.NotifyUiOpen();
        SetActiveRecursive(gameObject, true);
    }

    void CloseDebugMenu()
    {
        m_isDebugMenuOpen = false;
        m_cursorHandler.NotifyUiOpen();
        SetActiveRecursive(gameObject, false);
    }

    void SetActiveRecursive(GameObject obj, bool enabled)
    {
        obj.SetActive(enabled);
        foreach (Transform transform in obj.transform)
        {
            SetActiveRecursive(transform.gameObject, enabled);
        }
    }
}
