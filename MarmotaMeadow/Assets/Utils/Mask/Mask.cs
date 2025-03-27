using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://www.youtube.com/watch?v=Blits1yymCw
public class Mask : MonoBehaviour
{
    [SerializeField] private GameObject[] m_objectsThatAreMasked;

    public static Mask AddScriptToObject(GameObject target, GameObject[] objectsToMask)
    {
        Mask mask = target.AddComponent<Mask>(); 
        mask.m_objectsThatAreMasked = objectsToMask; 
        return mask;
    }

    void Start()
    {
        foreach (GameObject obj in m_objectsThatAreMasked)
        {
            if (obj.TryGetComponent<MeshRenderer>(out var mesh))
            {
                mesh.material.renderQueue = 3002;
            }
        }
    }
}
