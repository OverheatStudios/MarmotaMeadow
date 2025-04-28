using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoEndHandler : MonoBehaviour
{
    [SerializeField] private FadeIn m_fadeOut;
    [SerializeField] private VideoPlayer m_player;

    void Start()
    {
        m_fadeOut.gameObject.SetActive(false);
        m_fadeOut.LoadSceneAfter("Day Scene");
        StartCoroutine(HandleTransition());
    }

    private IEnumerator HandleTransition()
    {
        yield return new WaitForSeconds((float)m_player.length - m_fadeOut.GetFadeSeconds());
        m_fadeOut.gameObject.SetActive(true);
    }
}
