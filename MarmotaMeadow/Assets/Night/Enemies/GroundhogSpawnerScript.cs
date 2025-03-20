using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public class GroundhogSpawnerScript : MonoBehaviour
{
    /// <summary>
    /// Script of parent object of all burrow objects
    /// </summary>
    [SerializeField] private BurrowContainerScript m_burrowContainer;

    /// <summary>
    /// Game data
    /// </summary>
    [SerializeField] private ScrObjGlobalData m_data;

    [SerializeField] private AllGroundhogSpawns m_allGroundhogSpawns;

    private float m_secondsSinceNightBegin = 0;
    private List<NightGroundhogSpawns> m_nightGroundhogSpawns;

    void Start()
    {
        m_nightGroundhogSpawns = m_allGroundhogSpawns.GetNightGroundhogSpawns();
        GetSpawnsTonight().Setup();
    }

    void Update()
    {
        m_secondsSinceNightBegin += Time.deltaTime;

        TrySpawnGroundhog();
    }

    private NightGroundhogSpawns GetSpawnsTonight()
    {
        return m_nightGroundhogSpawns[Mathf.Min(m_nightGroundhogSpawns.Count - 1, m_data.GetData().GetNightCounter())];
    }

    /// <summary>
    /// Try and spawn a groundhog in a random empty burrow, fails silently if there was no empty burrows
    /// </summary>
    private void TrySpawnGroundhog()
    {
        NightGroundhogSpawns spawns = GetSpawnsTonight();
        GroundhogSpawn nextSpawn = null;

        for (int i = 1; i <= spawns.GroundhogsThisNight.Count; i++)
        {
            do
            {
                // Should we spawn something?
                if (spawns.GroundhogsThisNight.Count <= 0)
                {
                    return;
                }
                nextSpawn = spawns.GroundhogsThisNight[spawns.GroundhogsThisNight.Count - i];
                if (nextSpawn.SecondsSinceNightBeginning >= m_secondsSinceNightBegin)
                {
                    return; // Not ready for this spawn yet
                }
                if (!nextSpawn.ShouldSpawnThisFrame(Time.deltaTime))
                {
                    break; // Spawned some of this spawn too recently
                }

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
                    emptyBurrows[UnityEngine.Random.Range(0, emptyBurrows.Count)].SpawnGroundhog(m_data.GetData().GetNightCounter(), nextSpawn.GroundhogType);
                    nextSpawn.NumberGroundhogs--;
                    if (nextSpawn.NumberGroundhogs <= 0)
                    {
                        spawns.GroundhogsThisNight.Remove(nextSpawn);
                    }
                }

                break;
            } while (true);
        }
    }
}