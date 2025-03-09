using UnityEngine;

public class Player : MonoBehaviour {
    [SerializeField] private PlayerHand playerHand;
    [SerializeField] private Dealer dealer;

    private int balance = 0;
    public int currentBet = 0;
    public bool betPlaced = false;

    private const string BALANCE_KEY = "Player_Currency";

    private void Start() {
        LoadBalance();  // Load balance when the game starts
    }

    private void LoadBalance() {
        balance = PlayerPrefs.GetInt(BALANCE_KEY, 1000);
        Debug.Log($"Loaded Balance: {balance}");
    }

    private void SaveBalance() {
        PlayerPrefs.SetInt(BALANCE_KEY, balance);
        PlayerPrefs.Save();
    }

    public void PlaceBet(int amount) {
        if (amount > balance) {
            Debug.Log("Not enough balance to place bet.");
            return;
        }
        currentBet += amount;
        balance -= amount;
        betPlaced = true;
        SaveBalance();
        Debug.Log($"Player placed a bet of {amount}. New balance: {balance}");
    }

    public bool HasPlacedBet() {
        return betPlaced;
    }

    public void ResetBet() {
        betPlaced = false;
        currentBet = 0;
    }

    public void WinBet(float multiplier) {
        int winnings = Mathf.RoundToInt(currentBet * multiplier);
        balance += winnings;
        SaveBalance();
        Debug.Log($"Player won {winnings}. New balance: {balance}");
    }

    public void LoseBet() {
        Debug.Log($"Player lost {currentBet}. New balance: {balance}");
        SaveBalance();
    }

    public void Hit() {
        if (playerHand.GetScore() >= 21) {
            Stand();
        } else {
            dealer.PlayerHit();
        }
    }

    public void Stand() {
        dealer.PlayerStand();
    }
}
