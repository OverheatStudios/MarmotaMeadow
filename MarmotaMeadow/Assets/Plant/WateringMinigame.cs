using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WateringMinigame : MonoBehaviour
{
    public LineRenderer lineRenderer;

    private Vector3 pointA;
    private Vector3 pointB;
    private Vector3 pointC;
    private Vector3 pointD;
    private Vector3 pointE;
    private Vector3 pointF;
    private Vector3 pointG;
    private Vector3 pointH;
    private Vector3 pointI;
    private Vector3 pointJ;
    private Vector3 pointK;
    private Vector3 pointL;
    private Vector3 pointM;
    
    [SerializeField] private GameObject colliders;
    [SerializeField] private float m_maxDistance;
    [SerializeField] private LayerMask checkMask;
    [SerializeField] private GameObject trail;
    
    
    [SerializeField] private float rangeY = 1f;
    [SerializeField] private Plant plant;
    
    [SerializeField] private bool finished = false;

    private void OnEnable()
    {
        finished = false;
        GenerateRandomPoints();
        DrawLine();
        AddColliders();
    }

    void Update()
    {
        if(!finished)
            CheckForCollisions();
    }

    void GenerateRandomPoints()
    {

        pointA = new Vector3(transform.position.x - 0.2f, transform.position.y + 0.25f, transform.position.z);
        pointB = new Vector3(transform.position.x + 0.2f, transform.position.y + 0.25f, transform.position.z);
        pointC = new Vector3(transform.position.x + 0.3f, transform.position.y, transform.position.z);
        pointD = transform.position * 2 - pointA;
        pointE = transform.position * 2 - pointB;
        pointF = transform.position * 2 - pointC;
        pointG = pointA;
        
        pointH = (pointA + pointB) / 2;
        pointI = (pointB + pointC) / 2;
        pointJ = (pointC + pointD) / 2;
        pointK = (pointD + pointE) / 2;
        pointL = (pointE + pointF) / 2;
        pointM = (pointF + pointG) / 2;
        pointM = (pointG + pointH) / 2;
        pointM = (pointH + pointI) / 2;
    }

    void DrawLine()
    {
        lineRenderer.positionCount = 7;
        lineRenderer.SetPosition(0, pointA);
        lineRenderer.SetPosition(1, pointB);
        lineRenderer.SetPosition(2, pointC);
        lineRenderer.SetPosition(3, pointD);
        lineRenderer.SetPosition(4, pointE);
        lineRenderer.SetPosition(5, pointF);
        lineRenderer.SetPosition(6, pointG);
    }

    void AddColliders()
    {
        GameObject A = Instantiate(colliders, pointA, Quaternion.identity);
        GameObject B = Instantiate(colliders, pointB, Quaternion.identity);
        GameObject C =Instantiate(colliders, pointC, Quaternion.identity);
        GameObject D = Instantiate(colliders, pointD, Quaternion.identity);
        GameObject E = Instantiate(colliders, pointE, Quaternion.identity);
        GameObject F = Instantiate(colliders, pointF, Quaternion.identity);
        GameObject G = Instantiate(colliders, pointG, Quaternion.identity);
        GameObject H = Instantiate(colliders, pointH, Quaternion.identity);
        GameObject I = Instantiate(colliders, pointI, Quaternion.identity);
        GameObject J = Instantiate(colliders, pointJ, Quaternion.identity);
        GameObject K = Instantiate(colliders, pointK, Quaternion.identity);
        GameObject L = Instantiate(colliders, pointL, Quaternion.identity);
        GameObject M = Instantiate(colliders, pointM, Quaternion.identity);
        
        A.transform.parent = transform;
        A.name = "A";
        B.transform.parent = transform;
        B.name = "B";
        C.transform.parent = transform;
        C.name = "C";
        D.transform.parent = transform;
        D.name = "D";
        E.transform.parent = transform;
        E.name = "E";
        F.transform.parent = transform;
        F.name = "F";
        G.transform.parent = transform;
        G.name = "G";
        H.transform.parent = transform;
        H.name = "H";
        I.transform.parent = transform;
        I.name = "I";
        J.transform.parent = transform;
        J.name = "J";
        K.transform.parent = transform;
        K.name = "K";
        L.transform.parent = transform;
        L.name = "L";
        M.transform.parent = transform;
        M.name = "M";
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
            Destroy(hit.collider.gameObject);
            Debug.DrawRay(ray.origin, ray.direction * m_maxDistance, Color.red, 2f);
            Debug.Log(hit.collider.name);
        }

        if (transform.childCount == 0)
        {
            lineRenderer.positionCount = 0;
            plant.WaterCrop();
            finished = true;
        }
    }
}


