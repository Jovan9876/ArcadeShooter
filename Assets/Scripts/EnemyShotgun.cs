using UnityEngine;

public class EnemyShotgun : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint; 
    public float bulletSpeed = 10f;
    public int bulletCount = 5;
    public float spreadAngle = 30f; 
    public float fireRate = 1.5f;

    private Transform player;
    private float nextFireTime = 0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null) return;

        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        if (player == null) return;

        Vector3 directionToPlayer = (player.position - firePoint.position).normalized;
        float baseAngle = Mathf.Atan2(directionToPlayer.z, directionToPlayer.x) * Mathf.Rad2Deg;

        for (int i = 0; i < bulletCount; i++)
        {
            float angleOffset = spreadAngle * ((float)i / (bulletCount - 1) - 0.5f);
            float finalAngle = baseAngle + angleOffset;
            Vector3 bulletDirection = new Vector3(Mathf.Cos(finalAngle * Mathf.Deg2Rad), 0, Mathf.Sin(finalAngle * Mathf.Deg2Rad));

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = bulletDirection * bulletSpeed;
            }
        }
    }
}
