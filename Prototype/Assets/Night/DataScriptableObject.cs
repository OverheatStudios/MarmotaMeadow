using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "DataScriptableObject", menuName = "Scriptable Objects/DataScriptableObject")]
public class DataScriptableObject : ScriptableObject
{
    [HideInInspector] public int GroundhogsSpawned;
    [HideInInspector] public int GroundhogsKilled;
    [HideInInspector] public int NightCounter;
    [HideInInspector] public int MaxHealth;
    [HideInInspector] public int CurrentHealth;
    private bool m_isInfiniteHealthCheatEnabled;

    private void OnEnable()
    {
        GroundhogsKilled = 0;
        GroundhogsSpawned = 0;
        NightCounter = 0;
        MaxHealth = 15;
        CurrentHealth = MaxHealth;
        m_isInfiniteHealthCheatEnabled = false;
    }

    public void SetInfiniteHealthCheatStatus(Toggle toggleUi)
    {
        m_isInfiniteHealthCheatEnabled = toggleUi.isOn;
    }

    /// <summary>
    /// Decrements current health
    /// </summary>
    /// <param name="damage">Amount of damage</param>
    public void Damage(int damage)
    {
        if (m_isInfiniteHealthCheatEnabled) return;
        CurrentHealth -= damage;
    }
}
