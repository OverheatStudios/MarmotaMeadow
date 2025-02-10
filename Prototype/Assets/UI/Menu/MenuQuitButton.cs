using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuQuitButton : MonoBehaviour
{
    /// <summary>
    /// Called when quit button is clicked
    /// </summary>
    public void Quit()
    {
        Application.Quit(0);
    }
}
