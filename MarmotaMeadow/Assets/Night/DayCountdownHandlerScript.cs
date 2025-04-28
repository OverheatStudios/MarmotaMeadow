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
    [SerializeField] private NightCounter m_nightCounter;
    [SerializeField] private FadeIn m_fadeOut;
    [SerializeField] private ScrObjSun m_sun;
    private float m_secondsRemaining = 0;

    void Start()
    {
        Assert.IsFalse(m_fadeOut.gameObject.activeInHierarchy);
        Assert.IsTrue(m_data.GetData().GetNightCounter() >= 0);
        m_secondsRemaining = GetNightLengthSeconds();
        m_fadeOut.LoadSceneAfter("SunScene");
        m_sun.IsSunset = false;
    }

    void Update()
    {
        m_secondsRemaining -= Time.deltaTime;

        m_countdownText.text = string.Format("{0}", 1 + (int)m_secondsRemaining);

        if (m_secondsRemaining < 0)
        {
            m_nightCounter.NotifyNightEnd();
            m_fadeOut.gameObject.SetActive(true);
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

    public void SkipNight()
    {
        m_secondsRemaining = 0.01f;
    }
}
