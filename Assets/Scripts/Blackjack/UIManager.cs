using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour {
    [Header("Betting UI")]
    [SerializeField] private GameObject bettingPanel; // chips and deal button
    [SerializeField] private TextMeshProUGUI betAmount;
    [SerializeField] private TextMeshProUGUI playerBalance;


    [Header("Gameplay UI")]
    [SerializeField] private GameObject gameplayPanel; // Hit, Stand, Double Down, Split buttons
    [SerializeField] private GameObject doubleDownButton;


    private void Start() {
        Application.targetFrameRate = 60;
        ShowBettingUI();
    }

    public void ShowBettingUI() {
        bettingPanel.SetActive(true);
        gameplayPanel.SetActive(false);
    }

    public void ShowGameplayUI() {
        bettingPanel.SetActive(false);
        gameplayPanel.SetActive(true);
    }

    public void UpdateBetAmount(float bet) {
        if (betAmount != null) {
            betAmount.text = "$" + bet.ToString();
        }
    }

    public void UpdatePlayerBalance(float balance) {
        if (playerBalance != null) {
            playerBalance.text = "Balance: $" + balance.ToString();
        }
    }

    public void ToggleDoubleDown(bool show) {
        doubleDownButton.SetActive(show);
    }
}
