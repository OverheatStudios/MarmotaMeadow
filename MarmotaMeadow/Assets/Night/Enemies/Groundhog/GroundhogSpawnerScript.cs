using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public class GroundhogSpawn : IComparable
{
    [Header("Groundhog Spawn Set")]
    public float SecondsSinceNightBeginning = 0;
    public GroundhogType GroundhogType;
    public int NumberGroundhogs = 1;
    public float SpawnInterval = 1.0f;
    [HideInInspector] public float SecondsSinceLastSpawn = 0;

    /// <summary>
    /// Check if a groundhog should spawn this frame, updates seconds since last spawn
    /// </summary>
    /// <param name="deltaTime">Delta time this frame</param>
    /// <returns>True if a groundhog should spawn</returns>
    public bool ShouldSpawnThisFrame(float deltaTime)
    {
        SecondsSinceLastSpawn += deltaTime;
        if (SecondsSinceLastSpawn >= SpawnInterval)
        {
            SecondsSinceLastSpawn = 0;
            return true;
        }
        return false;
    }

    public int CompareTo(object obj)
    {
        if (obj is GroundhogSpawn spawn)
        {
            return SecondsSinceNightBeginning.CompareTo(spawn.SecondsSinceNightBeginning);
        }
        return 0;
    }
}

[System.Serializable]
public class NightGroundhogSpawns
{
    [Header("---NIGHT---")]
    public List<GroundhogSpawn> GroundhogsThisNight;

    /// <summary>
    /// Sort the list in descending order of spawn time
    /// </summary>
    public void Setup()
    {
        GroundhogsThisNight.Sort();
        GroundhogsThisNight.Reverse();

        foreach (var spawn in GroundhogsThisNight)
        {
            spawn.SecondsSinceLastSpawn = spawn.SpawnInterval;
        }
    }
}

public class GroundhogSpawnerScript : MonoBehaviour
{
    /// <summary>
    /// Script of parent object of all burrow objects
    /// </summary>
    [SerializeField] private BurrowContainerScript m_burrowContainer;

    [Tooltip("What groundhogs spawn each night")]
    [SerializeField] private List<NightGroundhogSpawns> m_nightGroundhogSpawns;

    /// <summary>
    /// Game data
    /// </summary>
    [SerializeField] private DataScriptableObject m_data;

    private float m_secondsSinceNightBegin = 0;

    void Start()
    {
        GetSpawnsTonight().Setup();
    }

    void Update()
    {
        m_secondsSinceNightBegin += Time.deltaTime;

        TrySpawnGroundhog();
    }

    private NightGroundhogSpawns GetSpawnsTonight()
    {
        return m_nightGroundhogSpawns[Mathf.Min(m_nightGroundhogSpawns.Count - 1, m_data.NightCounter)];
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
                    return;
                } // Not ready for this spawn yet
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
                    emptyBurrows[UnityEngine.Random.Range(0, emptyBurrows.Count)].SpawnGroundhog(m_data.NightCounter, nextSpawn.GroundhogType);
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