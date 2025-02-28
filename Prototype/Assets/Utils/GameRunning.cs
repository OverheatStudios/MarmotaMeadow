using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameRunning
{
    public static bool IsGameRunning()
    {
#if UNITY_EDITOR
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return true;
        }
#endif
        return Application.isPlaying;
    }
}
