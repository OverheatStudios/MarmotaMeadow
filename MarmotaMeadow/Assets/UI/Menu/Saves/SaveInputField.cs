using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaveInputField : MonoBehaviour
{
    [SerializeField] private SaveManager m_saveManager;

    void Start()
    {
        m_saveManager.SaveNameInputField = gameObject.GetComponent<TMP_InputField>();
    }
}
