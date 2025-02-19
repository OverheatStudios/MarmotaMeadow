using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(fileName = "DataScriptableObject", menuName = "Scriptable Objects/DataScriptableObject")]
public class DataScriptableObject : ScriptableObject
{
    [HideInInspector] public int GroundhogsSpawned;
    [HideInInspector] public int GroundhogsKilled;
    [HideInInspector] public int NightCounter;
    [HideInInspector] public int MaxHealth;
    [HideInInspector] public int CurrentHealth;
    [HideInInspector] public float BulletHoleSize; // Scale of bullet hole prefab (.15 is a good number)

    private void OnEnable()
    {
        GroundhogsKilled = 0;
        GroundhogsSpawned = 0;
        NightCounter = 0;
        MaxHealth = 10;
        Assert.IsTrue(MaxHealth % 2 == 0); // Max health must be even! This is due to how health bar works
        CurrentHealth = MaxHealth;
        BulletHoleSize = 0.15f;
    }
}
