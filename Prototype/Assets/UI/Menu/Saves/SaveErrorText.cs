using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaveErrorText : MonoBehaviour
{
    [SerializeField] private SaveManager m_saveManager;

    void Start()
    {
        m_saveManager.SaveNameErrorText = gameObject.GetComponent<TextMeshProUGUI>();
        gameObject.GetComponent<TextMeshProUGUI>().text = "";
    }
}
