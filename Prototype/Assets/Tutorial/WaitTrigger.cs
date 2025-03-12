using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitTrigger : TriggerBase
{
    [SerializeField] private float waitTime;
    private float startTime = 0;
    public override void ActivateTrigger()
    {
        StartCoroutine(WaitForSeconds());
    }

    private IEnumerator WaitForSeconds()
    {
        while (startTime < waitTime)
        {
            startTime += Time.deltaTime;
            yield return null;
        }
        CompleteTrigger();
    }
}
