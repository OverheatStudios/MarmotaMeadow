using UnityEngine;

[CreateAssetMenu(fileName = "DataScriptableObject", menuName = "Scriptable Objects/DataScriptableObject")]
public class DataScriptableObject : ScriptableObject
{
    public int groundhogsSpawned = 0;
    public int groundhogsKilled = 0;

    private void OnEnable()
    {
        groundhogsKilled = 0;
        groundhogsSpawned = 0;
    }
}
