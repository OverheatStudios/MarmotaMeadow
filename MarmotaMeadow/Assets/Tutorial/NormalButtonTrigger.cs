using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalButtonTrigger : TriggerBase
{
    [SerializeField] private bool isActive;

    public override void ActivateTrigger()
    {
        isActive = true;
    }

    public void ButtonTrigger()
    {
        if (isActive)
        {
            CompleteTrigger();
            isActive = false;
        }
    }
}
