using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FloorType
{
  None = 0,  Grass, Wood
}

public class FloorTypeScript : MonoBehaviour
{
    [SerializeField] private FloorType m_floorType;

    public FloorType GetFloorType()
    {
        return m_floorType;
    }
}
