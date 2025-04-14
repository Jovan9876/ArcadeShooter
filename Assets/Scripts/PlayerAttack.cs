using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elements
{
    public enum AttackType
    {
        Normal,
        Fire,
        Water,
        Lightning,
        Leaf
    }
}

public class PlayerAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject ActivePrefab;
    [SerializeField] private GameObject NormalPrefab;
    [SerializeField] private GameObject FirePrefab;
    [SerializeField] private GameObject WaterPrefab;
    [SerializeField] private GameObject LightningPrefab;
    [SerializeField] private GameObject LeafPrefab;
    [SerializeField] private Transform firePoint;

    [Header("Settings")]
    [SerializeField] private float attackRate = 0.5f;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private LayerMask enemyLayer;

    private float nextAttackTime;
    private CharacterController characterController;
    public float damage = 20f;
    private float finalDamage;
    public Dictionary<Elements.AttackType, float> damageModifiers = new Dictionary<Elements.AttackType, float>();

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        damageModifiers[Elements.AttackType.Normal] = 1.0f;
        damageModifiers[Elements.AttackType.Fire] = 1.0f;
        damageModifiers[Elements.AttackType.Water] = 1.0f;
        damageModifiers[Elements.AttackType.Lightning] = 1.0f;
        damageModifiers[Elements.AttackType.Leaf] = 1.0f;

        SetNormalAttack();
    }

    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackRate;
        }
    }

    // Upgrades element damage
    public void upgradeElement(Elements.AttackType element, float modifier)
    {
        damageModifiers[element] += modifier;
    }

    public Dictionary<Elements.AttackType, float> getUpgrades()
    {
        return damageModifiers;
    }

    // Setters for attack types
    public void SetNormalAttack()
    {
        ActivePrefab = NormalPrefab;
        finalDamage = damage * damageModifiers[Elements.AttackType.Normal];
    }

    public void SetFireAttack()
    {
        ActivePrefab = FirePrefab;
        finalDamage = damage * damageModifiers[Elements.AttackType.Fire];
    }

    public void SetWaterAttack()
    {
        ActivePrefab = WaterPrefab;
        finalDamage = damage * damageModifiers[Elements.AttackType.Water];
    }

    public void SetLightningAttack()
    {
        ActivePrefab = LightningPrefab;
        finalDamage = damage * damageModifiers[Elements.AttackType.Lightning];
    }

    public void SetLeafAttack()
    {
        ActivePrefab = LeafPrefab;
        finalDamage = damage * damageModifiers[Elements.AttackType.Leaf];
    }

    void Attack()
    {
        Vector3 direction = Quaternion.Euler(0, 90, 0) * transform.forward;

        if (direction == Vector3.zero) return;

        GameObject projectile = Instantiate(ActivePrefab, firePoint.position, Quaternion.LookRotation(direction)); ;

        var ps = projectile.GetComponent<ParticleSystem>();
        AudioScript audioScript = projectile.GetComponent<AudioScript>();
        if (ps != null)
        {
            var main = ps.main;
            main.startSpeed = 0;
            ps.Play();
        }

        if (audioScript != null)
        {
            MasterAudio.PlaySound(audioScript.sfxName);
        }

        StartCoroutine(MoveProjectile(projectile, direction));
    }

    IEnumerator MoveProjectile(GameObject projectile, Vector3 direction)
    {
        float distanceTraveled = 0f;
        bool hitDetected = false;

        AddCollisionComponents(projectile);

        while (distanceTraveled < maxDistance && !hitDetected)
        {
            hitDetected = CheckCollision(projectile.transform.position, direction);

            if (hitDetected)
            {
                HandleImpact(projectile);
                yield break;
            }

            float moveAmount = moveSpeed * Time.deltaTime;
            projectile.transform.position += direction * moveAmount;
            distanceTraveled += moveAmount;

            yield return null;
        }

        Destroy(projectile);
    }

    void AddCollisionComponents(GameObject projectile)
    {
        if (!projectile.GetComponent<Collider>())
        {
            var collider = projectile.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = 0.5f;
        }

        if (!projectile.GetComponent<Rigidbody>())
        {
            var rb = projectile.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    bool CheckCollision(Vector3 position, Vector3 direction)
    {
        if (Physics.Raycast(position, direction, out RaycastHit hit, moveSpeed * Time.deltaTime, enemyLayer))
        {
            hit.collider.GetComponent<EnemyHealth>()?.TakeDamage(damage);
            return true;
        }

        return Physics.SphereCast(position, 0.3f, direction, out hit, moveSpeed * Time.deltaTime, enemyLayer);
    }

    void HandleImpact(GameObject projectile)
    {
        var ps = projectile.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            Destroy(projectile, ps.main.duration);
        }
        else
        {
            Destroy(projectile, 0.1f);
        }
    }
}
