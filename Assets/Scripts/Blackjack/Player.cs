using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class Player : MonoBehaviour {
    [SerializeField] private PlayerHand playerHand;
    [SerializeField] private Dealer dealer;
    [SerializeField] private UIManager manager;
    [SerializeField] public GameObject bettingArea;


    private int balance = 0;
    public int currentBet = 0;
    public bool betPlaced = false;

    private const string BALANCE_KEY = "Player_Currency";

    [SerializeField] private GameObject[] chipPrefabs;
    public List<GameObject> placedChips = new List<GameObject>();


    private void Start() {
        LoadBalance();
    }

    private void Update() {
        manager.UpdatePlayerBalance(balance);
        manager.UpdateBetAmount(currentBet);
        if (currentBet <= 0) {
            betPlaced = false;
        } else {
            betPlaced = true;
        }
    }

    private void LoadBalance() {
        balance = PlayerPrefs.GetInt(BALANCE_KEY, 10000);
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
        //betPlaced = true;
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

    async public Task ResetBet() {
        await Task.Delay(1000);
        currentBet = 0;
        manager.ShowBettingUI();
    }

    async public void WinBet(float multiplier) {
        int winnings = Mathf.RoundToInt(currentBet * multiplier);
        currentBet = winnings;
        balance += winnings;
        SaveBalance();
        Debug.Log($"Player won {winnings}. New balance: {balance}");
        await ResetBet();
    }

    async public void LoseBet() {
        Debug.Log($"Player lost {currentBet}. New balance: {balance}");
        SaveBalance();
        await ResetBet();
    }

    async public void PushBet() {

        balance += currentBet;
        SaveBalance();
        Debug.Log($"Push! Bet returned. New balance: {balance}");

        await ResetBet();
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
        UpdateDoubleDownButton();
    }

    public void Hit() {
        if (!betPlaced) return;
        if (dealer.playerStand) return;
        if (playerHand.GetScore() >= 21) Stand();
        dealer.PlayerHit();
        UpdateDoubleDownButton();
    }

    public void Stand() {
        dealer.PlayerStand();
    }

    public void DoubleDown() {
        if (dealer.playerStand) return;

        if (balance >= currentBet && playerHand.cards.Count == 2) {
            balance -= currentBet;
            currentBet *= 2;
            SaveBalance();
            PlaceDoubleDownChips();
            dealer.PlayerDouble();
            UpdateDoubleDownButton();
        }

    }

    private void PlaceDoubleDownChips() {
        List<GameObject> originalChips = new List<GameObject>(placedChips); // Store original chips separately

        foreach (GameObject chip in originalChips) {
            GameObject chipInstance = Instantiate(chipPrefabs[chip.GetComponent<Chip>().chipIndex], bettingArea.transform, false);
            Destroy(chipInstance.GetComponentInChildren<UniversalAdditionalLightData>());
            Destroy(chipInstance.GetComponentInChildren<Light>());

            // Place behind the original chip
            chipInstance.transform.localPosition = chip.transform.localPosition + new Vector3(0, 0, -0.23f);
            chipInstance.transform.localRotation = chip.transform.localRotation;

            placedChips.Add(chipInstance);
        }

        //ReorderChips();
    }


    public void UpdateDoubleDownButton() {
        manager.ToggleDoubleDown(playerHand.cards.Count == 2);
    }


}
