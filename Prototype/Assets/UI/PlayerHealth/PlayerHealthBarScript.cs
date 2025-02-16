using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBarScript : MonoBehaviour
{
    [SerializeField] private DataScriptableObject m_data;
    /// <summary>
    /// Texture to use for half a heart
    /// </summary>
    [SerializeField] private Sprite m_halfTexture;
    /// <summary>
    /// Texture to use for a full heart
    /// </summary>
    [SerializeField] private Sprite m_fullTexture;
    /// <summary>
    /// UI Canvas
    /// </summary>
    [SerializeField] private Canvas m_canvas;

    /// <summary>
    /// How far apart are the heart images?
    /// </summary>
    [SerializeField] private float m_heartSpacing = 50;

    private List<Image> m_images = new();
    private float m_healthLastFrame = 0;
    private float m_maxHealthLastFrame = 0;

    // Start is called before the first frame update
    void Start()
    {
        GenerateNecessaryHearts();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            m_data.CurrentHealth--;
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            m_data.CurrentHealth++;
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            m_data.MaxHealth--;
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            m_data.MaxHealth++;
        }

        if (m_data.CurrentHealth != m_healthLastFrame || m_data.MaxHealth != m_maxHealthLastFrame)
        {
            GenerateNecessaryHearts();
            m_healthLastFrame = m_data.CurrentHealth;
            m_maxHealthLastFrame = m_data.MaxHealth;
        }
    }

    private void GenerateNecessaryHearts()
    {
        // Make any half hearts full hearts
        foreach (Image image in m_images)
        {
            image.sprite = m_fullTexture;
        }

        // Create hearts
        int requiredHearts = Mathf.CeilToInt(m_data.CurrentHealth / 2.0f);
        for (int i = m_images.Count; i < requiredHearts; i++)
        {
            CreateHealthImage();
        }

        // Remove any unneeded hearts
        while (m_images.Count > requiredHearts)
        {
            Destroy(m_images[m_images.Count - 1]);
            m_images.RemoveAt(m_images.Count - 1);
        }

        // Make last heart half if necessary
        if (m_images.Count > 0 && m_data.CurrentHealth % 2 != 0)
        {
            m_images[m_images.Count - 1].sprite = m_halfTexture;
        }
    }

    /// <summary>
    /// Calculates what the x coordinate of the next heart image should be based on the number of current heart images
    /// </summary>
    /// <returns>The next x coordinate</returns>
    private float GetNextHeartX()
    {
        return m_images.Count * m_heartSpacing * transform.localScale.x;
    }

    /// <summary>
    /// Create a health image with the correct position, adds to m_images list. Uses full texture by default.
    /// </summary>
    private void CreateHealthImage()
    {
        // Image
        GameObject obj = new GameObject();
        Image image = obj.AddComponent<Image>();
        image.sprite = m_fullTexture;

        // Transform
        Transform imageTransform = obj.transform;
        imageTransform.SetParent(m_canvas.transform, false);
        imageTransform.position = transform.position + new Vector3(GetNextHeartX(), 0, 1);
        imageTransform.localScale = transform.localScale;

        // Finished
        obj.name = string.Format("HealthIcon{0}", m_images.Count + 1);
        obj.SetActive(true);
        m_images.Add(image);
    }
}
