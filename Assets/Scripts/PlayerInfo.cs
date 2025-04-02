using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class PlayerInfo : MonoBehaviour
{
    public int health = 3;
    private int currentHealth;

    public TMP_Text healthTMP;

    public GameObject deathMessageUI;
    public string blackjack = "BlackjackScene";

    private void Start()
    {
        currentHealth = health;
        UpdateHealthUI();

        if (deathMessageUI != null)
        {
            deathMessageUI.SetActive(false);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthUI()
    {
        int displayHealth = Mathf.Max(0, currentHealth);
        if (healthTMP != null)
        {
            healthTMP.text = "HP : " + displayHealth;
        }
    }

    private void Die()
    {
        Debug.Log("Player died!");

        if (deathMessageUI != null)
        {
            deathMessageUI.SetActive(true);
            Time.timeScale = 0f;
        }

        //StartCoroutine(death());
    }

    private IEnumerator death()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(blackjack);
    }
}
