using NUnit.Framework;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class ShootScript : MonoBehaviour
{
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
    [UnityEngine.Range(0, 1)][SerializeField] private float m_randomDamageScale = 0.2f;

    /// <summary>
    /// Bullet hole decal
    /// </summary>
    [SerializeField] private GameObject m_bulletHolePrefab;

    /// <summary>
    /// Layer which anything that can have a bullet hole decal applied, should not include enemies
    /// </summary>
    [SerializeField] private LayerMask m_shootablesLayer;

    /// <summary>
    /// Move the bullet hole decal along the normal of the surface this many units
    /// </summary>
    [SerializeField] private float m_bulletHoleOffset = 0.5f;

    [SerializeField] private InventoryMager m_inventoryManager;

    private const float MAX_RAY_DISTANCE = 100f;

    private float m_currentCooldown = 0;
    private float m_lastCooldownSet = 0;

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
            Assert.IsTrue(gun.GetNumBullets() >= 1);
            SetAmmo(GetGunUnsafe().GetCurrentAmmo() - 1);
            for (int i = 0; i < gun.GetNumBullets(); ++i)
            {
                ShootBullet();
            }
            return;
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
        bulletHole.GetComponentInChildren<DecalProjector>().size = new Vector3(m_data.BulletHoleSize, m_data.BulletHoleSize, m_data.BulletHoleSize);
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
