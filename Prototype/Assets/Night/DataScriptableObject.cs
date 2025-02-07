using UnityEngine;

[CreateAssetMenu(fileName = "DataScriptableObject", menuName = "Scriptable Objects/DataScriptableObject")]
public class DataScriptableObject : ScriptableObject
{
    [HideInInspector] public int groundhogsSpawned;
    [HideInInspector] public int groundhogsKilled;
    [HideInInspector] public int maxAmmo;
    [HideInInspector] public int currentAmmo;
    [HideInInspector] public int nightCounter;

    private void OnEnable()
    {
        groundhogsKilled = 0;
        groundhogsSpawned = 0;
        maxAmmo = 30;
        currentAmmo = maxAmmo;
        nightCounter = 10;
    }
}
