using System.Collections;
using UnityEngine;

public class KeyPressTrigger : TriggerBase
{
    public KeyCode keyToPress = KeyCode.E;
    [SerializeField] private bool isMouseWheel;

    public override void ActivateTrigger()
    {
        StartCoroutine(WaitForKeyPress());
    }

    private IEnumerator  WaitForKeyPress()
    {
        if (!isMouseWheel)
        {
            while (!Input.GetKeyDown(keyToPress))
            {
                yield return null;
            }
        }
        else if(isMouseWheel)
        {
            while (Input.GetAxis("Mouse ScrollWheel") == 0)
            {
                yield return null;
            }
        }

        CompleteTrigger();
    }
}