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
            if (transform.position.y >= 1)
            {
                transform.position = new Vector3(transform.position.x, 1, transform.position.z);
                m_state = State.Idle;
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
            if (transform.position.y <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
