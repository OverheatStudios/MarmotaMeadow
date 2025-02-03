using UnityEngine;
using UnityEngine.Assertions;

public class BulletScript : MonoBehaviour
{
    [SerializeField] private float m_lifespan = 2;
    private float m_timeAlive = 0;
    public Vector3 m_velocity = Vector3.zero;
    [SerializeField] private Collider m_collider;
    [SerializeField] private DataScriptableObject m_data;

    void Start()
    {
        Assert.IsNotNull(m_collider);
        Assert.IsNotNull(m_data);
    }

    void Update()
    {
        m_timeAlive += Time.deltaTime;
        if (m_timeAlive > m_lifespan)
        {
            Destroy(gameObject);
            return;
        }

        transform.position += m_velocity * Time.deltaTime;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player" || other.gameObject.name == "Balcony")
        {
            return;
        }
        Destroy(gameObject);
        if (other.gameObject.name.Contains("Groundhog"))
        {
            Destroy(other.gameObject);
            m_data.groundhogsKilled++;
        }
    }
}
