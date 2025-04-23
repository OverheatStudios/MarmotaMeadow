using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class CrouchTutorial : MonoBehaviour
{
    [SerializeField] private SaveManager m_saveManager;
    [SerializeField] private MovementScript m_movementScript;
    [SerializeField] private TextMeshProUGUI m_text;
    [SerializeField] private string m_textText = "Press [KEY] to crouch.";

    void Start()
    {
        string path = m_saveManager.GetFilePath("crouchTutorial");
        if (File.Exists(path))
        {
            Destroy(gameObject);
            return;
        }
    }

    void Update()
    {
        var keybind = GameInput.GetKeybind("Crouch");
        m_text.text = m_textText.Replace("[KEY]", keybind.ToString());
        if (keybind.GetKeyDown())
        {
            string path = m_saveManager.GetFilePath("crouchTutorial");
            File.Create(path).Close();

            Destroy(gameObject);
            return;
        }
    }
}
