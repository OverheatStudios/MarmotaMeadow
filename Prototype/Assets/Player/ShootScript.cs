using UnityEngine;
using UnityEngine.Assertions;

public class ShootScript : MonoBehaviour
{
    [SerializeField] private float m_cooldown = 0.4f;
    private float m_currentCooldown = 0;
    [SerializeField] private LayerMask m_enemyLayer;
    [SerializeField] private GameObject m_camera;
    [SerializeField] private float m_maxDistance = 100.0f;
    [SerializeField] private DataScriptableObject m_data;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Assert.IsNotNull(m_camera);
        Assert.IsNotNull(m_data);
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
            ShootBullet();
            m_currentCooldown = m_cooldown;
        }
    }

    void ShootBullet()
    {
        Ray ray = new Ray(m_camera.transform.position, m_camera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, m_maxDistance, m_enemyLayer))
        {
            GroundhogScript groundhogScript = hit.collider.gameObject.GetComponent<GroundhogScript>();
            Assert.IsNotNull(groundhogScript);
            KillGroundhog(groundhogScript);
        }
    }

    void KillGroundhog(GroundhogScript groundhog)
    {
        Destroy(groundhog.gameObject);
        m_data.groundhogsKilled++;
    }
}
