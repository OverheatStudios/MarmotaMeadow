using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "Scriptable Objects/Gun")]
public class Gun : BaseItem
{
    /// <summary>
    /// Damage per bullet, 10 is a nice number
    /// </summary>
    [SerializeField] private float m_damage;
    [SerializeField] private int m_numBulletsPerShot;
    [SerializeField] private float m_shootCooldownSeconds;
    [SerializeField] private float m_reloadCooldownSeconds;
    [SerializeField] private int m_maxAmmo;
    private int m_currentAmmo;
    /// <summary>
    /// Bullet spread, 0.01f is a nice number
    /// </summary>
    [SerializeField] private float m_spread = 0.01f;
    /// <summary>
    /// Scale of bullet hole prefab (.15 is a good number)
    /// </summary>
    [SerializeField] private float m_bulletHoleSize;
    [SerializeField] private float m_swapCooldownSeconds = 1.5f;

    private void OnEnable()
    {
        m_currentAmmo = m_maxAmmo;
    }

    public float GetDamage()
    {
        return m_damage;
    }

    public int GetNumBullets()
    {
        return m_numBulletsPerShot;
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
