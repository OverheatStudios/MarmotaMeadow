
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BurrowScript : MonoBehaviour
{
    [SerializeField] private GroundhogTypes m_groundhogTypes;

    [Tooltip("Each night groundhogs gain this amount more max health, if this is 5 for element 1 (second element) then on night 1 (second night) groundhogs have 105% health")]
    [SerializeField] private List<float> m_groundhogBonusHealthPercentages;

    [SerializeField] private CoinManager m_coinManager;

    [SerializeField] private GameObject m_groundhogSpawnParticles;
    [SerializeField] private Vector3 m_particleSpawnOffset = Vector3.up * 0.5f;

    /// <summary>
    /// The current groundhog object in this burrow, may be null
    /// </summary>
    private GameObject m_groundhog;
    private bool m_wasGroundhogActive;

    void Start()
    {
        Assert.IsTrue(m_groundhogBonusHealthPercentages != null && m_groundhogBonusHealthPercentages.Count > 0);
    }

    private void OnDisable()
    {
        if (m_groundhog)
        {
            m_wasGroundhogActive = m_groundhog.activeInHierarchy;
            m_groundhog.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (m_groundhog)
        {
            m_groundhog.SetActive(m_wasGroundhogActive);
        }
    }

    /// <summary>
    /// Spawn a groundhog in this burrow, destroying any existing groundhog
    /// </summary>
    public void SpawnGroundhog(int night, GroundhogType type)
    {
        Instantiate(m_groundhogSpawnParticles).transform.position = transform.position + m_particleSpawnOffset;

        if (m_groundhog != null) Destroy(m_groundhog);

        GroundhogScript groundhogScript = m_groundhogTypes.InstantiateGroundhog(type);

        groundhogScript.SetCoinManager(m_coinManager);
        m_groundhog = groundhogScript.gameObject;
        m_groundhog.transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        groundhogScript.SetMaxHealth(groundhogScript.GetMaxHealth() * (1 + night / (100.0f / GetGroundhogHealthPercentBonus(night))));
    }

    private float GetGroundhogHealthPercentBonus(int night)
    {
        return m_groundhogBonusHealthPercentages[Mathf.Min(m_groundhogBonusHealthPercentages.Count - 1, night)];
    }

    /// <summary>
    /// Check if a groundhog exists in this burrow
    /// </summary>
    /// <returns>True if a groundhog exists, else false</returns>
    public bool HasNoGroundhog()
    {
        return m_groundhog == null;
    }
}
