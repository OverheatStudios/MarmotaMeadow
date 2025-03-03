using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private SettingsScriptableObject m_settings;
    [Tooltip("Takes 1/fadeSpeed seconds to fade in")]
    [SerializeField] private float m_volumeFadeSpeed = 1.0f;
    private float m_volume = 0;

    private void Update()
    {
        if (m_volume < 1)
        {
            m_volume += Time.deltaTime * m_volumeFadeSpeed;
        }

        m_audioSource.volume = GetVolume();
    }

    private float GetVolume()
    {
        return m_volume * m_settings.GetSettings().GetMusicVolume();
    }
}
