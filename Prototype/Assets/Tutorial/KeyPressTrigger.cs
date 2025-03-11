using UnityEngine;

public class KeyPressTrigger : TriggerBase
{
    public KeyCode keyToPress = KeyCode.E;

    public override void ActivateTrigger()
    {
        StartCoroutine(WaitForKeyPress());
    }

    private System.Collections.IEnumerator WaitForKeyPress()
    {
        while (!Input.GetKeyDown(keyToPress))
        {
            yield return null;
        }

        CompleteTrigger();
    }
}