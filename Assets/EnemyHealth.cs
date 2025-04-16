using TMPro;
using UnityEngine;

public class EnemyHealth : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] private float maxHealth = 10f;

    [SerializeField] private float normalDamageMultiplier = 1f;
    [SerializeField] private float fireDamageMultiplier = 1.5f;
    [SerializeField] private float waterDamageMultiplier = 0.8f;
    [SerializeField] private float lightningDamageMultiplier = 1.2f;
    [SerializeField] private float leafDamageMultiplier = 0.7f;

    [Header("Effects")]
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private GameObject deathEffect;

    private GameStateManager gameStateManager;
    public int mobExp = 20;
    private float currentHealth;
    private bool isDead = false;

    public int rewardAmount = 10; 
    public int soulAmount = 10; // 10 soul per enemy
    public int scorePerKill = 10;

    void Start() {
        currentHealth = maxHealth;
        gameStateManager = GameObject.Find("GameStateManager").GetComponent<GameStateManager>();
    }

    public void TakeDamage(float baseDamage) {
        if (isDead) return;

        float damage = baseDamage;
        currentHealth -= damage;

        // Optional hit effect
        // if (hitEffect != null) hitEffect.Play();

        if (currentHealth <= 0) {
            Die();
        }
    }

    private float GetDamageMultiplier(AttackType attackType) {
        switch (attackType) {
            case AttackType.Fire: return fireDamageMultiplier;
            case AttackType.Water: return waterDamageMultiplier;
            case AttackType.Lightning: return lightningDamageMultiplier;
            case AttackType.Leaf: return leafDamageMultiplier;
            default: return normalDamageMultiplier;
        }
    }

    private void Die() {
        if (isDead) return;
        isDead = true;

        Debug.Log("Enemy died!");
        gameStateManager.addExp(mobExp);

        // Update round score (runtime only)
        PlayerInfo player = FindObjectOfType<PlayerInfo>();
        if (player != null) {
            player.AddScore(scorePerKill);
        }

        // Update saved souls
        PlayerData data = SaveSystem.LoadProgress();
        data.balance += soulAmount;
        if (player != null) {
            player.AddSouls(soulAmount);
        }
        SaveSystem.SaveProgress(data);

        // Optional death effect
        // if (deathEffect != null) Instantiate(deathEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}

public enum AttackType {
    Normal,
    Fire,
    Water,
    Lightning,
    Leaf
}
