using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInteraction : MonoBehaviour
{
    [SerializeField] private ShowUI showUI;
    private void OnMouseOver()
    {
        showUI.InteractedWithUI();
    }

    private void OnMouseExit()
    {
        showUI.StopInteractionWithUI();
    }
}
