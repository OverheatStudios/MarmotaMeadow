using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class HarvestingMinigame : MonoBehaviour
{

    private Vector3 pointA;
    private Vector3 pointB;
    private Vector3 pointC;
    
    [SerializeField] private GameObject colliders;
    [SerializeField] private float m_maxDistance;
    [SerializeField] private LayerMask checkMask;
    [SerializeField] private GameObject trail;
    
    
    [SerializeField] private float rangeY = 1f;
    [SerializeField] private Plant plant;
    
    [SerializeField] private bool finished = false;
    [SerializeField] private AudioClip soundEffect;

    private void OnEnable()
    {
        
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 1f;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        trail.transform.position = worldPosition;
        
        finished = false;
        GenerateRandomPoints();
        AddColliders();
    }

    private void OnDisable()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        } 
    }

    void Update()
    {
        if(!finished)
            CheckForCollisions();
    }

    void GenerateRandomPoints()
    {

        pointA = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        pointB = new Vector3(transform.position.x - 0.47f, transform.position.y + 0.47f, transform.position.z);
        pointC = transform.position * 2 - pointB;
    }

    void AddColliders()
    {
        GameObject A = Instantiate(colliders, pointA, Quaternion.identity);
        GameObject B = Instantiate(colliders, pointB, Quaternion.identity);
        GameObject C =Instantiate(colliders, pointC, Quaternion.identity);
        
        A.transform.parent = transform;
        A.name = "A";
        B.transform.parent = transform;
        B.name = "B";
        C.transform.parent = transform;
        C.name = "C";


        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 90));
    }

    void CheckForCollisions()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 1f; // Set the object's z position to a defined depth

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        trail.transform.position = worldPosition;
            
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit, m_maxDistance, checkMask))
        {
            if (hit.collider.gameObject.CompareTag("collider"))
            {
                AudioSource.PlayClipAtPoint(soundEffect, transform.position);
                Destroy(hit.collider.gameObject);
                Debug.DrawRay(ray.origin, ray.direction * m_maxDistance, Color.red, 2f);   
            }
        }

        if (transform.childCount == 0)
        {
            plant.HarvestCrop();
            finished = true;
        }
    } 
}

