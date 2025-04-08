using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Manages the game's UI, including the betting and gameplay panels,
// button visibility, and updating UI text like bet amount and balance.
public class UIManager : MonoBehaviour {

    [Header("Betting UI")]
    [SerializeField] private GameObject bettingPanel;            // Panel with betting chips and Deal button
    [SerializeField] private TextMeshProUGUI betAmount;          // Text element showing current bet
    [SerializeField] private TextMeshProUGUI playerBalance;      // Text element showing player's balance

    [Header("Gameplay UI")]
    [SerializeField] private GameObject gameplayPanel;           // Panel with Hit, Stand, Double Down, Split
    [SerializeField] private GameObject doubleDownButton;        // Double Down button
    [SerializeField] private GameObject splitButton;             // Split button


    private void Start() {
        ShowBettingUI();
    }

    public void ShowBettingUI() {
        // Shows the betting UI and hides the gameplay UI
        bettingPanel.SetActive(true);
        gameplayPanel.SetActive(false);
    }
    public void HideGameplayUI() {
        // Hides the gameplay UI
        gameplayPanel.SetActive(false);
    }

    public void ShowGameplayUI() {
        // Shows the gameplay UI and hides the betting UI
        bettingPanel.SetActive(false);
        gameplayPanel.SetActive(true);
    }

    public void UpdateBetAmount(float bet) {
        // Updates the displayed bet amount
        if (betAmount != null) {
            betAmount.text = "$" + bet.ToString();
        }
    }

    public void UpdatePlayerBalance(float balance) {
        // Updates the displayed player balance
        if (playerBalance != null) {
            playerBalance.text = "Balance: $" + balance.ToString();
        }
    }

    public void ToggleDoubleDown(bool show) {
        // Shows or double down button
        doubleDownButton.SetActive(show);
    }

    public void ToggleSplit(bool show) {
        // Shows or hides split button
        splitButton.SetActive(show);
    }

}
