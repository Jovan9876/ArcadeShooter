using UnityEngine;
using System.Collections;

public class LaserAttack : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform firePoint;
    public float laserRange = 10f;
    public float warningDuration = 1f;
    public float trackingDuration = 0.5f;
    public float laserDuration = 2f;
    public float fireRate = 3f;
    public int laserDamage = 10;

    public Color warningColor = Color.yellow;
    public Color laserColor = Color.red; 
    public Material laserMaterial;

    private Transform player;
    private bool isFiring = false;
    private Vector3 lockedDirection;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        lineRenderer.enabled = false;

        if (laserMaterial != null)
        {
            lineRenderer.material = laserMaterial;
        }

        InvokeRepeating(nameof(StartLaserAttack), 1f, fireRate);
    }

    void StartLaserAttack()
    {
        if (player == null || isFiring) return;
        StartCoroutine(LaserSequence());
    }

    private IEnumerator LaserSequence()
    {
        isFiring = true;
        lineRenderer.enabled = true;

        lineRenderer.material.color = warningColor;
        lineRenderer.startColor = warningColor;
        lineRenderer.endColor = warningColor;

        Vector3 direction = (player.position - firePoint.position).normalized;
        for (float t = 0; t < trackingDuration; t += Time.deltaTime)
        {
            direction = (player.position - firePoint.position).normalized;
            SetLaserPosition(direction);
            yield return null;
        }

        lockedDirection = direction;
        SetLaserPosition(lockedDirection);

        yield return new WaitForSeconds(warningDuration);

        lineRenderer.material.color = laserColor;
        lineRenderer.startColor = laserColor;
        lineRenderer.endColor = laserColor;

        float elapsed = 0f;
        while (elapsed < laserDuration)
        {
            elapsed += Time.deltaTime;
            DamagePlayer();
            yield return null;
        }

        lineRenderer.enabled = false;
        isFiring = false;
    }

    void SetLaserPosition(Vector3 direction)
    {
        Vector3 endPoint = firePoint.position + direction * laserRange;
        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, endPoint);
    }

    void DamagePlayer()
    {
        if (Physics.Raycast(firePoint.position, lockedDirection, out RaycastHit hit, laserRange))
        {
        }
    }
}
