using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurrowScript : MonoBehaviour
{
    [SerializeField] private GameObject m_groundhogPrefab;

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
    public void SpawnGroundhog()
    {
        if (m_groundhog != null) Destroy(m_groundhog);

        m_groundhog = Instantiate(m_groundhogPrefab);
        m_groundhog.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
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
