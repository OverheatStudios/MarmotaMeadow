using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.Assertions;

public class GroundhogScript : MonoBehaviour
{
    enum State
    {
        Idle, // At max height, not moving
        Rising, // Just spawned, on way up
        Falling // Going down, about to disappear
    }

    /// <summary>
    /// Speed at which the groundhog moves up/down
    /// </summary>
    [SerializeField] private float m_speed = 1.0f;

    /// <summary>
    /// Game data
    /// </summary>
    [SerializeField] private DataScriptableObject m_data;

    private State m_state = State.Rising;
    /// <summary>
    /// Seconds remaining that the groundhog will stay idle
    /// </summary>
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
        // Go up until reach MAX_Y
        if (m_state == State.Rising)
        {
            transform.position += m_speed * Time.deltaTime * Vector3.up;
            if (transform.position.y >= MAX_Y)
            {
                // Reached max Y
                transform.position = new Vector3(transform.position.x, MAX_Y, transform.position.z);
                m_state = State.Idle;
            } else if (transform.position.y < MIN_Y)
            {
                // Somehow below minimum height (probably just spawned in)
                transform.position = new Vector3(transform.position.x, MIN_Y, transform.position.z);
            }
        }
        // Do nothing
        else if (m_state == State.Idle)
        {
            m_upTime -= Time.deltaTime;
            if (m_upTime < 0)
            {
                m_state = State.Falling;
            }
        }
        // Go down and destroy self
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
