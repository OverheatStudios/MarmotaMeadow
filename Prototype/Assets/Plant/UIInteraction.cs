using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInteraction : MonoBehaviour
{
    private void OnMouseOver()
    {
        gameObject.GetComponentInParent<ShowUI>().InteractedWithUI();
    }

    private void OnMouseExit()
    {
        gameObject.GetComponentInParent<ShowUI>().StopInteractionWithUI();
    }
}
