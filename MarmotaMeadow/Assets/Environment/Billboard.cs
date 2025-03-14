using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_image1;
    [SerializeField] private SpriteRenderer m_image2;
    [SerializeField] private Sprite m_sprite;
    [SerializeField] private bool m_randomiseRotation = true;

    private void Start()
    {
        m_image1.sprite = m_sprite;
        m_image2.sprite = m_sprite;

        if (m_randomiseRotation)
        {
            var rot = transform.rotation.eulerAngles;
            rot.y = Random.Range(0f, 360f);
            transform.rotation = Quaternion.Euler(rot);
        }

        transform.position += m_image1.size.y / 2 * Vector3.up;
    }
}
