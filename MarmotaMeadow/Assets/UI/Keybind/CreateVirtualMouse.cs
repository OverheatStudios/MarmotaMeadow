using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateVirtualMouse : MonoBehaviour
{
    [SerializeField] private CursorHandlerScript m_cursorHandlerScript;

    void Start()
    {
        m_cursorHandlerScript.GetVirtualMouse();
    }

}
