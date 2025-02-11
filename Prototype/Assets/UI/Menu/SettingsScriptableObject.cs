using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SettingsScriptableObject", menuName = "Scriptable Objects/SettingsScriptableObject")]
public class SettingsScriptableObject : ScriptableObject
{
    [SerializeField][Range(0, 1)] private float m_musicVolume;
    public float MusicVolume
    {
        get
        {
            return m_musicVolume;
        }
        set
        {
            m_musicVolume = value;
        }
    }

    private void OnEnable()
    {
        m_musicVolume = 0.5f;
    }

    public float test()
    {
        return m_musicVolume;
    }
}
