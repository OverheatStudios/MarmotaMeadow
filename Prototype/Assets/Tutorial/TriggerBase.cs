using UnityEngine;
using System;

public abstract class TriggerBase : MonoBehaviour
{
    public event Action OnTriggerCompleted;

    protected void CompleteTrigger()
    {
        OnTriggerCompleted?.Invoke();
    }

    public abstract void ActivateTrigger();
}