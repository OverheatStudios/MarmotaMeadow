using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "Scriptable Objects/Seeds")]
public class Seeds : BaseItem
{
    [SerializeField] public float m_amount;
    [SerializeField] private float m_growthDuration;
    public Crops crop;
    [SerializeField] private float m_purchasePrice;
    [SerializeField] private Sprite plantedSprite;
    [SerializeField] private Sprite growingSprite;
    [SerializeField] private Sprite finishedSprite;
    [SerializeField] private float m_sellPrice;

    public float GetSellPrice()
    {
        return m_sellPrice;
    }

    public override float ReturnBuyCoinsAmount()
    {
        Assert.IsTrue(m_purchasePrice > 0);
        Assert.IsTrue(m_purchasePrice >= m_sellPrice);
        return m_purchasePrice;
    }

    public float ReturnGrowDuration()
    {
        return m_growthDuration;
    }

    public Crops ReturnCrop()
    {
        return crop;
    }

    public float ReturnAmount()
    {
        return m_amount;
    }

    public Sprite ReturnPlantedSprite()
    {
        return plantedSprite;
    }

    public Sprite ReturnGrowingSprite()
    {
        return growingSprite;
    }

    public Sprite ReturnFinishedSprite()
    {
        return finishedSprite;
    }
}
