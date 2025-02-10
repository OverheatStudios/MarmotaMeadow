using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "Scriptable Objects/Seeds")]
public class Seeds : BaseItem
{
    [SerializeField] private float m_amount;
    [SerializeField] private float m_growthDuration;

    public float ReturnGrowDuration()
    {
        return m_growthDuration;
    }
}
