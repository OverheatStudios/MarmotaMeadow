using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundhogTypeInfo : MonoBehaviour
{
    [SerializeField] private Collider m_highPrecisionCollider;

    public Collider GetHighPrecisionCollider()
    {
        return m_highPrecisionCollider;
    }
}
