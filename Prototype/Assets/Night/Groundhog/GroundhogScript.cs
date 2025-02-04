using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.Assertions;

public class GroundhogScript : MonoBehaviour
{
    enum State
    {
        Idle, Rising, Falling
    }

    [SerializeField] private float m_speed = 1.0f;
    [SerializeField] private DataScriptableObject m_data;
    private State m_state = State.Rising;
    private float m_upTime;
    private const float MAX_Y = 0.9f;
    private const float MIN_Y = 0.1f;

    void Start()
    {
        m_upTime = Random.Range(1.5f, 2.5f);
        Assert.IsNotNull(m_data);
        m_data.groundhogsSpawned++;
    }

    void Update()
    {
        if (m_state == State.Rising)
        {
            transform.position += m_speed * Time.deltaTime * Vector3.up;
            if (transform.position.y >= MAX_Y)
            {
                transform.position = new Vector3(transform.position.x, MAX_Y, transform.position.z);
                m_state = State.Idle;
            } else if (transform.position.y < MIN_Y)
            {
                transform.position = new Vector3(transform.position.x, MIN_Y, transform.position.z);
            }
        }
        else if (m_state == State.Idle)
        {
            m_upTime -= Time.deltaTime;
            if (m_upTime < 0)
            {
                m_state = State.Falling;
            }
        }
        else
        {
            transform.position += m_speed * Time.deltaTime * Vector3.down;
            if (transform.position.y <= MIN_Y)
            {
                Destroy(gameObject);
            }
        }
    }
}
