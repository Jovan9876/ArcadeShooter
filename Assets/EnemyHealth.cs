using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float maxHealth = 10f;

    //too be implemented
    [SerializeField] private float normalDamageMultiplier = 1f;
    [SerializeField] private float fireDamageMultiplier = 1.5f;
    [SerializeField] private float waterDamageMultiplier = 0.8f;
    [SerializeField] private float lightningDamageMultiplier = 1.2f;
    [SerializeField] private float leafDamageMultiplier = 0.7f;

    [Header("Effects")]
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private GameObject deathEffect;

    private float currentHealth;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
    }


    public void TakeDamage(float baseDamage)
    {
        if (isDead) return;

        float damage = baseDamage;
        currentHealth -= damage;

        // Show hit effect
/*        if (hitEffect != null)
        {
            hitEffect.Play();
        }*/

        // Check for death
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private float GetDamageMultiplier(AttackType attackType)
    {
        switch (attackType)
        {
            case AttackType.Fire:
                return fireDamageMultiplier;
            case AttackType.Water:
                return waterDamageMultiplier;
            case AttackType.Lightning:
                return lightningDamageMultiplier;
            case AttackType.Leaf:
                return leafDamageMultiplier;
            default:
                return normalDamageMultiplier;
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Enemy died!");

        // Play death effect
/*        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }*/



        Destroy(gameObject);
    }
}


public enum AttackType
{
    Normal,
    Fire,
    Water,
    Lightning,
    Leaf
}