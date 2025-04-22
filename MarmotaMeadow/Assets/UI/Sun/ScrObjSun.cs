using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScrObjSun", menuName = "Scriptable Objects/ScrObjSun")]

public class ScrObjSun : ScriptableObject
{
    public bool IsSunset = true;
    public bool GameOver = false;
}
