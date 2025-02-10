using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GroundhogSpawnerScript : MonoBehaviour
{
    /// <summary>
    /// Script of parent object of all burrow objects
    /// </summary>
    [SerializeField] private BurrowContainerScript m_burrowContainer;

    /// <summary>
    /// Seconds until we start spawning groundhogs
    /// </summary>
    [SerializeField] private float m_secondsBeforeSpawning = 2;

    /// <summary>
    /// How much randomness in the spawn interval? 0.1 is +- 10%
    /// </summary>
    [Range(0, 1)]
    [SerializeField] private float m_randomIntervalPercent = 0.1f;

    /// <summary>
    /// Game data
    /// </summary>
    [SerializeField] private DataScriptableObject m_data;

    /// <summary>
    /// Time until we next spawn a groundhog in seconds
    /// </summary>
    private float m_timeToSpawn = 2;

    void Start()
    {
        m_timeToSpawn = m_secondsBeforeSpawning;
    }

    void Update()
    {
        m_timeToSpawn -= Time.deltaTime;
        if (m_timeToSpawn > 0) return;

        SetTimeToSpawn();
        TrySpawnGroundhog();
    }

    /// <summary>
    /// Set spawn interval for groundhogs, slightly randomised
    /// </summary>
    private void SetTimeToSpawn()
    {
        float baseInterval = 5.0f / m_burrowContainer.CurrentBurrowCount;
        float randomness = Random.Range(1 - m_randomIntervalPercent, 1 + m_randomIntervalPercent);
        m_timeToSpawn = baseInterval * randomness;
    }

    /// <summary>
    /// Try and spawn a groundhog in a random empty burrow, fails silently if there was no empty burrows
    /// </summary>
    private void TrySpawnGroundhog()
    {
        // Find all empty burrows
        List<BurrowScript> emptyBurrows = new List<BurrowScript>();
        foreach (Transform child in m_burrowContainer.transform)
        {
            if (!child.gameObject.activeInHierarchy) continue;

            BurrowScript burrow = child.gameObject.GetComponent<BurrowScript>();
            if (!burrow.HasNoGroundhog()) continue;

            emptyBurrows.Add(burrow);
        }

        // Spawn groundhog
        if (emptyBurrows.Count > 0)
        {
            emptyBurrows[Random.Range(0, emptyBurrows.Count)].SpawnGroundhog(m_data.NightCounter);
        }
    }
}
