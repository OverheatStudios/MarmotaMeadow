using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GroundhogEscapeScriptableObject", menuName = "Scriptable Objects/GroundhogEscapeScriptableObject")]
public class GroundhogEscapeScriptableObject : ScriptableObject
{
    [Tooltip("How many groundhogs need to escape to cause the player to lose a single health (half a heart)")]
    [SerializeField] private int m_groundhogsPerHealthPoint = 3;
    [SerializeField] private DataScriptableObject m_data;

    [SerializeField] private GameObject m_damageTooltip;

    [HideInInspector] private int m_groundhogsEscaped = 0;

    /// <summary>
    /// Should be called once whenever a groundhog escapes
    /// </summary>
    public void NotifyGroundhogEscaped()
    {
        m_groundhogsEscaped++;
        if (m_groundhogsEscaped % m_groundhogsPerHealthPoint == 0)
        {
            m_data.Damage(1);
            TooltipManager.Get().ShowTooltip(m_damageTooltip);
        }
    }
}
