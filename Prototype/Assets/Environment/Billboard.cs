using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_image1;
    [SerializeField] private SpriteRenderer m_image2;
    [SerializeField] private Sprite m_sprite;

    private void Start()
    {
        m_image1.sprite = m_sprite;
        m_image2.sprite = m_sprite;
    }
}
