using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildBoard : MonoBehaviour
{
    private Camera mainCamera;

    private void Start()
    {
        // Cache the main camera for better performance
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // Make the UI face the camera
        if (mainCamera)
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
        }
    }
}
