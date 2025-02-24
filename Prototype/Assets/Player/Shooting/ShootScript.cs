using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class ShootScript : MonoBehaviour
{
    [Tooltip("Layer which all enemies and only enemies are on")]
    [SerializeField] private LayerMask m_enemyLayer;

    [Tooltip("Player camera")]
    [SerializeField] private GameObject m_camera;

    [Tooltip("Game data")]
    [SerializeField] private DataScriptableObject m_data;

    [Tooltip("Sound to play on shoot")]
    [SerializeField] private AudioSource m_shootSound;

    [Tooltip("Shooting cooldown bar background")]
    [SerializeField] private Image m_cooldownBar;

    [Tooltip("Shooting cooldown bar mask (progress)")]
    [SerializeField] private Image m_cooldownBarMask;

    [Tooltip("Current and max ammo display")]
    [SerializeField] private TextMeshProUGUI m_ammoText;

    [Tooltip("Text telling user how to reload")]
    [SerializeField] private TextMeshProUGUI m_reloadText;

    [Tooltip("Randomness in damage, 0.1 means +-10%")]
    [UnityEngine.Range(0, 1)]
    [SerializeField] private float m_randomDamageScale = 0.2f;

    [Tooltip("Bullet hole decal")]
    [SerializeField] private GameObject m_bulletHolePrefab;

    [Tooltip("Layer which anything that can have a bullet hole decal applied, should not include enemies")]
    [SerializeField] private LayerMask m_shootablesLayer;

    [Tooltip("Move the bullet hole decal along the normal of the surface this many units")]
    [SerializeField] private float m_bulletHoleOffset = 0.5f;


    [SerializeField] private InventoryMager m_inventoryManager;

    private const float MAX_RAY_DISTANCE = 100f;

    private float m_currentCooldown = 0;
    private float m_lastCooldownSet = 0;
    private BaseItem m_heldItemLastSwap;
    private bool m_isInfiniteAmmoCheatEnabled = false;

    void Start()
    {

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

        // Not on cooldown after this point

        HandleSwapCooldown();

        // Is holding gun?
        if (m_inventoryManager.GetHeldItem() is not Gun gun)
        {
            HideGunUi();
            return;
        }

        SetAmmo(gun.GetCurrentAmmo());
        ShowGunUi();

        // Reloading
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadAmmo();
            return;
        }

        // Ammo
        if (gun.GetCurrentAmmo() <= 0)
        {
            m_reloadText.enabled = true;
            return;
        }

        // Shoot
        if (Input.GetMouseButtonDown(0))
        {
            ShootGun();
            return;
        }
    }

    /// <summary>
    /// Enable/disable infinite ammo (no ammo consumption)
    /// </summary>
    /// <param name="toggleUi">The debug ui toggle for infinite ammo</param>
    public void SetInfiniteAmmoCheat(Toggle toggleUi)
    {
        m_isInfiniteAmmoCheatEnabled = toggleUi.isOn;
    }

    /// <summary>
    /// Shoot the gun once, consuming 1 ammo (if not infinite), depending on the gun this may shoot multiple bullets
    /// </summary>
    private void ShootGun()
    {
        Gun gun = GetGunUnsafe();
        Assert.IsTrue(gun.GetNumBullets() >= 1);

        if (!m_isInfiniteAmmoCheatEnabled)
        {
            SetAmmo(GetGunUnsafe().GetCurrentAmmo() - 1);
        }
        for (int i = 0; i < gun.GetNumBullets(); ++i)
        {
            ShootBullet();
        }
    }

    /// <summary>
    /// Handle swap cooldown, player does not need to be holding a gun when this method is called.
    /// Checks if a swap occurred and handles it if it did
    /// </summary>
    private void HandleSwapCooldown()
    {
        if (m_inventoryManager.GetHeldItem() is not Gun gun)
        {
            m_heldItemLastSwap = null;
            return;
        }

        if (m_heldItemLastSwap != gun)
        {
            if (m_currentCooldown < gun.GetSwapCooldownSeconds())
            {
                SetShootCooldown(gun.GetSwapCooldownSeconds());
            }
            m_heldItemLastSwap = gun;
        }
    }

    private void HideGunUi()
    {
        m_reloadText.enabled = false;
        m_ammoText.enabled = false;
        foreach (Transform child in m_ammoText.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    private void ShowGunUi()
    {
        // Reload text will enable itself when needed
        m_ammoText.enabled = true;
        foreach (Transform child in m_ammoText.transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    private void SetAmmo(int ammo)
    {
        Gun gun = GetGunUnsafe();
        if (ammo < 0) ammo = 0;
        else if (ammo > gun.GetMaxAmmo()) ammo = gun.GetMaxAmmo();

        gun.SetCurrentAmmo(ammo);
        m_ammoText.text = new StringBuilder("")
            .Append(ammo)
            .Append("/")
            .Append(gun.GetMaxAmmo())
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
    /// Shoot a bullet and set cooldown, should not be called when on cooldown, this will not decrement ammo
    /// </summary>
    private void ShootBullet()
    {
        SetShootCooldown(GetGunUnsafe().GetShootCooldownSeconds());

        m_shootSound.Play();

        // See if bullet hit anything
        Ray ray = new Ray(m_camera.transform.position, GetRandomBulletDirection());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, MAX_RAY_DISTANCE, m_enemyLayer))
        {
            // Hit a groundhog, damage it
            GroundhogScript groundhogScript = hit.collider.gameObject.GetComponent<GroundhogScript>();
            DamageGroundhog(groundhogScript);
            if (groundhogScript.IsAlive())
            {
                // Cast ray against groundhogs mesh
                groundhogScript.EnableHighPrecisionCollider();
                if (groundhogScript.GetHighPrecisionCollider().Raycast(ray, out hit, MAX_RAY_DISTANCE))
                {
                    SpawnBulletHoleDecal(hit);
                }
                groundhogScript.DisableHighPrecisionCollider();
            }
        }
        else if (Physics.Raycast(ray, out hit, MAX_RAY_DISTANCE, m_shootablesLayer))
        {
            // Hit shootable
            SpawnBulletHoleDecal(hit);
        }
    }

    /// <summary>
    /// Get the direction a bullet would travel in if one was shot right now, player must be holding a gun if this method is called. Takes gun bullet spread into account.
    /// </summary>
    /// <returns>Normalised vector representing bullet spread</returns>
    private Vector3 GetRandomBulletDirection()
    {
        Gun gun = GetGunUnsafe();
        Vector3 baseDirection = m_camera.transform.forward;
        baseDirection.x += Random.Range(-gun.GetBulletSpread(), gun.GetBulletSpread());
        baseDirection.y += Random.Range(-gun.GetBulletSpread(), gun.GetBulletSpread());
        baseDirection.z += Random.Range(-gun.GetBulletSpread(), gun.GetBulletSpread());
        return baseDirection.normalized;
    }

    /// <summary>
    /// Spawns a bullet hole decal on the surface that the raycast intersected with
    /// </summary>
    /// <param name="hit">Raycast result</param>
    private void SpawnBulletHoleDecal(RaycastHit hit)
    {
        GameObject bulletHole = Instantiate(m_bulletHolePrefab);
        bulletHole.transform.position = hit.point + hit.normal * m_bulletHoleOffset;
        bulletHole.transform.forward = hit.normal;
        bulletHole.transform.SetParent(hit.transform, true);
        bulletHole.GetComponentInChildren<DecalProjector>().size = new Vector3(GetGunUnsafe().GetBulletHoleSize(), GetGunUnsafe().GetBulletHoleSize(), GetGunUnsafe().GetBulletHoleSize());
    }

    /// <summary>
    /// Damage a groundhog
    /// </summary>
    /// <param name="groundhog">Groundhog to damage</param>
    private void DamageGroundhog(GroundhogScript groundhog)
    {
        float baseDamage = GetGunUnsafe().GetDamage();
        groundhog.Damage(baseDamage * Random.Range(1.0f - m_randomDamageScale, 1.0f + m_randomDamageScale));
    }

    /// <summary>
    /// Reload ammo unless at max ammo, set cooldown, should not be called when on cooldown
    /// </summary>
    async void ReloadAmmo()
    {
        Gun gun = GetGunUnsafe();
        if (gun.GetCurrentAmmo() >= gun.GetMaxAmmo()) return;

        float reloadCooldown = GetGunUnsafe().GetReloadCooldownSeconds();
        SetShootCooldown(reloadCooldown);
        await Task.Delay((int)(reloadCooldown * 1000));
        SetAmmo(gun.GetMaxAmmo());
    }

    private Gun GetGunUnsafe()
    {
        BaseItem heldItem = m_inventoryManager.GetHeldItem();
        if (heldItem is Gun gun) return gun;
        Assert.IsTrue(false);
        return null;
    }
}
