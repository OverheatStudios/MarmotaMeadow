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
    [SerializeField] private DiscordRPCHandler m_discordRPCHandler;
    [SerializeField] private string m_wonDetails = "Winning";
    [SerializeField] private string m_diedDetails = "Turning into a groundhog";
    [SerializeField] private string m_bankruptDetails = "Filing for bankruptcy";

    private void Start()
    {
        switch (m_gameOverReason.GameOverReason)
        {
            case ScrObjGameOver.Reason.Won:
                m_background.sprite = m_spriteWon;
                m_discordRPCHandler.UpdatePresence(m_wonDetails);
                break;
            case ScrObjGameOver.Reason.Died:
                m_background.sprite = m_spriteDied;
                m_discordRPCHandler.UpdatePresence(m_diedDetails);
                break;
            case ScrObjGameOver.Reason.Bankrupt:
                m_background.sprite = m_spriteBankrupt;
                m_discordRPCHandler.UpdatePresence(m_bankruptDetails);
                break;
        }

        StartCoroutine(m_saveManager.MarkSaveGameOver(m_saveManager.GetCurrentSavePath()));
    }
}
