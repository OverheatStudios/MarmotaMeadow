using UnityEngine;

public class ShootScript : MonoBehaviour
{
    [SerializeField] private float m_cooldown = 0.4f;
    private float m_currentCooldown = 0;
    [SerializeField] private GameObject m_bulletPrefab;
    [SerializeField] private GameObject m_camera;
    [SerializeField] private float m_bulletSpeed = 15;

    void Start()
    {

    }

    void Update()
    {
        if (m_currentCooldown >= 0)
        {
            m_currentCooldown -= Time.deltaTime;
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            GameObject bullet = Instantiate(m_bulletPrefab);
            bullet.transform.position = transform.position;
            bullet.transform.rotation = m_camera.transform.rotation;
            bullet.transform.rotation *= Quaternion.Euler(0, 90, 0);
            BulletScript bulletScript = bullet.GetComponent<BulletScript>();
            bulletScript.m_velocity = m_camera.transform.forward * m_bulletSpeed;
            m_currentCooldown = m_cooldown;
        }
    }
}
