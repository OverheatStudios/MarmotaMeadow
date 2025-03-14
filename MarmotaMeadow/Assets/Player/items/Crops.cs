using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "Scriptable Objects/Crop")]
public class Crops : BaseItem
{
    [SerializeField] private float sellCoins;
    
    public float ReturnSellCoinsAmount()
    {
        return sellCoins;
    }
}
