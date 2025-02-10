using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPlayButtonScript : MonoBehaviour
{
    /// <summary>
    /// Called when user clicks play button
    /// </summary>
    public void Play()
    {
        SceneManager.LoadScene("Day Scene", LoadSceneMode.Single);
    }
}
