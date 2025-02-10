using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "Scriptable Objects/BaseItem")]
public class BaseItem : ScriptableObject
{
    [SerializeField] protected string m_itemName;
    [SerializeField] protected bool m_isStackable;
    [SerializeField] protected Sprite m_image;
    [SerializeField] protected int m_maxAmount;
    
    public Sprite ReturnImage()
    {
        return m_image;
    }
    
    public int ReturnMaxAmount()
    {
        return m_maxAmount;
    }
    
    public bool IsStackable()
    {
        return m_isStackable;
    }
}
