using UnityEngine;

[CreateAssetMenu(fileName = "DataScriptableObject", menuName = "Scriptable Objects/DataScriptableObject")]
public class DataScriptableObject : ScriptableObject
{
    [HideInInspector] public int GroundhogsSpawned;
    [HideInInspector] public int GroundhogsKilled;
    [HideInInspector] public int MaxAmmo;
    [HideInInspector] public int CurrentAmmo;
    [HideInInspector] public int NightCounter;
    [HideInInspector] public int Damage;

    private void OnEnable()
    {
        GroundhogsKilled = 0;
        GroundhogsSpawned = 0;
        MaxAmmo = 30;
        CurrentAmmo = MaxAmmo;
        NightCounter = 0;
        Damage = 10;
    }
}
