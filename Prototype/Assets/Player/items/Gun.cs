using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "Scriptable Objects/Gun")]
public class Gun : BaseItem
{
    [SerializeField] private int m_damage;
    
    public int GetDamage()
    {
        return m_damage;
    }
}
