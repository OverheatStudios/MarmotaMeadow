using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ScrObjNumNights", menuName = "Scriptable Objects/ScrObjNumNights")]
public class ScrObjNumNights : ScriptableObject
{
    [SerializeField] private int m_numNights;
    [SerializeField] private int m_moneyRequired = 1000;

    public int GetFinalNightPlusOne()
    {
        return m_numNights;
    }

    public int GetMoneyRequired()
    {
        return m_moneyRequired; 
    }
}
