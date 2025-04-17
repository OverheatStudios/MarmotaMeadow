using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum FloorType
{
  None = 0,  Grass, Wood
}

public class FloorTypeScript : MonoBehaviour
{
    [SerializeField] private FloorType m_floorType;

    public FloorType GetFloorType()
    {
        Assert.IsTrue(m_floorType != FloorType.None);
        return m_floorType;
    }
}
