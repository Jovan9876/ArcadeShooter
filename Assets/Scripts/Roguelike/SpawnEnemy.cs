using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public GameObject BasicEnemy;
    private static float minDistance = 10f;
    private static float maxDistance = 12f;
    public float spawnDelay = 6f;
    private float spawnTimer = 0f;

    private Vector3 generateRandomSpawnPoint(Vector3 center)
    {
        Vector3 randomSpawnPoint = Random.insideUnitSphere * maxDistance;

        // Ensure the point is at least minDistance away
        while (Vector3.Distance(randomSpawnPoint, center) < minDistance)
        {
            randomSpawnPoint = Random.insideUnitSphere * maxDistance;
        }

        randomSpawnPoint += center;
        randomSpawnPoint.y = 20f;

        return randomSpawnPoint;
    }

    void FixedUpdate()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnDelay)
        {
            spawnTimer = 0;
            Vector3 randomSpawnPoint = generateRandomSpawnPoint(transform.position);
            Instantiate(BasicEnemy, randomSpawnPoint, Quaternion.identity);
        }
    }
}