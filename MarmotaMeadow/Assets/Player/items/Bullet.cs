using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "Scriptable Objects/Bullet")]
public class Bullet : BaseItem
{
    [Tooltip("What gun does this bullet work on?")]
    [SerializeField] private string m_gunName;
    [Tooltip("Price per bullet, note that you buy bullet 'packs', each pack is enough to reload a gun fully once")]
    [SerializeField] private float m_purchasePrice;

    public override float ReturnBuyCoinsAmount()
    {
        return m_purchasePrice;
    }

    public string GetGunName()
    {
        return m_gunName;
    }
}
