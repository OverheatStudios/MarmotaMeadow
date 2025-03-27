using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GroundhogAttackScript : MonoBehaviour
{
    [SerializeField] private GroundhogTypeInfo m_typeInfo;
    [Tooltip("Projectile to spawn, make sure it doesn't collide with groundhog or boundary")]
    [SerializeField] private GameObject m_projectilePrefab;
    [Tooltip("Interval between attacks in seconds")]
    [SerializeField] private float m_minAttackInterval = 1.0f;
    [SerializeField] private float m_maxAttackInterval = 1.25f;
    [Tooltip("Scale of the instantiated projectile")]
    [SerializeField] private float m_minScale = 1.0f;
    [SerializeField] private float m_maxScale = 1.0f;
    // May be scaled above maximum depending on distance to player
    [Tooltip("Base speed of the projectile")]
    [SerializeField] private float m_minProjectileSpeed = 8;
    [SerializeField] private float m_maxProjectileSpeed = 10;
    [Tooltip("Projectile inaccuracy (0.03 is a good number)")]
    [SerializeField] private float m_minProjectileInaccuracy = 0.01f;
    [SerializeField] private float m_maxProjectileInaccuracy = 0.025f;
    [Tooltip("Projectile damage")]
    [SerializeField] private float m_projectileDamage = 1;
    [Tooltip("Projectile velocity will be multiplied based on the distance to the player, so further groundhogs shoot faster projectiles (necessary to counteract gravity)")]
    [SerializeField] private float m_minVelocityScalar = 1;
    [SerializeField] private float m_maxVelocityScalar = 2;
    [Tooltip("Add some additional Y velocity to counteract gravity")]
    [SerializeField] private float m_additionalYVelocity = 2.0f;
    [Tooltip("Position offset for the spawn location of the projectile, useful for preventing the projectile spawning in the ground")]
    [SerializeField] private Vector3 m_projectileSpawnOffset = new Vector3(0, 1, 0);

    private float m_secondsToAttack;

    private void Start()
    {
        Assert.IsTrue(m_minScale > 0.0f);
        Assert.IsTrue(m_minScale <= m_maxScale);

        Assert.IsTrue(m_minAttackInterval > 0.0f);
        Assert.IsTrue(m_minAttackInterval <= m_maxAttackInterval);

        Assert.IsTrue(m_minProjectileSpeed > 0.0f);
        Assert.IsTrue(m_minProjectileSpeed <= m_maxProjectileSpeed);

        Assert.IsTrue(m_minProjectileInaccuracy >= 0.0f);
        Assert.IsTrue(m_minProjectileInaccuracy <= m_maxProjectileInaccuracy);

        m_secondsToAttack = GetAttackCooldown();
    }

    private void Update()
    {
        m_secondsToAttack -= Time.deltaTime;
        if (m_secondsToAttack >= 0) return;
        if (m_typeInfo.CurrentState != GroundhogScript.GroundhogState.Idle) return; // don't attack while going up or down

        m_secondsToAttack = GetAttackCooldown();
        Attack();
    }

    private void Attack()
    {
        Vector3 target = MovementScript.PlayerInstance.transform.position;

        // Instantiate projectile
        GameObject proj = Instantiate(m_projectilePrefab);
        proj.transform.position = transform.position + m_projectileSpawnOffset;
        proj.transform.localScale = Vector3.one * Random.Range(m_minScale, m_maxScale);
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        proj.GetComponent<GroundhogProjectile>().Damage = m_projectileDamage;

        // Throw at player
        Vector3 toPlayer = target - transform.position;
        float velocityScale = Mathf.Lerp(m_minVelocityScalar, m_maxVelocityScalar, Mathf.InverseLerp(4 * 4, 10 * 10, toPlayer.sqrMagnitude)); // Increase scale based on distance or close enemies will overshoot and far enemies will undershoot
        rb.velocity = ((Vector3.up * m_additionalYVelocity) + (GetSpeed() * (toPlayer.normalized + GetInaccuracy()))) * velocityScale;

    }

    private float GetAttackCooldown()
    {
        return Random.Range(m_minAttackInterval, m_maxAttackInterval);
    }

    private float GetSpeed()
    {
        return Random.Range(m_minProjectileSpeed, m_maxProjectileSpeed);
    }

    private Vector3 GetInaccuracy()
    {
        return new Vector3(Random.Range(m_minProjectileInaccuracy, m_maxProjectileInaccuracy),
            Random.Range(m_minProjectileInaccuracy, m_maxProjectileInaccuracy),
            Random.Range(m_minProjectileInaccuracy, m_maxProjectileInaccuracy));
    }
}
