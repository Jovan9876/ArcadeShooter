using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elements{
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
    [SerializeField] private FixedJoystick movementJoystick;
    [SerializeField] private FixedJoystick aimJoystick;

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
        
        // Initialize with normal attack by default
        SetNormalAttack();

        // Debug.Log("Damage Modifiers");
        // Debug.Log(damageModifiers["fire"]);
        // Debug.Log(damageModifiers["water"]);
        // Debug.Log(damageModifiers["neutral"]);
        // Debug.Log(damageModifiers["leaf"]);
        // Debug.Log(damageModifiers["lightning"]);

    }

    void Update()
    {
        //HandleMovement();
        HandleRotation();

        if (Time.time >= nextAttackTime && HasAimInput())
        {
            Attack();
            nextAttackTime = Time.time + attackRate;
        }
    }

    // Upgrades element damage
    public void upgradeElement(Elements.AttackType element, float modifier) {
        damageModifiers[element] += modifier;
    }

    public Dictionary<Elements.AttackType, float> getUpgrades(){
        return damageModifiers;
    }

/*    void HandleMovement()
    {
        Vector3 moveDirection = new Vector3(movementJoystick.Horizontal, 0, movementJoystick.Vertical);
        if (moveDirection.magnitude > 0)
        {
            // Move the character
            characterController.Move(moveDirection.normalized * moveSpeed * Time.deltaTime);
        }
    }*/

    void HandleRotation()
    {
        Vector3 aimDirection = new Vector3(aimJoystick.Horizontal, 0, aimJoystick.Vertical);
        if (aimDirection.magnitude > 0)
        {
            // Rotate the character to face aim direction
            Quaternion targetRotation = Quaternion.LookRotation(aimDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    // Public functions to change attack type (call these from button click events)
    public void SetNormalAttack()
    {
        ActivePrefab = NormalPrefab;
        Debug.Log("Attack type set to: Normal");
        finalDamage = damage * damageModifiers[Elements.AttackType.Normal];
    }

    public void SetFireAttack()
    {
        ActivePrefab = FirePrefab;
        Debug.Log("Attack type set to: Fire");
        finalDamage = damage * damageModifiers[Elements.AttackType.Fire];
    }

    public void SetWaterAttack()
    {
        ActivePrefab = WaterPrefab;
        Debug.Log("Attack type set to: Water");
        finalDamage = damage * damageModifiers[Elements.AttackType.Water];

    }

    public void SetLightningAttack()
    {
        ActivePrefab = LightningPrefab;
        Debug.Log("Attack type set to: Lightning");
        finalDamage = damage * damageModifiers[Elements.AttackType.Lightning];

    }

    public void SetLeafAttack()
    {
        ActivePrefab = LeafPrefab;
        Debug.Log("Attack type set to: Leaf");
        finalDamage = damage * damageModifiers[Elements.AttackType.Leaf];

    }

    void Attack()
    {
        Vector3 direction = GetAttackDirection();
        if (direction == Vector3.zero) return;

        GameObject projectile = Instantiate(ActivePrefab, firePoint.position, Quaternion.LookRotation(direction));

        // Configure particle system
        var ps = projectile.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;
            main.startSpeed = 0;
            ps.Play();
        }

        StartCoroutine(MoveProjectile(projectile, direction));
    }

    IEnumerator MoveProjectile(GameObject projectile, Vector3 direction)
    {
        float distanceTraveled = 0f;
        Vector3 startPosition = projectile.transform.position;
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
        // Check for collisions with enemy layer only
        if (Physics.Raycast(position, direction, out RaycastHit hit, moveSpeed * Time.deltaTime, enemyLayer))
        {
            hit.collider.GetComponent<EnemyHealth>().TakeDamage(damage);
            return true;
        }

        // Alternative sphere cast
        return Physics.SphereCast(position, 0.3f, direction, out hit, moveSpeed * Time.deltaTime, enemyLayer);
    }

    void HandleImpact(GameObject projectile)
    {
        var ps = projectile.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
        Destroy(projectile, ps?.main.duration ?? 0.1f);
    }

    private bool HasAimInput() => aimJoystick.Horizontal != 0 || aimJoystick.Vertical != 0;
    private Vector3 GetAttackDirection() => new Vector3(aimJoystick.Horizontal, 0, aimJoystick.Vertical).normalized;
}