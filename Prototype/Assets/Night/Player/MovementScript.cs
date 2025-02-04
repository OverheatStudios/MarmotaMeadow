using UnityEngine;
using UnityEngine.Assertions;

public class MovementScript : MonoBehaviour
{
    [SerializeField] private GameObject m_camera;
    [SerializeField] private float m_speed = 8.0f;
    [SerializeField] private Rigidbody m_rigidbody;

    void Start()
    {
        Assert.IsNotNull(m_camera);
    }

    void Update()
    {
        Vector3 movement = Vector2.zero;
        if (Input.GetKey(KeyCode.W))
            movement.x += 1;
        if (Input.GetKey(KeyCode.A))
            movement.z -= 1;
        if (Input.GetKey(KeyCode.S))
            movement.x -= 1;
        if (Input.GetKey(KeyCode.D))
            movement.z += 1;
        
        

        movement = movement.normalized * Time.deltaTime * m_speed;
        

        Vector3 forward = m_camera.transform.forward;
        forward.y = 0;
        forward = forward.normalized;

        Vector3 right = m_camera.transform.right;
        right.y = 0;
        right = right.normalized;
        
        m_rigidbody.velocity = forward * movement.x + right * movement.z;

        //transform.position += forward * movement.x;
        //transform.position += right * movement.z;
    }
}
