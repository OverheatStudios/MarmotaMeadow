using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Forces cursor handler to have the ui open while this object is alive, should be used for ui scenes like shop
/// </summary>
public class UiAlwaysOpenScript : MonoBehaviour
{
    [SerializeField] private CursorHandlerScript m_cursorHandler;
    void Update()
    {
        if (!m_cursorHandler.IsUiOpen())
        {
            m_cursorHandler.NotifyUiOpen();
        }
    }
}
