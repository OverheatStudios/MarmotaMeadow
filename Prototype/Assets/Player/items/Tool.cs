using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "Scriptable Objects/Tool")]
public class Tool : BaseItem
{
    [SerializeField] private float toolMultiplier;
    [SerializeField] private float toolLevel;
    [SerializeField] private float toolLevelCost;
}
