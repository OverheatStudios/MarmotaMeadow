using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShootScript : MonoBehaviour
{
    /// <summary>
    /// Shoot cooldown in secnods
    /// </summary>
    [SerializeField] private float m_cooldown = 0.4f;

    /// <summary>
    /// Reload cooldown in seconds
    /// </summary>
    [SerializeField] private float m_reloadCooldown = 2.5f;

    /// <summary>
    /// Layer which all enemies and only enemies are on
    /// </summary>
    [SerializeField] private LayerMask m_enemyLayer;

    /// <summary>
    /// Player camera
    /// </summary>
    [SerializeField] private GameObject m_camera;

    /// <summary>
    /// Game data
    /// </summary>
    [SerializeField] private DataScriptableObject m_data;

    /// <summary>
    /// Sound to play on shoot
    /// </summary>
    [SerializeField] private AudioSource m_shootSound;

    /// <summary>
    /// Shooting cooldown bar background
    /// </summary>
    [SerializeField] private Image m_cooldownBar;

    /// <summary>
    /// Shooting cooldown bar mask (progress)
    /// </summary>
    [SerializeField] private Image m_cooldownBarMask;

    /// <summary>
    /// Current and max ammo display
    /// </summary>
    [SerializeField] private TextMeshProUGUI m_ammoText;

    /// <summary>
    /// Text telling user how to reload
    /// </summary>
    [SerializeField] private TextMeshProUGUI m_reloadText;

    private const float MAX_RAY_DISTANCE = 100f;

    private float m_currentCooldown = 0;
    private float m_lastCooldownSet = 0;

    void Start()
    {
        SetAmmo(m_data.maxAmmo);
    }

    void Update()
    {
        // Cooldown
        if (m_currentCooldown >= 0)
        {
            m_currentCooldown -= Time.deltaTime;
            m_cooldownBar.enabled = true;
            m_cooldownBarMask.fillAmount = (m_currentCooldown / Mathf.Max(m_lastCooldownSet, 0));
            return;
        }

        m_cooldownBar.enabled = false;

        // Not on cooldown passed this point

        // Reloading
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadAmmo();
            return;
        }

        // Ammo
        if (m_data.currentAmmo < 1)
        {
            m_reloadText.enabled = true;
            return;
        }

        // Shoot
        if (Input.GetMouseButtonDown(0))
        {
            ShootBullet();
            return;
        }
    }

    private void SetAmmo(int ammo)
    {
        if (ammo < 0) ammo = 0;
        else if (ammo > m_data.maxAmmo) ammo = m_data.maxAmmo;

        m_data.currentAmmo = ammo;
        m_ammoText.text = new StringBuilder("Ammo: ")
            .Append(ammo)
            .Append("/")
            .Append(m_data.maxAmmo)
            .ToString();

        m_reloadText.enabled = false;
    }

    /// <summary>
    /// Sets cooldown, overwriting current cooldown
    /// </summary>
    /// <param name="shootCooldown">Cooldown in seconds</param>
    private void SetShootCooldown(float shootCooldown)
    {
        m_currentCooldown = shootCooldown;
        m_lastCooldownSet = shootCooldown;
    }

    /// <summary>
    /// Shoot a bullet and set cooldown, should not be called when on cooldown
    /// </summary>
    private void ShootBullet()
    {
        // Ammo
        SetAmmo(m_data.currentAmmo - 1);

        SetShootCooldown(m_cooldown);

        m_shootSound.Play();

        // See if bullet hit anything
        Ray ray = new Ray(m_camera.transform.position, m_camera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, MAX_RAY_DISTANCE, m_enemyLayer))
        {
            // Hit a groundhog, kill it
            GroundhogScript groundhogScript = hit.collider.gameObject.GetComponent<GroundhogScript>();
            KillGroundhog(groundhogScript);
        }
    }

    /// <summary>
    /// Kill a groundhog
    /// </summary>
    /// <param name="groundhog">Groundhog to kill</param>
    private void KillGroundhog(GroundhogScript groundhog)
    {
        Destroy(groundhog.gameObject);
        m_data.groundhogsKilled++;
    }

    /// <summary>
    /// Reload ammo unless at max ammo, set cooldown, should not be called when on cooldown
    /// </summary>
    async void ReloadAmmo()
    {
        if (m_data.currentAmmo >= m_data.maxAmmo) return;

        SetShootCooldown(m_reloadCooldown);
        await Task.Delay((int)(m_reloadCooldown * 1000));
        SetAmmo(m_data.maxAmmo);
    }
}
