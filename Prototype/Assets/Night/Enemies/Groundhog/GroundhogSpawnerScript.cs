using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GroundhogSpawnerScript : MonoBehaviour
{
    [SerializeField] protected GameObject m_burrowContainer;
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

    void TrySpawnGroundhog()
    {
        List<BurrowScript> emptyBurrows = new List<BurrowScript>();
        foreach (Transform child in m_burrowContainer.transform)
        {
            BurrowScript burrow = child.gameObject.GetComponent<BurrowScript>();
            if (!burrow.HasNoGroundhog()) continue;

            emptyBurrows.Add(burrow);
        }

        if (emptyBurrows.Count > 0)
        {
            emptyBurrows[Random.Range(0, emptyBurrows.Count)].SpawnGroundhog();
        }
    }
}
