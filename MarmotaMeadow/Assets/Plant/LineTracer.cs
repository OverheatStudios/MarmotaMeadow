using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class LineTracer : MonoBehaviour
{
    public LineRenderer lineRenderer;

    private Vector3 pointA;
    private Vector3 pointB;
    private Vector3 pointC;
    
    [SerializeField] private GameObject colliders;
    [SerializeField] private GameObject m_camera;
    [SerializeField] private float m_maxDistance;
    [SerializeField] private LayerMask checkMask;
    [SerializeField] private GameObject trail;

    void Start()
    {
        GenerateRandomPoints();
        DrawLine();
        AddColliders();
    }

    void Update()
    {
        CheckForCollisions();
    }

    void GenerateRandomPoints()
    {
        float rangeY = 1f;

        pointA = new Vector3(-1, Random.Range(-rangeY, rangeY), 0);
        pointB = pointA * -1;
        pointC = new Vector3((pointA.x + pointB.x) / 2, (pointA.y + pointB.y) / 2, 0);
    }

    void DrawLine()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, pointA);
        lineRenderer.SetPosition(1, pointB);
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
    }

    void CheckForCollisions()
    {
        if (GameInput.GetKeyCode("Interact").GetKeyDown())
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 5f; // Set the object's z position to a defined depth

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
                Destroy(gameObject);
            }
        }
    }
}
