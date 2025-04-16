using UnityEngine;

public class EnemyAttack : MonoBehaviour
{

    public int damage = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void DealDamage(GameObject target)
    {
        PlayerInfo playerHealth = target.GetComponent<PlayerInfo>();
        if (playerHealth != null)
        {
            Debug.Log("Dealing " + damage + " damage to player");
            playerHealth.TakeDamage(damage);
        }
        else
        {
            Debug.Log("PlayerHealth component not found on " + target.name);
        }
    }

}
