using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "Scriptable Objects/Seeds")]
public class Seeds : BaseItem
{
    [SerializeField] public float m_amount;
    [SerializeField] private float m_growthDuration;
    public Crops crop;

    public float ReturnGrowDuration()
    {
        return m_growthDuration;
    }

    public override float ReturnBuyCoinsAmount()
    {
        return buyCoins;
    }

    public Crops ReturnCrop()
    {
        return crop;
    }

    public float ReturnAmount()
    {
        return m_amount;
    }
}
