using UnityEngine;
using UnityEngine.Assertions;

public class GroundhogSpawnerScript : MonoBehaviour
{
    [SerializeField] private GameObject m_prefab;
    private float m_timeToSpawn = 2;

    void Start()
    {
        Assert.IsNotNull(m_prefab);
    }

    void Update()
    {
        m_timeToSpawn -= Time.deltaTime;
        if (m_timeToSpawn > 0) return;

        m_timeToSpawn = Random.Range(0.5f, 1.5f);

        Vector3 position = new Vector3(Random.Range(-4, 4), 0, Random.Range(-4, 1));
        GameObject groundhog = Instantiate(m_prefab);
        groundhog.transform.position = position + transform.position;
    }
}
