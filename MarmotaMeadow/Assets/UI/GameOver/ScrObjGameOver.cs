using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScrObjGameOver", menuName = "Scriptable Objects/ScrObjGameOver")]
public class ScrObjGameOver : ScriptableObject
{
    [System.Serializable]
    public enum Reason
    {
        Bankrupt, Won, Died
    }

    public Reason GameOverReason;

    private void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset;

        GameOverReason = Reason.Won;
    }
}
