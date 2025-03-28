using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class DayCountdownHandlerScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_countdownText;
    [SerializeField] private ScrObjGlobalData m_data;
    [SerializeField] private AllGroundhogSpawns m_allGroundhogSpawns;
    [Tooltip("Number of seconds between groundhog spawning ending and night ending")]
    [SerializeField] private float m_nightBufferSeconds = 8;
    private float m_secondsRemaining = 0;

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsTrue(m_data.GetData().GetNightCounter() >= 0);
        m_secondsRemaining = GetNightLengthSeconds();
    }

    // Update is called once per frame
    void Update()
    {
        m_secondsRemaining -= Time.deltaTime;

        m_countdownText.text = string.Format("{0}", 1 + (int)m_secondsRemaining);

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
        return m_allGroundhogSpawns.GetMinimumNightLength(m_data.GetData().GetNightCounter()) + m_nightBufferSeconds;
    }

    private void SwitchDayScene()
    {
        SceneManager.LoadScene("Day Scene", LoadSceneMode.Single);
    }

    public void SkipNight()
    {
        m_secondsRemaining = 0.01f;
    }
}
