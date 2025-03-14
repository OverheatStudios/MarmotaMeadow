using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class MultiKeyPressTrigger : TriggerBase
{
    [Header("Key Press Configuration")]
    public KeyCode[] requiredKeys;  // Keys the player must press

    private HashSet<KeyCode> keysPressed = new HashSet<KeyCode>();

    public override void ActivateTrigger()
    {
        keysPressed.Clear(); // Reset on activation
        StartCoroutine(CheckKeyPresses());
    }

    private IEnumerator CheckKeyPresses()
    {
        while (true)
        {
            foreach (var key in requiredKeys)
            {
                if (Input.GetKeyDown(key) && !keysPressed.Contains(key))
                {
                    keysPressed.Add(key);
                }
            }

            // If all keys are pressed, trigger completion
            if (keysPressed.Count == requiredKeys.Length)
            {
                CompleteTrigger();
                yield break;
            }

            yield return null;
        }
    }
}