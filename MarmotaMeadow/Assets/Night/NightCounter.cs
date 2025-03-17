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

    /// <summary>
    /// Shuold be called before going to night scene
    /// </summary>
    /// <returns>True if shold go to night scene, false if we went to game over scene</returns>
    public bool NotifyPreNightStart()
    {
        m_data.IncrementNightCounter();
        if (m_data.GetData().GetNightCounter() >= m_numNights.GetFinalNightPlusOne())
        {
            m_gameOverReason.GameOverReason = m_coinManager.GetCoins() >= m_numNights.GetMoneyRequired() ? ScrObjGameOver.Reason.Won: ScrObjGameOver.Reason.Bankrupt;
            SceneManager.LoadScene("GameOverScene", LoadSceneMode.Single);
            return false;
        }
        return true;
    }
}
