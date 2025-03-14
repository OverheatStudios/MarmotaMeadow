using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalDataSaveLoad : MonoBehaviour
{
    [SerializeField] private ScrObjGlobalData m_data;

    private void OnEnable()
    {
        m_data.Load();
    }

    private void OnDisable()
    {
        m_data.Save();
    }
}
