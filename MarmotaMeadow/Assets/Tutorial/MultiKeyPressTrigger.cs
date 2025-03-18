using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class MultiKeyPressTrigger : TriggerBase
{
    [Header("Key Press Configuration")]
    [SerializeField] private string[] requiredKeyActionNames;
    private KeyCode[] requiredKeys;  // Keys the player must press

    private HashSet<KeyCode> keysPressed = new HashSet<KeyCode>();
    private string m_originalText;
    [SerializeField] private bool isMouseWheel;

    public override void ActivateTrigger()
    {
        keysPressed.Clear(); // Reset on activation
        StartCoroutine(CheckKeyPresses());
    }

    private void Start()
    {
        m_originalText = StepText;
        UpdateKeys();
    }

    private void Update()
    {
        if (Keybind.DidKeybindsChange())
        {
            UpdateKeys();
        }
    }

    void UpdateKeys()
    {
        requiredKeys = new KeyCode[requiredKeyActionNames.Length];
        for (int i = 0; i < requiredKeyActionNames.Length; i++)
        {
            requiredKeys[i] = Keybind.GetKeyCode(requiredKeyActionNames[i]);
        }

        StepText = m_originalText;
        var keys = new StringBuilder();
        foreach (KeyCode key in requiredKeys) keys = keys.Append(key);
        StepText = StepText.Replace("[KEYS]", keys.ToString());
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