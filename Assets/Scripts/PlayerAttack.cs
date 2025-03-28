using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private FixedJoystick joystick;

    [Header("Settings")]
    [SerializeField] private float attackRate = 0.5f;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private LayerMask enemyLayer; // Set this to your Enemy layer in Inspector

    private float nextAttackTime;

    void Update()
    {
        if (Time.time >= nextAttackTime && HasJoystickInput())
        {
            Attack();
            nextAttackTime = Time.time + attackRate;
        }
    }

    void Attack()
    {
        Vector3 direction = GetAttackDirection();
        if (direction == Vector3.zero) return;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));

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

            // You can access the enemy component here if needed
            // Example: hit.collider.GetComponent<EnemyHealth>().TakeDamage();
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

    private bool HasJoystickInput() => joystick.Horizontal != 0 || joystick.Vertical != 0;
    private Vector3 GetAttackDirection() => new Vector3(joystick.Horizontal, 0, joystick.Vertical).normalized;
}