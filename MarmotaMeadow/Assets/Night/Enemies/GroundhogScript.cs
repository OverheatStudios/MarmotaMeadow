using UnityEngine;
using UnityEngine.Assertions;

public class GroundhogScript : MonoBehaviour
{
    public enum GroundhogState
    {
        Idle, // At max height, not moving
        Rising, // Just spawned, on way up
        Falling // Going down, about to disappear
    }

    [SerializeField, Tooltip("Speed at which the groundhog moves up/down")]
    private float m_speed = 1.0f;

    [SerializeField, Tooltip("Game data")]
    private ScrObjGlobalData m_data;

    [SerializeField, Tooltip("Default maximum health, may be changed by night number or other means")]
    private float m_maxHealth = 15;

    [SerializeField, Tooltip("Health bar")]
    private ProgressBarScript m_healthBar;

    [SerializeField, Tooltip("Minimum seconds that groundhog will be fully up for (maximum is this value + 1)")]
    private float m_minUptime = 2.25f;

    [SerializeField]
    private GroundhogEscapeScriptableObject m_escapeObj;

    [SerializeField, Tooltip("High precision collider of the groundhog, probably should be a mesh collider. This will be disabled most of the time and require enabling before use.")]
    private Collider m_highPrecisionCollider;

    [SerializeField] 
    private SettingsScriptableObject m_settings;

    private CoinManager m_coinManager;

    private GroundhogState m_state = GroundhogState.Rising;
    /// <summary>
    /// Seconds remaining that the groundhog will stay idle
    /// </summary>
    private float m_upTime;
    private const float MAX_Y = 0.9f;
    private float m_health;
    private GroundhogTypeInfo m_typeInfo;

    void Awake()
    {
        m_upTime = Random.Range(0, 1) + m_minUptime;
        m_data.GetData().GroundhogsSpawned++;
        Assert.IsTrue(m_maxHealth > 0);
        m_health = m_maxHealth;

        // Update health bar
        m_healthBar.SetProgress(m_health, m_maxHealth, 0);
    }

    public void SetCoinManager(CoinManager coinManager)
    {
        m_coinManager = coinManager;
    }

    private void Start()
    {
        m_typeInfo = GetComponentInChildren<GroundhogTypeInfo>();
        m_highPrecisionCollider = m_typeInfo.GetHighPrecisionCollider();

        SetMaxHealth(m_maxHealth * m_typeInfo.GetHealthScalar() * m_settings.GetSettings().GetDifficulty());
        SetState(GroundhogState.Rising);
    }

    void Update()
    {
        if (m_data.m_isSettingsOpen) return;

        // Go up until reach MAX_Y
        if (m_state == GroundhogState.Rising)
        {
            transform.position += m_speed * Time.deltaTime * Vector3.up;
            if (transform.position.y >= MAX_Y)
            {
                // Reached max Y
                transform.position = new Vector3(transform.position.x, MAX_Y, transform.position.z);
                SetState(GroundhogState.Idle);
            }
            else if (transform.position.y < m_typeInfo.GetMinYBeforeDisappear())
            {
                // Somehow below minimum height (probably just spawned in)
                transform.position = new Vector3(transform.position.x, m_typeInfo.GetMinYBeforeDisappear(), transform.position.z);
            }
        }
        // Do nothing
        else if (m_state == GroundhogState.Idle)
        {
            m_upTime -= Time.deltaTime;
            if (m_upTime < 0)
            {
                SetState(GroundhogState.Falling);
            }
        }
        // Go down and destroy self
        else
        {
            transform.position += m_speed * Time.deltaTime * Vector3.down;
            if (transform.position.y <= m_typeInfo.GetMinYBeforeDisappear())
            {
                m_escapeObj.NotifyGroundhogEscaped();
                Destroy(gameObject);
                return;
            }
        }
    }

    private void SetState(GroundhogState state)
    {
        m_state = state;
        m_typeInfo.CurrentState = state;
    }

    /// <summary>
    /// Damage the groundhog, killing it if the new health is <= 0
    /// </summary>
    /// <param name="damage">Amount to damage by</param>
    /// <param name="bulletWorldPosition">World position of where the bullet hit the groundhog, set to Vector3.zero if not using</param>
    public void Damage(float damage, Vector3 bulletWorldPosition)
    {
        m_health -= damage;
        if (damage > 0 && bulletWorldPosition != Vector3.zero)
        {
            m_typeInfo.PlayWoundedSfx(transform.position);
        }

        if (bulletWorldPosition != Vector3.zero)
        {
            GameObject ps = Instantiate(m_typeInfo.GetBloodParticle());
            ps.transform.position = bulletWorldPosition + m_typeInfo.GetBloodParticleOffset();
        }

        // Update health bar
        m_healthBar.SetProgress(m_health, m_maxHealth, 0);

        if (m_health <= 0)
        {
            m_data.GetData().GroundhogsKilled++;
            m_coinManager.IncreaseCoins(m_typeInfo.GetCoinsDropped());
            Destroy(gameObject, 0.01f);
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
        Damage(m_maxHealth - maxHealth, Vector3.zero); // May heal if the new max health is greater
        m_maxHealth = maxHealth;

        // Update health bar
        m_healthBar.SetProgress(m_health, m_maxHealth, 0);
    }

    public float GetMaxHealth()
    {
        return m_maxHealth;
    }

    public bool IsAlive()
    {
        return m_health > 0;
    }

    public Collider GetHighPrecisionCollider()
    {
        return m_highPrecisionCollider;
    }

    public void EnableHighPrecisionCollider()
    {
        m_highPrecisionCollider.enabled = true;
    }

    public void DisableHighPrecisionCollider()
    {
        m_highPrecisionCollider.enabled = false;
    }
}
