using UnityEngine;
using UnityEngine.Assertions;

public class GroundhogScript : MonoBehaviour
{
    enum State
    {
        Idle, // At max height, not moving
        Rising, // Just spawned, on way up
        Falling // Going down, about to disappear
    }

    /// <summary>
    /// Speed at which the groundhog moves up/down
    /// </summary>
    [SerializeField] private float m_speed = 1.0f;

    /// <summary>
    /// Game data
    /// </summary>
    [SerializeField] private DataScriptableObject m_data;

    /// <summary>
    /// Default maximum health, may be changed by night number or other means
    /// </summary>
    [SerializeField] private float m_maxHealth = 15;

    /// <summary>
    /// Health bar
    /// </summary>
    [SerializeField] private ProgressBarScript m_healthBar;

    /// <summary>
    /// Minimum seconds that groundhog will be fully up for (maximum is this value + 1)
    /// </summary>
    [SerializeField] private float m_minUptime = 2.25f;

    [SerializeField] private GroundhogEscapeScriptableObject m_escapeObj;

    private State m_state = State.Rising;
    /// <summary>
    /// Seconds remaining that the groundhog will stay idle
    /// </summary>
    private float m_upTime;
    private const float MAX_Y = 0.9f;
    private const float MIN_Y = 0.1f;
    private float m_health;

    void Awake()
    {
        m_upTime = Random.Range(0, 1) + m_minUptime;
        m_data.GroundhogsSpawned++;
        Assert.IsTrue(m_maxHealth > 0);
        m_health = m_maxHealth;

        // Update health bar
        m_healthBar.SetProgress(m_health, m_maxHealth, 0);
    }

    void Update()
    {
        // Go up until reach MAX_Y
        if (m_state == State.Rising)
        {
            transform.position += m_speed * Time.deltaTime * Vector3.up;
            if (transform.position.y >= MAX_Y)
            {
                // Reached max Y
                transform.position = new Vector3(transform.position.x, MAX_Y, transform.position.z);
                m_state = State.Idle;
            } else if (transform.position.y < MIN_Y)
            {
                // Somehow below minimum height (probably just spawned in)
                transform.position = new Vector3(transform.position.x, MIN_Y, transform.position.z);
            }
        }
        // Do nothing
        else if (m_state == State.Idle)
        {
            m_upTime -= Time.deltaTime;
            if (m_upTime < 0)
            {
                m_state = State.Falling;
            }
        }
        // Go down and destroy self
        else
        {
            transform.position += m_speed * Time.deltaTime * Vector3.down;
            if (transform.position.y <= MIN_Y)
            {
                m_escapeObj.NotifyGroundhogEscaped();
                Destroy(gameObject);
                return;
            }
        }
    }

    /// <summary>
    /// Damage the groundhog, killing it if the new health is <= 0
    /// </summary>
    /// <param name="damage">Amount to damage by</param>
    public void Damage(float damage)
    {
        m_health -= damage;

        // Update health bar
        m_healthBar.SetProgress(m_health, m_maxHealth, 0);

        if (m_health <= 0)
        {
            m_data.GroundhogsKilled++;
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// Set the maximum health of the groundhog, this will modify the health too (damaged if max health lowered, healed if max health increased)
    /// This may kill the groundhog
    /// </summary>
    /// <param name="maxHealth">Max health, must be greater than 0</param>
    public void SetMaxHealth(float maxHealth)
    {
        Assert.IsTrue(m_maxHealth > 0);
        Damage(m_maxHealth - maxHealth); // May heal if the new max health is greater
        m_maxHealth = maxHealth;

        // Update health bar
        m_healthBar.SetProgress(m_health, m_maxHealth, 0);
    }

    public float GetMaxHealth()
    {
        return m_maxHealth;
    }
}
