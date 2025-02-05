using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Actions : MonoBehaviour
{
    [SerializeField] private BaseItem[] items;
    [SerializeField] private int selectedItemIndex;
    [SerializeField] private GameObject camera;
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask plantLayerMask;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            selectedItemIndex++;
        }else if (Input.GetKeyDown(KeyCode.Q))
        {
            selectedItemIndex--;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit hit;
            Ray ray = new Ray(camera.transform.position, camera.transform.forward);
            if (Physics.Raycast(ray, out hit, maxDistance, plantLayerMask))
            {
                if (hit.collider.CompareTag("Plant"))
                {
                    hit.collider.GetComponent<Plant>().ChangeState(items[selectedItemIndex]);
                }
            }
        }
    }
}
