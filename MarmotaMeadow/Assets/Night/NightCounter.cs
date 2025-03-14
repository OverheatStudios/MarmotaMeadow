using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NightCounter : MonoBehaviour
{
    [SerializeField] private DataScriptableObject m_data;
    [SerializeField] private ScrObjGameOver m_gameOverReason;
    [Tooltip("Lose on this night")]
    [SerializeField] private ScrObjNumNights m_numNights;
    [SerializeField] private CoinManager m_coinManager;

    void Start()
    {
        m_data.NightCounter++;
        if (m_data.NightCounter >= m_numNights.GetFinalNightPlusOne())
        {
            m_gameOverReason.GameOverReason = m_coinManager.GetCoins() >= m_numNights.GetMoneyRequired() ? ScrObjGameOver.Reason.Won: ScrObjGameOver.Reason.Bankrupt;
            SceneManager.LoadScene("GameOverScene", LoadSceneMode.Single);
        }
    }
}
