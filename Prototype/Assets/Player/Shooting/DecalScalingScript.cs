using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DecalScalingScript : MonoBehaviour
{
    /// <summary>
    /// The decal projector to scale
    /// </summary>
    [SerializeField] private DecalProjector m_decalProjector;
    /// <summary>
    /// The transform to use, only local scale is used (parent transform scales aren't included)
    /// </summary>
    [SerializeField] private Transform m_scalar;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_decalProjector.size = m_scalar.localScale;
        print(m_scalar.transform.localScale);
    }
}
