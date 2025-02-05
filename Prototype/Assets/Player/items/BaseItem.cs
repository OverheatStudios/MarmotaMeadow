using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "Scriptable Objects/BaseItem")]
public class BaseItem : ScriptableObject
{
    [SerializeField] protected string itemName;
}
