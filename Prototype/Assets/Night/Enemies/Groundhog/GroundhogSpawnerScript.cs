using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GroundhogSpawnerScript : MonoBehaviour
{
    /// <summary>
    /// Parent object of all burrow objects
    /// </summary>
    [SerializeField] protected GameObject m_burrowContainer;

    /// <summary>
    /// Time until we next spawn a groundhog in seconds
    /// </summary>
    private float m_timeToSpawn = 2;

    void Start()
    {
        Assert.IsNotNull(m_burrowContainer);
    }

    void Update()
    {
        m_timeToSpawn -= Time.deltaTime;
        if (m_timeToSpawn > 0) return;

        m_timeToSpawn = Random.Range(0.5f, 1.5f);
        TrySpawnGroundhog();
    }

    /// <summary>
    /// Try and spawn a groundhog in a random empty burrow, fails silently if there was no empty burrows
    /// </summary>
    void TrySpawnGroundhog()
    {
        // Find all empty burrows
        List<BurrowScript> emptyBurrows = new List<BurrowScript>();
        foreach (Transform child in m_burrowContainer.transform)
        {
            BurrowScript burrow = child.gameObject.GetComponent<BurrowScript>();
            if (!burrow.HasNoGroundhog()) continue;

            emptyBurrows.Add(burrow);
        }

        // Spawn groundhog
        if (emptyBurrows.Count > 0)
        {
            emptyBurrows[Random.Range(0, emptyBurrows.Count)].SpawnGroundhog();
        }
    }
}
