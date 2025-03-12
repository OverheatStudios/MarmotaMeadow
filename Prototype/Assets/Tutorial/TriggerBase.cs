using UnityEngine;
using System;

public abstract class TriggerBase : MonoBehaviour
{
    public event Action OnTriggerCompleted;

    public String StepText;

    protected void CompleteTrigger()
    {
        OnTriggerCompleted?.Invoke();
    }

    public abstract void ActivateTrigger();
}