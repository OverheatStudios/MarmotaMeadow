using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundhogTypeInfo : MonoBehaviour
{
    [SerializeField] private Collider m_highPrecisionCollider;
    [SerializeField] private float m_healthScalar = 1.0f;

    public Collider GetHighPrecisionCollider()
    {
        return m_highPrecisionCollider;
    }

    public float GetHealthScalar() { return m_healthScalar; }
}
