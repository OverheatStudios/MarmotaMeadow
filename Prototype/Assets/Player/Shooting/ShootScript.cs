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

    /// <summary>
    /// Randomness in damage, 0.1 means +-10%
    /// </summary>
    [Range(0, 1)][SerializeField] private float m_randomDamageScale = 0.2f;

    /// <summary>
    /// Bullet hole decal
    /// </summary>
    [SerializeField] private GameObject m_bulletHolePrefab;

    /// <summary>
    /// Layer which anything that can have a bullet hole decal applied, should not include enemies
    /// </summary>
    [SerializeField] private LayerMask m_shootablesLayer;

    /// <summary>
    /// To prevent z-fighting, move the bullet hole decal along the normal of the surface this many units
    /// </summary>
    [SerializeField] private float m_bulletHoleOffset = 0.01f;

    private const float MAX_RAY_DISTANCE = 100f;

    private float m_currentCooldown = 0;
    private float m_lastCooldownSet = 0;

    void Start()
    {
        SetAmmo(m_data.MaxAmmo);
    }

    void Update()
    {
        // Cooldown
        if (m_currentCooldown >= 0)
        {
            m_currentCooldown -= Time.deltaTime;
            m_cooldownBar.enabled = true;
            m_reloadText.enabled = false;
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
        if (m_data.CurrentAmmo < 1)
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
        else if (ammo > m_data.MaxAmmo) ammo = m_data.MaxAmmo;

        m_data.CurrentAmmo = ammo;
        m_ammoText.text = new StringBuilder("")
            .Append(ammo)
            .Append("/")
            .Append(m_data.MaxAmmo)
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
        SetAmmo(m_data.CurrentAmmo - 1);

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
        else if (Physics.Raycast(ray, out hit, MAX_RAY_DISTANCE, m_shootablesLayer))
        {
            // Hit shootable
            SpawnBulletHoleDecal(hit);
        }
    }

    /// <summary>
    /// Spawns a bullet hole decal on the surface that the raycast intersected with
    /// </summary>
    /// <param name="hit">Raycast result</param>
    private void SpawnBulletHoleDecal(RaycastHit hit)
    {
        GameObject bulletHole = Instantiate(m_bulletHolePrefab);
        bulletHole.transform.localScale = new Vector3(m_data.BulletHoleSize, m_data.BulletHoleSize, m_data.BulletHoleSize);
        bulletHole.transform.position = hit.point + hit.normal * m_bulletHoleOffset;
        bulletHole.transform.forward = hit.normal;
        bulletHole.transform.SetParent(hit.transform, true);
    }

    /// <summary>
    /// Kill a groundhog
    /// </summary>
    /// <param name="groundhog">Groundhog to kill</param>
    private void KillGroundhog(GroundhogScript groundhog)
    {
        groundhog.Damage(m_data.Damage * Random.Range(1.0f - m_randomDamageScale, 1.0f + m_randomDamageScale));
    }

    /// <summary>
    /// Reload ammo unless at max ammo, set cooldown, should not be called when on cooldown
    /// </summary>
    async void ReloadAmmo()
    {
        if (m_data.CurrentAmmo >= m_data.MaxAmmo) return;

        SetShootCooldown(m_reloadCooldown);
        await Task.Delay((int)(m_reloadCooldown * 1000));
        SetAmmo(m_data.MaxAmmo);
    }
}
