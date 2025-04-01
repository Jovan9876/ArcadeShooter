using UnityEngine;

public class ShotgunBullet : MonoBehaviour
{
    public float lifetime = 3f;
    private EnemyAttack enemyAttack;

    private void Start()
    {
        enemyAttack = GetComponent<EnemyAttack>();
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemyAttack?.DealDamage(other.gameObject);
            Destroy(gameObject);
        }
    }
}
