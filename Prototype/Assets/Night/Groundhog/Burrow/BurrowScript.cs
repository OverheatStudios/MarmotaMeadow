using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurrowScript : MonoBehaviour
{
    [SerializeField] private GameObject m_groundhogPrefab;
    private GameObject m_groundhog;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Spawn()
    {
        m_groundhog = Instantiate(m_groundhogPrefab);
        m_groundhog.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

    public bool IsEmpty()
    {
        return m_groundhog == null;
    }
}
