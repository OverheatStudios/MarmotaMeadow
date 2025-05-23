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
    [SerializeField] private ScrObjGlobalData m_data;

    [Tooltip("Shooting cooldown bar")]
    [SerializeField] private ProgressBar m_cooldownBar;

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

    [Tooltip("Text that should be displayed if user needs to reload but doesn't have any ammo packs in inventory and also the gun requires ammo packs")]
    [SerializeField] private TextMeshProUGUI m_noBulletsForReloadText;

    [SerializeField] private InventoryMager m_inventoryManager;

    [SerializeField] private CursorHandlerScript m_cursorHandler;

    [SerializeField] private CameraScript m_cameraScript;

    [SerializeField] private CameraShake m_cameraShake;

    [SerializeField] private Transform m_canvas;
    [SerializeField] private MovementScript m_movementScript;
    [SerializeField] private SettingsScriptableObject m_settings;
    [SerializeField] private GunUpgrades m_gunUpgrades;
    [SerializeField] private float m_shootVolume = 0.4f;

    [SerializeField] private Vector3 m_actionTooltipOffset = Vector3.up * 0.25f;
    [SerializeField] private GameObject m_shootBadActionTooltip;
    [SerializeField] private GameObject m_shootGoodActionTooltip;
    [SerializeField] private float m_actionTooltipInterval = 0.8f;

    private ReloadAnimation m_reloadBar;

    private const float MAX_RAY_DISTANCE = 100f;

    private float m_currentCooldown = 0;
    private float m_lastCooldownSet = 0;
    private BaseItem m_heldItemLastSwap;
    private bool m_isInfiniteAmmoCheatEnabled = false;
    private Bullet m_bulletForHeldGun = null; // possibly null
    private bool m_noBulletsToReload = false;
    private float[] m_shootSoundTimestamps = new float[3];
    [Tooltip("3 sounds per interval seconds")]
    [SerializeField] private float m_shootSoundInterval = 0.4f;
    private int m_currentShootSoundIndex = 0;
    private float m_secondsSinceLastActionTooltipShowed = 1.0f;
    
    [Header("Animation")]
    [SerializeField] private Animator m_animator;

    void Start()
    {
        HideReloadUi();
        m_cooldownBar.SetVisible(false);

        for (int i = 0; i < m_shootSoundTimestamps.Length; i++)
        {
            m_shootSoundTimestamps[i] = 0;
        }
    }

    void Update()
    {
        m_secondsSinceLastActionTooltipShowed += Time.deltaTime;

        m_reloadText.text = "Press " + GameInput.GetKeybind("Reload").ToString() + " to reload";

        // Is holding gun?
        if (m_inventoryManager.GetHeldItem() is not Gun gun)
        {
            HideGunUi();
            HandleCooldown();
            m_reloadBar = null;
            return;
        }
        else
        {
            ///m_animator = GameObject.FindObjectOfType<Animator>();
        }

        // Cooldown
        if (HandleCooldown()) return;

        // Not on cooldown after this point
        HandleGunSwap();

        SetAmmo(gun.GetCurrentAmmo());
        ShowGunUi();
        if (m_cursorHandler.IsUiOpen()) return;

        // Reloading
        if (GameInput.GetKeybind("Reload").GetKeyDown())
        {
            AttemptAmmoReload();
            return;
        }

        // Ammo
        if (gun.GetCurrentAmmo() <= 0)
        {
            ShowReloadUi();
            return;
        }

        // Shoot
        if (GameInput.GetKeybind("Interact").GetKeyDown())
        {
            if (!m_movementScript.IsCrouching())
            {
                ShootGun();
            }
            return;
        }
    }

    private void SetupReloadBar()
    {
        m_reloadBar = Instantiate(GetGunUnsafe().GetReloadBarPrefab()).GetComponent<ReloadAnimation>();
        m_reloadBar.name = "ReloadBar";
        m_reloadBar.transform.SetParent(m_canvas.transform, false);
    }

    /// <summary>
    /// Handle cooldown logic
    /// </summary>
    /// <returns>True if on cooldown, false if not</returns>
    private bool HandleCooldown()
    {
        if (m_reloadBar != null && m_reloadBar.IsRunning()) return true;

        if (m_currentCooldown >= 0)
        {
            m_currentCooldown -= Time.deltaTime;
            m_cooldownBar.SetVisible(true);
            HideReloadUi();
            m_cooldownBar.SetProgress01(m_currentCooldown / Mathf.Max(m_lastCooldownSet, 0.01f));
            return true;
        }

        m_cooldownBar.SetVisible(false);
        return false;
    }

    private void HideReloadUi()
    {
        m_reloadText.enabled = false;
        m_noBulletsForReloadText.enabled = false;
    }

    private void ShowReloadUi()
    {
        HideReloadUi();

        if (m_reloadBar && m_reloadBar.IsRunning()) return;

        if (m_noBulletsToReload && HoldingEmptyGun())
        {
            m_noBulletsForReloadText.enabled = true;
        }
        else
        {
            m_reloadText.enabled = true;
        }
    }

    private bool HoldingEmptyGun()
    {
        if (m_inventoryManager.GetHeldItem() is Gun gun)
        {
            return gun.GetCurrentAmmo() <= 0;
        }
        return false;
    }

    private void UpdateBulletForHeldGun()
    {
        // Is holding a gun?
        if (m_inventoryManager.GetHeldItem() is not Gun gun)
        {
            m_bulletForHeldGun = null;
            m_noBulletsToReload = false;
            return;
        }

        // Yes, find bullet
        m_bulletForHeldGun = GetBullet(gun);
        m_noBulletsToReload = false;
        if (m_bulletForHeldGun != null)
        {
            m_noBulletsToReload = m_inventoryManager.CountItemsOwned(m_bulletForHeldGun) < 1;
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
        
        
        ///m_animator.SetTrigger("Shoot");

        if (!m_isInfiniteAmmoCheatEnabled && m_gunUpgrades.ShouldConsumeAmmo())
        {
            SetAmmo(GetGunUnsafe().GetCurrentAmmo() - 1);
        }
        int extraBullets = m_gunUpgrades.GetExtraBullets();
        for (int i = 0; i < gun.GetNumBullets() + extraBullets; ++i)
        {
            ShootBullet();
        }
        m_cameraShake.ShakeFor(gun.GetCameraShake(), true);

        m_cameraScript.ApplyRecoil(gun.GetRecoilForce());
    }

    /// <summary>
    /// Handle swap cooldown, player does not need to be holding a gun when this method is called.
    /// Checks if a swap occurred and handles it if it did
    /// </summary>
    private void HandleGunSwap()
    {
        if (m_inventoryManager.GetHeldItem() is not Gun gun)
        {
            m_heldItemLastSwap = null;
            UpdateBulletForHeldGun();
            return;
        }

        if (m_heldItemLastSwap != gun)
        {
            // Swapped item
            SetupReloadBar();

            if (m_currentCooldown < gun.GetSwapCooldownSeconds())
            {
                SetShootCooldown(gun.GetSwapCooldownSeconds());
                UpdateBulletForHeldGun();
            }
            m_heldItemLastSwap = gun;
        }

        if (!m_reloadBar)
        {
            SetupReloadBar();
        }
    }

    private void HideGunUi()
    {
        if (m_reloadBar != null)
        {
            m_reloadBar.SetVisible(false);
        }
        m_reloadText.enabled = false;
        m_ammoText.enabled = false;
        m_noBulletsForReloadText.enabled = false;
        foreach (Transform child in m_ammoText.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    private void ShowGunUi()
    {
        // Reload text will enable itself when needed
        m_ammoText.enabled = true;
        if (m_reloadBar) m_reloadBar.SetVisible(true);
        foreach (Transform child in m_ammoText.transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    public void SetAmmo(int ammo)
    {
        if (m_inventoryManager.GetHeldItem() is not Gun gun) return;

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
        shootCooldown *= m_gunUpgrades.GetShootSpeedScalar();
        m_currentCooldown = shootCooldown;
        m_lastCooldownSet = shootCooldown;
    }

    private void TryPlayShootSound(Gun gun)
    {
        if (!gun.GetShootSfx()) return;

        float now = Time.time;
        if (m_shootSoundTimestamps[m_currentShootSoundIndex] < now)
        {
            m_shootSoundTimestamps[m_currentShootSoundIndex] = now + m_shootSoundInterval;
            m_currentShootSoundIndex = (m_currentShootSoundIndex + 1) % m_shootSoundTimestamps.Length;
            AudioSource.PlayClipAtPoint(gun.GetShootSfx(), transform.position, m_settings.GetSettings().GetGameVolume() * m_shootVolume);
        }
    }

    /// <summary>
    /// Shoot a bullet and set cooldown, should not be called when on cooldown, this will not decrement ammo
    /// </summary>
    private void ShootBullet()
    {
        SetShootCooldown(GetGunUnsafe().GetShootCooldownSeconds());

        Gun gun = GetGunUnsafe();
        TryPlayShootSound(gun);

        // See if bullet hit anything
        Ray ray = new Ray(m_camera.transform.position, GetRandomBulletDirection());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, MAX_RAY_DISTANCE, m_enemyLayer))
        {
            // Hit a groundhog, damage it
            GroundhogScript groundhogScript = hit.collider.gameObject.GetComponentInParent<GroundhogScript>();
            DamageGroundhog(groundhogScript, hit.point);
            ShowActionTooltip(m_shootGoodActionTooltip, hit.point);
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

            ShowActionTooltip(m_shootBadActionTooltip, hit.point);
        }
    }

    private void ShowActionTooltip(GameObject prefab, Vector3 position)
    {
        if (m_secondsSinceLastActionTooltipShowed < m_actionTooltipInterval) return;
        m_secondsSinceLastActionTooltipShowed = 0;
        Instantiate(prefab).transform.position = position + m_actionTooltipOffset;
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
    private void DamageGroundhog(GroundhogScript groundhog, Vector3 bulletWorldPosition)
    {
        float baseDamage = GetGunUnsafe().GetDamage();
        baseDamage += m_inventoryManager.GetHeldInventoryItem().ReturnMultiplier();
        groundhog.Damage(baseDamage * Random.Range(1.0f - m_randomDamageScale, 1.0f + m_randomDamageScale), bulletWorldPosition);
    }

    /// <summary>
    /// Reload ammo unless at max ammo, set cooldown, should not be called when on cooldown
    /// </summary>
    async void AttemptAmmoReload()
    {
        Gun gun = GetGunUnsafe();
        if (gun.GetCurrentAmmo() >= gun.GetMaxAmmo()) return;

        if (gun.DoesRequireBullets())
        {
            // Check if we have the ammo pack
            if (m_noBulletsToReload) return;

            // Remove ammo pack
            bool removedSuccessfully = m_inventoryManager.RemoveItems(m_bulletForHeldGun, 1);
            Assert.IsTrue(removedSuccessfully);
            UpdateBulletForHeldGun();
        }

        // Reload
        ///m_animator.SetBool("Reloading", true);
        float reloadCooldown = gun.GetReloadCooldownSeconds();
        gun.PlayReloadSfx();
        m_reloadBar.StartProgress(reloadCooldown);
        await Task.Delay((int)(reloadCooldown * 1000 - 10));
        if (gun == m_inventoryManager.GetHeldInventoryItem().item) SetAmmo(gun.GetMaxAmmo());
        ///m_animator.SetBool("Reloading", false);
    }

    /// <summary>
    /// Get the bullet (ammo pack) associated with a gun
    /// </summary>
    /// <param name="gun">The gun</param>
    /// <returns>The bullet, or null if one doesn't exist</returns>
    private Bullet GetBullet(Gun gun)
    {
        foreach (BaseItem item in m_inventoryManager.GetItemTypes())
        {
            if (item is not Bullet bullet) continue;
            if (bullet.GetGunName().Equals(gun.GetItemName())) return bullet;
        }
        return null;
    }

    private Gun GetGunUnsafe()
    {
        BaseItem heldItem = m_inventoryManager.GetHeldItem();
        if (heldItem is Gun gun) return gun;
        Assert.IsTrue(false);
        return null;
    }
}
