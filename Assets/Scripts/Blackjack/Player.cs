using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Player : MonoBehaviour {
    [SerializeField] private PlayerHand playerHand;
    [SerializeField] private Dealer dealer;
    [SerializeField] private UIManager manager;
    [SerializeField] private GameObject bettingArea;


    private int balance = 0;
    public int currentBet = 0;
    public bool betPlaced = false;

    private const string BALANCE_KEY = "Player_Currency";

    [SerializeField] private GameObject[] chipPrefabs;
    private List<GameObject> placedChips = new List<GameObject>();


    private void Start() {
        LoadBalance();
    }

    private void LoadBalance() {
        balance = PlayerPrefs.GetInt(BALANCE_KEY, 100000);
        Debug.Log($"Loaded Balance: {balance}");
    }

    private void SaveBalance() {
        PlayerPrefs.SetInt(BALANCE_KEY, balance);
        PlayerPrefs.Save();
    }

    public void PlaceBet(Chip chip) {
        int chipValue = chip.chipValue;
        if (chipValue > balance) {
            Debug.Log("Not enough balance to place bet.");
            return;
        }
        betPlaced = true;
        currentBet += chipValue;
        balance -= chipValue;
        PlaceBetOnTable(chip);
        Debug.Log($"Player placed a bet of {chipValue}. New balance: {balance}");
    }

    public void RemoveBet(Chip chip) {
        if (placedChips.Contains(chip.gameObject)) {
            placedChips.Remove(chip.gameObject);
            Destroy(chip.gameObject);
            currentBet -= chip.chipValue;
            balance += chip.chipValue;
            SaveBalance();
            Debug.Log($"Removed a {chip.chipValue} chip. New balance: {balance}");
            ReorderChips();
        }
    }

    private void PlaceBetOnTable(Chip chip) {
        if (bettingArea == null) {
            Debug.LogError("Betting area not set up!");
            return;
        }

        GameObject chipInstance = Instantiate(chipPrefabs[chip.chipIndex], bettingArea.transform, false);
        Destroy(chipInstance.GetComponentInChildren<UniversalAdditionalLightData>());
        Destroy(chipInstance.GetComponentInChildren<Light>());

        Chip chipComponent = chipInstance.GetComponent<Chip>();
        if (chipComponent != null) {
            chipComponent.IsPlaced = true;
            chipComponent.player = this;
        }

        chipInstance.transform.localRotation = Quaternion.Euler(90, 0, 0);

        placedChips.Add(chipInstance);
        ReorderChips();
    }


    private void ReorderChips() {
        placedChips.Sort((a, b) => {
            Chip chipA = a.GetComponent<Chip>();
            Chip chipB = b.GetComponent<Chip>();
            if (chipA == null || chipB == null) return 0;
            return chipB.chipValue - chipA.chipValue;
        });

        for (int i = 0; i < placedChips.Count; i++) {
            placedChips[i].transform.SetSiblingIndex(i);
            placedChips[i].transform.localPosition = new Vector3(0, i * 0.02f, 0);
            placedChips[i].transform.localRotation = Quaternion.Euler(90, 0, 0);
        }
    }

    public void SaveBalanceAfterDeal() {
        SaveBalance();
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

    public void Deal() {
        if (!HasPlacedBet()) {
            Debug.Log("You must place a bet before dealing.");
            return;
        }
        manager.ShowGameplayUI();
        SaveBalanceAfterDeal();
        dealer.DealCards();
        Debug.Log("Dealing cards...");
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
