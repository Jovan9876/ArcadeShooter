using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class PlayerInfo : MonoBehaviour {
    public int health = 3;
    private int currentHealth;

    public TMP_Text healthTMP;
    public TMP_Text timeAliveTMP;
    public TMP_Text scoreTMP;
    public TMP_Text soulsGainedTMP;
    private int score = 0;
    private int soulsGained = 0;
    public GameObject deathMessageUI;

    public string blackjack = "BlackjackScene";

    private float timeAlive = 0f;
    private bool isAlive = true;

    private void Start() {
        currentHealth = health;
        UpdateHealthUI();

        if (deathMessageUI != null) {
            deathMessageUI.SetActive(false);
        }
    }

    private void Update() {
        if (isAlive) {
            timeAlive += Time.deltaTime;

            if (timeAliveTMP != null) {
                timeAliveTMP.text = Mathf.FloorToInt(timeAlive).ToString() + "s";
            }
            if (scoreTMP != null) {
                scoreTMP.text = score.ToString();
            }
            if (soulsGainedTMP != null) {
                soulsGainedTMP.text = soulsGained.ToString();
            }
        }
    }

    public void AddScore(int amount) {
        score += amount;
    }

    public void AddSouls(int amount) {
        soulsGained += amount;
    }


    public void TakeDamage(int amount) {
        currentHealth -= amount;
        UpdateHealthUI();

        if (currentHealth <= 0) {
            Die();
        }
    }

    private void UpdateHealthUI() {
        int displayHealth = Mathf.Max(0, currentHealth);
        if (healthTMP != null) {
            healthTMP.text = "HP : " + displayHealth;
        }
    }

    private void Die() {
        Debug.Log("Player died!");
        isAlive = false;

        if (deathMessageUI != null) {
            deathMessageUI.SetActive(true);
            Time.timeScale = 0f;
        }

    }

    private IEnumerator death() {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(blackjack);
    }
}
