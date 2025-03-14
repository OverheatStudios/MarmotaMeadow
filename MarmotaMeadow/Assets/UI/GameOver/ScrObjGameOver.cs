using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScrObjGameOver", menuName = "Scriptable Objects/ScrObjGameOver")]
public class ScrObjGameOver : ScriptableObject
{
    public enum Reason
    {
        Bankrupt, Won, Died
    }

    public Reason GameOverReason = Reason.Won;
}
