using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class PlaySave : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_text;
    [SerializeField] private SaveManager m_saveManager;
    private string m_saveName = null;

    public void SwitchToSave()
    {
        Assert.IsNotNull(m_saveName);
        m_saveManager.SwitchSave(m_saveName);
    }

    public void SetSaveName(string saveName)
    {
        m_saveName = saveName;
        m_text.text = "" + saveName;
    }

    public void PlayGame()
    {
        MenuButtonScript.PlayGame(false);
    }
}
