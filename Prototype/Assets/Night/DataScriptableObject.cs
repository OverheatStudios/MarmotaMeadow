using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(fileName = "DataScriptableObject", menuName = "Scriptable Objects/DataScriptableObject")]
public class DataScriptableObject : ScriptableObject
{
    [HideInInspector] public int GroundhogsSpawned;
    [HideInInspector] public int GroundhogsKilled;
    [HideInInspector] public int MaxAmmo;
    [HideInInspector] public int CurrentAmmo;
    [HideInInspector] public int NightCounter;
    [HideInInspector] public int Damage;
    [HideInInspector] public int MaxHealth;
    [HideInInspector] public int CurrentHealth;

    private void OnEnable()
    {
        GroundhogsKilled = 0;
        GroundhogsSpawned = 0;
        MaxAmmo = 30;
        CurrentAmmo = MaxAmmo;
        NightCounter = 0;
        Damage = 10;
        MaxHealth = 10;
        Assert.IsTrue(MaxHealth % 2 == 0); // Max health must be even! This is due to how health bar works
        CurrentHealth = MaxHealth;
    }
}
