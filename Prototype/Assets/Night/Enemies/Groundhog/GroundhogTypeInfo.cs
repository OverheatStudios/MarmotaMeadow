using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GroundhogScript;

public class GroundhogTypeInfo : MonoBehaviour
{
    [SerializeField] private Collider m_highPrecisionCollider;
    [SerializeField] private float m_healthScalar = 1.0f;
    [SerializeField] public GroundhogState CurrentState = GroundhogState.Rising;
    [SerializeField] private float m_minYBeforeDisappear = -0.5f;

    public float GetMinYBeforeDisappear()
    {
        return m_minYBeforeDisappear;
    }

    public Collider GetHighPrecisionCollider()
    {
        return m_highPrecisionCollider;
    }

    public float GetHealthScalar()
    {
        return m_healthScalar;
    }

}
