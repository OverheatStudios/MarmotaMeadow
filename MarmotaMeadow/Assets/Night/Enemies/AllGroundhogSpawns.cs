using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GroundhogSpawn : IComparable
{
    [Header("Groundhog Spawn Set")]
    public float SecondsSinceNightBeginning = 0;
    public GroundhogType GroundhogType;
    public int NumberGroundhogs = 1;
    public float SpawnInterval = 1.0f;
    [HideInInspector] public float SecondsSinceLastSpawn = 0;

    public GroundhogSpawn() { }
    public GroundhogSpawn(GroundhogSpawn other)
    {
        SecondsSinceNightBeginning = other.SecondsSinceNightBeginning;
        GroundhogType = other.GroundhogType;
        NumberGroundhogs = other.NumberGroundhogs;
        SpawnInterval = other.SpawnInterval;
        SecondsSinceLastSpawn = 0;
    }

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

    public NightGroundhogSpawns() { }
    public NightGroundhogSpawns(NightGroundhogSpawns other)
    {
        GroundhogsThisNight = new();
        foreach (var v in other.GroundhogsThisNight) GroundhogsThisNight.Add(new(v));
    }


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

[CreateAssetMenu(fileName = "AllGroundhogSpawns", menuName = "Scriptable Objects/AllGroundhogSpawns")]
public class AllGroundhogSpawns : ScriptableObject
{
    [Tooltip("What groundhogs spawn each night")]
    [SerializeField] private List<NightGroundhogSpawns> m_nightGroundhogSpawns;

    public List<NightGroundhogSpawns> GetNightGroundhogSpawns()
    {
        List<NightGroundhogSpawns> list = new();
        foreach (var v in m_nightGroundhogSpawns) { list.Add(new(v)); }
        return list;
    }

    /// <summary>
    /// Count how many groundhogs total will spawn
    /// </summary>
    /// <param name="night">Night (0 indexed)</param>
    /// <param name="type">Type of groundhog</param>
    /// <returns>Number of spawns</returns>
    public int CountSpawnsOnNight(int night, GroundhogType type)
    {
        var spawns = m_nightGroundhogSpawns[Mathf.Min(m_nightGroundhogSpawns.Count - 1, night)];
        int count = 0;
        foreach (var spawnSet in spawns.GroundhogsThisNight)
        {
            if (spawnSet.GroundhogType == type)
            {
                count += spawnSet.NumberGroundhogs;
            }
        }
        return count;
    }

    public float GetMinimumNightLength(int night)
    {
        night = Mathf.Min(m_nightGroundhogSpawns.Count - 1, night);
        float length = 0;
        foreach (var spawnSet in m_nightGroundhogSpawns[night].GroundhogsThisNight)
        {
            float spawnSetLength = spawnSet.NumberGroundhogs * spawnSet.SpawnInterval + spawnSet.SecondsSinceNightBeginning;
            length = Mathf.Max(length, spawnSetLength);
        }
        return length + 0.01f;
    }
}
