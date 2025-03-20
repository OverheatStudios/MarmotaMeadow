using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundhogProjectile : MonoBehaviour
{
    [SerializeField] private LayerMask m_playerLayer;
    [SerializeField] private ScrObjGlobalData m_data;
    [SerializeField] private float m_lifespanSeconds = 5.0f;

    public float Damage = 1;
    private float m_timeSinceSpawn = 0;

    private void Update()
    {
        m_timeSinceSpawn += Time.deltaTime;
        if (m_timeSinceSpawn >= m_lifespanSeconds)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & m_playerLayer) != 0)
        {
            Destroy(gameObject);
            m_data.Damage((int)Damage);
        }
    }
}
