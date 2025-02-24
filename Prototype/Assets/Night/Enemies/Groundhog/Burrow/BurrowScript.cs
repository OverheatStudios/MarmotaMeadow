
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BurrowScript : MonoBehaviour
{
    [SerializeField] private GameObject m_groundhogPrefab;
    [Tooltip("Each night groundhogs gain this amount more max health, if this is 5 for element 1 (second element) then on night 1 (second night) groundhogs have 105% health")]
    [SerializeField] private List<float> m_groundhogBonusHealthPercentages;

    /// <summary>
    /// The current groundhog object in this burrow, may be null
    /// </summary>
    private GameObject m_groundhog;

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsTrue(m_groundhogBonusHealthPercentages != null && m_groundhogBonusHealthPercentages.Count > 0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Spawn a groundhog in this burrow, destroying any existing groundhog
    /// </summary>
    public void SpawnGroundhog(int night)
    {
        if (m_groundhog != null) Destroy(m_groundhog);

        m_groundhog = Instantiate(m_groundhogPrefab);
        m_groundhog.transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        GroundhogScript groundhogScript = m_groundhog.GetComponent<GroundhogScript>();
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
