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

    public override float ReturnBuyCoinsAmount()
    {
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
