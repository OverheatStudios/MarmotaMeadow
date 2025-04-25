using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour
{
    [SerializeField] private float m_fadeTime = 1;
    [SerializeField] private Image m_image;
    [SerializeField] private float m_fadeDelay = 0.5f;
    [SerializeField] private bool m_fadeOut = false;
    private float m_secondsFading = 0;
    private float m_secondsDelaying = 0;
    private string m_sceneAfter = "";

    private void Start()
    {
        m_image.enabled = true;
        var color = Color.black;
        color.a = m_fadeOut ? 0 : 1;
        m_image.color = color;
    }

    void Update()
    {
        m_secondsDelaying += Time.deltaTime;
        if (m_secondsDelaying < m_fadeDelay) return;

        m_secondsFading += Time.deltaTime;
        var color = m_image.color;
        color.a = Mathf.InverseLerp(0, m_fadeTime, m_secondsFading);
        if (!m_fadeOut) color.a = 1 - color.a;
        m_image.color = color;
        if (m_fadeTime <= m_secondsFading) { 
            if (m_sceneAfter.Length > 0)
            {
                SceneManager.LoadScene(m_sceneAfter, LoadSceneMode.Single);
                return;
            }
            Destroy(m_image.gameObject);
        }
    }

    public float GetFadeSeconds()
    {
        return m_fadeTime;
    }

    public void LoadSceneAfter(string sceneName)
    {
        m_sceneAfter = sceneName;
    }
}
