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
    [Header("Config")]
    [Tooltip("Length of each night in seconds, length of last night is used for any nights past the last, must have at least one entry")]
    [SerializeField] private List<float> m_nightLengths;
    private float m_secondsRemaining = 0;

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsTrue(m_nightLengths != null && m_nightLengths.Count > 0);
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
        return m_nightLengths[Mathf.Min(m_nightLengths.Count - 1, m_data.GetData().NightCounter)];
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
