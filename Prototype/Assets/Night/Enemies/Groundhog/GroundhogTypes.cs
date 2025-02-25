using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum GroundhogType
{
    Basic
}

[CreateAssetMenu(fileName = "GroundhogTypes", menuName = "Scriptable Objects/ScrObjGroundhogTypes")]
public class GroundhogTypes : ScriptableObject
{
    [SerializeField] private GameObject m_baseGroundhogPrefab;
    [SerializeField] private List<Pair<GroundhogType, GameObject>> m_groundhogTypesPrefabs;

    public GameObject GetBasePrefab()
    {
        return m_baseGroundhogPrefab;
    }

    public List<Pair<GroundhogType, GameObject>> GetGroundhogTypesPrefabs()
    {
        return m_groundhogTypesPrefabs;
    }

    public GroundhogScript InstantiateGroundhog(GroundhogType groundhogType)
    {
        GameObject baseGroundhog = Instantiate(m_baseGroundhogPrefab);
        GroundhogScript script = baseGroundhog.GetComponent<GroundhogScript>();

        

        GameObject type = Instantiate(GetGroundhogPrefab(groundhogType));
        type.transform.SetParent(baseGroundhog.transform, false);

        return script;
    }

    public GameObject GetGroundhogPrefab(GroundhogType groundhogType)
    {
        foreach (Pair<GroundhogType, GameObject> pair in m_groundhogTypesPrefabs)
        {
            if (pair.First == groundhogType) return pair.Second;
        }
        Assert.IsTrue(false);
        return null;
    }
}
