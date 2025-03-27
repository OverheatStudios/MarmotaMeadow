using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverBackground : MonoBehaviour
{
    [SerializeField] private ScrObjGameOver m_gameOverReason;
    [SerializeField] private Sprite m_spriteWon;
    [SerializeField] private Sprite m_spriteDied;
    [SerializeField] private Sprite m_spriteBankrupt;
    [SerializeField] private Image m_background;
    [SerializeField] private SaveManager m_saveManager;

    private void Start()
    {
        switch (m_gameOverReason.GameOverReason)
        {
            case ScrObjGameOver.Reason.Won:
                m_background.sprite = m_spriteWon;
                break;
            case ScrObjGameOver.Reason.Died:
                m_background.sprite = m_spriteDied;
                break;
            case ScrObjGameOver.Reason.Bankrupt:
                m_background.sprite = m_spriteBankrupt;
                break;
        }

        StartCoroutine(m_saveManager.MarkSaveGameOver(m_saveManager.GetCurrentSavePath()));
    }
}
