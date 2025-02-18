using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "Scriptable Objects/Seeds")]
public class Seeds : BaseItem
{
    [SerializeField] public float m_amount;
    [SerializeField] private float m_growthDuration;
    [SerializeField] private float buyCoins;
    public Crops crop;

    public float ReturnGrowDuration()
    {
        return m_growthDuration;
    }

    public float ReturnBuyCoinsAmount()
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
