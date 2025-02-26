using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab; // Assign your projectile prefab in Inspector
    [SerializeField] private Transform firePoint;        // Where projectiles spawn
    [SerializeField] private FixedJoystick joystick;     // Reference to the joystick
    [SerializeField] private float attackRate = 0.5f;    // Time between attacks
    [SerializeField] private float projectileSpeed = 10f;// Speed of projectiles

    private float nextAttackTime;

    void Update()
    {
        if (Time.time >= nextAttackTime && (joystick.Horizontal != 0 || joystick.Vertical != 0))
        {
            Attack();
            nextAttackTime = Time.time + attackRate;
        }
    }

    void Attack()
    {
        // Determine attack direction based on joystick input
        Vector3 attackDirection = new Vector3(joystick.Horizontal, 0, joystick.Vertical).normalized;

        if (attackDirection != Vector3.zero)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(attackDirection));
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = attackDirection * projectileSpeed;
            }
        }
    }
}
