using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "Scriptable Objects/Gun")]
public class Gun : BaseItem
{
    [Tooltip("Damage per bullet, 10 is a nice number")]
    [SerializeField] private float m_damage;
    [SerializeField] private int m_numBulletsPerShot;
    [SerializeField] private float m_shootCooldownSeconds;
    [SerializeField] private float m_reloadCooldownSeconds;
    [SerializeField] private int m_maxAmmo;
    private int m_currentAmmo;
    [Tooltip("Bullet spread, 0.01f is a nice number")]
    [SerializeField] private float m_spread = 0.01f;
    [Tooltip("Scale of bullet hole prefab (.15 is a good number)")]
    [SerializeField] private float m_bulletHoleSize;
    [SerializeField] private float m_swapCooldownSeconds = 1.5f;
    [SerializeField] private float m_purchasePrice;
    [Tooltip("If this is false, there is still ammo but players can reload any time for free. If this is true, reloading will cost an ammo pack item (purchasable in shop)")]
    [SerializeField] private bool m_requiresBullets;
    [Tooltip("Gun recoil force, 5 is a good number")]
    [SerializeField] private float m_recoilForce = 5.0f;
    [SerializeField] private GameObject m_reloadBarPrefab;
    [SerializeField] private AudioClip m_reloadSfx;
    [SerializeField] private int m_reloadSfxLoopCount = 4;
    [SerializeField] private AudioClip m_shootSfx;
    [SerializeField] private AudioClip m_reloadFinishSfx;
    [SerializeField] private CameraShakeSettings m_cameraShake;
    [SerializeField] private int m_upgradeCost = 5;
    [SerializeField] private SettingsScriptableObject m_settings;

    private void OnEnable()
    {
        m_currentAmmo = m_maxAmmo;
    }

    public int GetUpgradeCost()
    {
        return m_upgradeCost;
    }

    public void PlayReloadSfx()
    {
        if (!m_reloadSfx) return;

        // Reload sfx
        GameObject reload = new GameObject();

        AudioSource reloadSource = reload.AddComponent<AudioSource>();
        reloadSource.clip = m_reloadSfx;
        reloadSource.loop = true;
        reloadSource.volume = m_settings.GetSettings().GetGameVolume();
        reloadSource.Play();

        float reloadDuration = m_reloadSfxLoopCount * m_reloadSfx.length + 0.01f;
        reload.AddComponent<AutoDestroyScript>().SetDelay(reloadDuration, 0);

        // Pump sfx
        if (!m_reloadFinishSfx) return;
        GameObject pump = new GameObject();

        AudioSource pumpSource = pump.AddComponent<AudioSource>();
        pumpSource.clip = m_reloadFinishSfx;
        pumpSource.volume = m_settings.GetSettings().GetGameVolume();
        pumpSource.PlayDelayed(reloadDuration);

        reload.AddComponent<AutoDestroyScript>().SetDelay(reloadDuration + m_reloadFinishSfx.length + 0.01f, 0);
    }

    public CameraShakeSettings GetCameraShake() { return m_cameraShake; }

    public AudioClip GetShootSfx()
    {
        return m_shootSfx;
    }

    public override float ReturnBuyCoinsAmount()
    {
        return m_purchasePrice;
    }

    public GameObject GetReloadBarPrefab()
    {
        return m_reloadBarPrefab;
    }

    public bool DoesRequireBullets()
    {
        return m_requiresBullets;
    }

    public float GetDamage()
    {
        return m_damage;
    }

    public int GetNumBullets()
    {
        return m_numBulletsPerShot;
    }

    public float GetRecoilForce()
    {
        return m_recoilForce;
    }

    public float GetShootCooldownSeconds()
    {
        return m_shootCooldownSeconds;
    }

    public float GetReloadCooldownSeconds()
    {
        return m_reloadCooldownSeconds;
    }

    public int GetMaxAmmo()
    {
        return m_maxAmmo;
    }

    public int GetCurrentAmmo()
    {
        return m_currentAmmo;
    }

    public void SetCurrentAmmo(int ammo)
    {
        Assert.IsTrue(ammo >= 0);
        Assert.IsTrue(ammo <= m_maxAmmo);
        m_currentAmmo = ammo;
    }

    public float GetBulletSpread()
    {
        return m_spread;
    }

    public float GetBulletHoleSize()
    {
        return m_bulletHoleSize;
    }

    public float GetSwapCooldownSeconds()
    {
        return m_swapCooldownSeconds;
    }
}
