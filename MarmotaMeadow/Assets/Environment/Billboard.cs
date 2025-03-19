using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private MeshRenderer m_quad1;
    [SerializeField] private MeshRenderer m_quad2;
    [SerializeField] private MeshRenderer m_quad3;
    [SerializeField] private MeshRenderer m_quad4;
    [SerializeField] private Sprite m_sprite;
    [SerializeField] private bool m_randomiseRotation = true;
    [SerializeField] private Shader m_shader;

    private void Start()
    {
        Material material = CreateMaterialFromSprite();

        MeshRenderer[] quads = new MeshRenderer[4] { m_quad1, m_quad2, m_quad3, m_quad4 };

        foreach (var quad in quads)
        {
            quad.material = material;
            quad.transform.localScale = new Vector3(5, 5, 5);
        }

        if (m_randomiseRotation)
        {
            var rot = transform.rotation.eulerAngles;
            rot.y = Random.Range(0f, 360f);
            transform.rotation = Quaternion.Euler(rot);
        }

        transform.position += m_quad1.bounds.size.y / 2 * Vector3.up;
    }

    private Material CreateMaterialFromSprite()
    {
        Material mat = new(m_shader)
        {
            mainTexture = m_sprite.texture
        };
        mat.SetTexture("_MainTex", m_sprite.texture);
        mat.SetFloat("_Cutoff", 0.1f); 
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest; 
        return mat;
    }
}
