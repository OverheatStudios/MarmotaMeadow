using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurrowScript : MonoBehaviour
{
    [SerializeField] private GameObject m_groundhogPrefab;
    /// <summary>
    /// Each night groundhogs gain this amount more max health, if this is 5 and night is 1 (second night!) then groundhog has 105% base health, night 3 has 115%
    /// </summary>
    private const float HP_GAIN_PERCENT_PER_NIGHT = 5.0f;

    /// <summary>
    /// The current groundhog object in this burrow, may be null
    /// </summary>
    private GameObject m_groundhog;

    // Start is called before the first frame update
    void Start()
    {

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
        groundhogScript.SetMaxHealth(groundhogScript.GetMaxHealth() * (1 + night / (100.0f / HP_GAIN_PERCENT_PER_NIGHT)));
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
