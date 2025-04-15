using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static GroundhogScript;

public class GroundhogTypeInfo : MonoBehaviour
{
    [SerializeField] private Collider m_highPrecisionCollider;
    [SerializeField] private float m_healthScalar = 1.0f;
    [SerializeField] public GroundhogState CurrentState = GroundhogState.Rising;
    [SerializeField] private float m_minYBeforeDisappear = -0.5f;
    [SerializeField] private GameObject m_bloodParticleSystem;
    [SerializeField] private Vector3 m_bloodParticleOffset = new Vector3(0, 0, 0);
    [SerializeField] private AudioClip m_woundedSfx;
    [SerializeField] private SettingsScriptableObject m_settings;
    [SerializeField] private float m_coinsDropped = 5.0f;
    private AudioSource m_woundedSource;

    private void Start()
    {
        m_woundedSource = new GameObject().AddComponent<AudioSource>();
        m_woundedSource.clip = m_woundedSfx;
        m_woundedSource.loop = false;
    }

    private void OnDestroy()
    {
        Destroy(m_woundedSource);
    }

    public void PlayWoundedSfx(Vector3 worldPos)
    {
        m_woundedSource.transform.position = worldPos;
        m_woundedSource.volume = m_settings.GetSettings().GetGameVolume();
        m_woundedSource.Play();
    }

    public float GetMinYBeforeDisappear()
    {
        return m_minYBeforeDisappear;
    }

    public Collider GetHighPrecisionCollider()
    {
        return m_highPrecisionCollider;
    }

    public float GetHealthScalar()
    {
        return m_healthScalar;
    }

    public GameObject GetBloodParticle()
    {
        return m_bloodParticleSystem;
    }

    public Vector3 GetBloodParticleOffset()
    {
        return m_bloodParticleOffset;
    }

    public float GetCoinsDropped()
    {
        return m_coinsDropped;
    }
}
