using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverControls : MonoBehaviour
{
   public void GoToMainMenu()
    {
        SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
    }
}
