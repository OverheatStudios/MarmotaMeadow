using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealthBarScript : MonoBehaviour
{
    [SerializeField] private ScrObjGlobalData m_data;
    [SerializeField] private Canvas m_canvas;
    [SerializeField] private ScrObjGameOver m_gameOverReason;
    [SerializeField] private TextMeshProUGUI m_healthText;
    [SerializeField] private Image m_image;
    [SerializeField] private List<Pair<Color, float>> m_colours;

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsTrue(ScrObjGlobalData.MAX_HEALTH >= 1);
        Assert.IsTrue(m_colours.Count > 0 && m_colours[0].Second == 0);

        float v = 0;
        for (int i = 1; i < m_colours.Count; i++)
        {
            float v2 = m_colours[i].Second;
            Assert.IsTrue(v2 > v);
            Assert.IsTrue(v2 <= 1);
            v = v2;
        }

        m_data.GetData().CurrentHealth = ScrObjGlobalData.MAX_HEALTH;
    }

    // Update is called once per frame
    void Update()
    {
        ClampHealthValue();

        if (m_data.GetData().CurrentHealth <= 0)
        {
            // Player lost
            m_gameOverReason.GameOverReason = ScrObjGameOver.Reason.Died;
            SceneManager.LoadScene("GameOverScene", LoadSceneMode.Single);
            return;
        }

        m_healthText.text = m_data.GetData().CurrentHealth + "%";

        // Set colour
        float healthFraction = (float)m_data.GetData().CurrentHealth / (float)ScrObjGlobalData.MAX_HEALTH;
        int i = 0;
        while (i < m_colours.Count && m_colours[i].Second < healthFraction) i++;
        i--;
        int nextIndex = i + 1 >= m_colours.Count ? i : i + 1;
        float t = Mathf.InverseLerp(m_colours[i].Second, m_colours[nextIndex].Second, healthFraction);
        Color color = Color.Lerp(m_colours[i].First, m_colours[nextIndex].First, t);
        color.a = 1;
        m_healthText.color = color;
        m_image.color = color;
    }

    public void SetHealth(int health)
    {
        if (health < 1) health = 1;
        if (health > ScrObjGlobalData.MAX_HEALTH) health = ScrObjGlobalData.MAX_HEALTH;
        m_data.GetData().CurrentHealth = health;
    }

    /// <summary>
    /// Ensure max and current healths are valid
    /// </summary>
    private void ClampHealthValue()
    {
        m_data.GetData().CurrentHealth = Mathf.Max(m_data.GetData().CurrentHealth, 0);
        m_data.GetData().CurrentHealth = Mathf.Min(m_data.GetData().CurrentHealth, ScrObjGlobalData.MAX_HEALTH);
    }
}
