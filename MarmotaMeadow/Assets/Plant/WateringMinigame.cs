using UnityEngine;

public class WateringMinigame : MonoBehaviour
{

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
    private Vector3 pointN;
    private Vector3 pointO;
    private Vector3 pointP;

    [SerializeField] private GameObject colliders;
    [SerializeField] private float m_maxDistance;
    [SerializeField] private LayerMask checkMask;
    [SerializeField] private GameObject trail;

    [SerializeField] private Plant plant;

    [SerializeField] private bool finished = false;
    [SerializeField] private AudioClip soundEffect;
    [SerializeField] private bool onSpot;

    private void OnEnable()
    {
        finished = false;
        GenerateRandomPoints();
        AddColliders();
        
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 1f;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        trail.transform.position = worldPosition;
    }

    public void IsOnSpot(bool isOnSpot)
    {
        onSpot = isOnSpot;
    }
    
    private void OnDisable()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        } 
    }

    void Update()
    {
        if (!finished & onSpot)
            CheckForCollisions();
    }
    
    void GenerateRandomPoints()
    {

        pointA = new Vector3(transform.position.x - 0.164f, transform.position.y + 0.27f, transform.position.z);
        pointB = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
        pointC = new Vector3(transform.position.x + 0.164f, transform.position.y + 0.27f, transform.position.z);
        pointD = new Vector3(transform.position.x + 0.3f, transform.position.y, transform.position.z);
        pointI = new Vector3(transform.position.x - 0.269f, transform.position.y + 0.145f, transform.position.z);
        pointK = new Vector3(transform.position.x + 0.269f, transform.position.y + 0.145f, transform.position.z);
        //pointM = new Vector3(transform.position.x - 0.39f, transform.position.y + 0.15f, transform.position.z);
        pointN = new Vector3(transform.position.x + 0.39f, transform.position.y + 0.15f, transform.position.z);
        
        
        pointE = transform.position * 2 - pointD;
        //pointF = transform.position * 2 - pointB;
        pointG = transform.position * 2 - pointC;
        pointH = transform.position * 2 - pointA;
        pointJ = transform.position * 2 - pointI;
        pointL = transform.position * 2 - pointK;
    }

    void AddColliders()
    {
        GameObject A = Instantiate(colliders, pointA, Quaternion.identity);
        GameObject B = Instantiate(colliders, pointB, Quaternion.identity);
        GameObject C = Instantiate(colliders, pointC, Quaternion.identity);
        GameObject D = Instantiate(colliders, pointD, Quaternion.identity);
        GameObject E = Instantiate(colliders, pointE, Quaternion.identity);
        GameObject G = Instantiate(colliders, pointG, Quaternion.identity);
        GameObject H = Instantiate(colliders, pointH, Quaternion.identity);
        GameObject I = Instantiate(colliders, pointI, Quaternion.identity);
        GameObject J = Instantiate(colliders, pointJ, Quaternion.identity);
        GameObject K = Instantiate(colliders, pointK, Quaternion.identity);
        GameObject L = Instantiate(colliders, pointL, Quaternion.identity);

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
    }

    void CheckForCollisions()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 1f; // Set the object's z position to a defined depth

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        trail.transform.position = worldPosition;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, m_maxDistance, checkMask))
        {
            Destroy(hit.collider.gameObject);
            AudioSource.PlayClipAtPoint(soundEffect, transform.position);

            Debug.DrawRay(ray.origin, ray.direction * m_maxDistance, Color.red, 2f);
        }

        if (transform.childCount == 0)
        {
            plant.WaterCrop();
            finished = true;
        }
    }
}


