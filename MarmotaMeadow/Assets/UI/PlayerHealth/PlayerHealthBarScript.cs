using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealthBarScript : MonoBehaviour
{
    [SerializeField] private DataScriptableObject m_data;
    [Tooltip("Sprites to use for hearts, 0 should be full, then 1 is next damaged, etc. Final should be empty.")]
    [SerializeField] private List<Sprite> m_heartSprites;
    /// <summary>
    /// UI Canvas
    /// </summary>
    [SerializeField] private Canvas m_canvas;

    /// <summary>
    /// How far apart are the heart images?
    /// </summary>
    [SerializeField] private float m_heartSpacing = 50;
    [SerializeField] private ScrObjGameOver m_gameOverReason;

    private List<Image> m_images = new();
    private float m_healthLastFrame = 0;
    private float m_maxHealthLastFrame = 0;

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsTrue(m_heartSprites.Count >= 2);
        GenerateNecessaryHearts();
    }

    // Update is called once per frame
    void Update()
    {
        ClampHealthValue();

        if (m_data.CurrentHealth <= 0)
        {
            // Player lost
            m_gameOverReason.GameOverReason = ScrObjGameOver.Reason.Died;
            SceneManager.LoadScene("GameOverScene", LoadSceneMode.Single);
            return;
        }

        if (m_data.CurrentHealth != m_healthLastFrame || m_data.MaxHealth != m_maxHealthLastFrame)
        {
            GenerateNecessaryHearts();
            m_healthLastFrame = m_data.CurrentHealth;
            m_maxHealthLastFrame = m_data.MaxHealth;
        }
    }

    public void SetHealth(int health)
    {
        if (health < 1) health = 1;
        if (health > m_data.MaxHealth) health = m_data.MaxHealth;
        m_data.CurrentHealth = health;
    }

    /// <summary>
    /// Ensure max and current healths are valid
    /// </summary>
    private void ClampHealthValue()
    {
        m_data.CurrentHealth = Mathf.Max(m_data.CurrentHealth, 0);
        m_data.MaxHealth = Mathf.Max(m_data.MaxHealth, 1);
        m_data.CurrentHealth = Mathf.Min(m_data.CurrentHealth, m_data.MaxHealth);
    }

    private void GenerateNecessaryHearts()
    {
        // Create hearts
        int healthPerHeart = (m_heartSprites.Count - 1);
        int requiredHearts = Mathf.CeilToInt((float)m_data.MaxHealth / (float)healthPerHeart);
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

        // Set damage states of hearts
        for (int i = 0; i < m_images.Count; ++i)
        {
            if (m_data.CurrentHealth >= healthPerHeart * (i + 1))
            {
                m_images[i].sprite = m_heartSprites[0]; // full
            }
            else if (m_data.CurrentHealth > healthPerHeart * i)
            {
                Assert.IsTrue(m_heartSprites.Count == 4); // not sure this line is correct for anything other than 4
                m_images[i].sprite = m_heartSprites[m_heartSprites.Count - (m_data.CurrentHealth - healthPerHeart * i) - 1]; // damaged
            }
            else
            {
                m_images[i].sprite = m_heartSprites[m_heartSprites.Count - 1]; // empty
            }
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
        image.sprite = m_heartSprites[0];

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
