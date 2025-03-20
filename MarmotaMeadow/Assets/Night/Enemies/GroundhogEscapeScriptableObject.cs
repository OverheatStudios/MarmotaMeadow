using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GroundhogEscapeScriptableObject", menuName = "Scriptable Objects/GroundhogEscapeScriptableObject")]
public class GroundhogEscapeScriptableObject : ScriptableObject
{
    [Tooltip("How much to damage the player per groundhog that escapes")]
    [SerializeField] private int m_damage = 10;
    [SerializeField] private ScrObjGlobalData m_data;

    [SerializeField] private GameObject m_damageTooltip;

    /// <summary>
    /// Should be called once whenever a groundhog escapes
    /// </summary>
    public void NotifyGroundhogEscaped()
    {
        m_data.Damage(m_damage);
        TooltipManager.Get().ShowTooltip(m_damageTooltip);
    }
}
