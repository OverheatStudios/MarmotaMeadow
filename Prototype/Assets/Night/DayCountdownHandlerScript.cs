using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DayCountdownHandlerScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_countdownText;
    [SerializeField] private DataScriptableObject m_data;
    private float m_secondsRemaining = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_secondsRemaining = GetNightLengthSeconds();
    }

    // Update is called once per frame
    void Update()
    {
        m_secondsRemaining -= Time.deltaTime;

        m_countdownText.text = string.Format("Day in {0} seconds", 1 + (int)m_secondsRemaining);

        if (m_secondsRemaining < 0)
        {
            SwitchDayScene();
        }
    }

    /// <summary>
    /// Get length of a specific night
    /// </summary>
    /// <returns>Number of seconds the current night should last</returns>
    private float GetNightLengthSeconds()
    {
        return 90 + Mathf.Min(60, m_data.NightCounter * 5) - 1;
    }

    private void SwitchDayScene()
    {
        SceneManager.LoadScene("Day Scene", LoadSceneMode.Single);
    }
}
