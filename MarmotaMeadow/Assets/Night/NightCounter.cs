using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NightCounter : MonoBehaviour
{
    [SerializeField] private ScrObjGlobalData m_data;
    [SerializeField] private ScrObjGameOver m_gameOverReason;
    [Tooltip("Lose on this night")]
    [SerializeField] private ScrObjNumNights m_numNights;
    [SerializeField] private CoinManager m_coinManager;
    [SerializeField] private ScrObjSun m_sun;
    private bool m_notified = false;

    public void NotifyNightEnd()
    {
        if (m_notified) return;
        m_notified = true;
        m_data.IncrementNightCounter();
        if (m_data.GetData().GetNightCounter() >= m_numNights.GetFinalNightPlusOne())
        {
            m_gameOverReason.GameOverReason = m_coinManager.GetCoins() >= m_numNights.GetMoneyRequired() ? ScrObjGameOver.Reason.Won: ScrObjGameOver.Reason.Bankrupt;
            m_sun.GameOver = true;
        }
    }
}
