using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class Player : MonoBehaviour {
    private Vector3 originalHandPosition;
    [SerializeField] public PlayerHand playerHand;
    [SerializeField] public GameObject bettingArea;

    [SerializeField] private Dealer dealer;
    [SerializeField] private UIManager manager;

    [SerializeField] public List<PlayerHand> extraHands = new List<PlayerHand>();
    public List<PlayerHand> activeSplitHands = new List<PlayerHand>();
    public int currentSplitHands = 0;
    public int currentHandIndex = 0;

    private int balance = 0;
    public int currentBet = 0;
    public bool betPlaced = false;

    private const string BALANCE_KEY = "Player_Currency";

    [SerializeField] private GameObject[] chipPrefabs;
    public List<GameObject> placedChips = new List<GameObject>();

    private void Start() {
        LoadBalance();
        originalHandPosition = playerHand.transform.position;
    }

    private void Update() {
        manager.UpdatePlayerBalance(balance);
        manager.UpdateBetAmount(currentBet);
        betPlaced = currentBet > 0;
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
        playerHand.IncrementBet(chipValue);
        currentBet += chipValue;
        balance -= chipValue;
        PlaceBetOnTable(chip);
        Debug.Log($"Player placed a bet of {chipValue}. New balance: {balance}");
    }

    public void RemoveBet(Chip chip) {
        if (placedChips.Contains(chip.gameObject)) {
            int chipValue = chip.chipValue;
            placedChips.Remove(chip.gameObject);
            Destroy(chip.gameObject);
            playerHand.DecrementBet(chipValue);
            currentBet -= chipValue;
            balance += chipValue;
            SaveBalance();
            Debug.Log($"Removed a {chip.chipValue} chip. New balance: {balance}");
            ReorderChips();
        }
    }

    private void PlaceBetOnTable(Chip chip) {
        if (playerHand.bettingArea == null) {
            Debug.LogError("Betting area not set up!");
            return;
        }

        GameObject chipInstance = Instantiate(chipPrefabs[chip.chipIndex], playerHand.bettingArea.transform, false);
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
        if (!betPlaced) {
            Debug.Log("You must place a bet before dealing.");
            return;
        }

        foreach (GameObject chip in placedChips) {
            chip.GetComponent<Chip>().IsBet = true;
        }

        manager.ShowGameplayUI();
        SaveBalanceAfterDeal();
        dealer.DealCards();
        Debug.Log("Dealing cards...");
        UpdateDoubleDownButton();
    }

    public void Hit() {
        if (!betPlaced) return;
        //if (dealer.playerStand) return;

        // Get the currently active hand
        PlayerHand currentHand = GetCurrentHand();

        if (currentHand.GetScore() >= 21) {
            Stand();
            return;
        }

        dealer.PlayerHit(currentHand);
        UpdateDoubleDownButton();

    }
    public void Stand() {
        if (currentHandIndex < activeSplitHands.Count) {
            // Move to the next split hand
            currentHandIndex++;
            PlayerHand nextHand = GetCurrentHand();
            dealer.PlayerHit(nextHand);
        } else {
            // If all hands have played, dealer takes their turn
            dealer.PlayerStand();
        }
    }

    public void DoubleDown() {
        //if (dealer.playerStand) return;

        if (balance >= currentBet && playerHand.cards.Count == 2) {
            balance -= currentBet;
            playerHand.IncrementBet(currentBet);
            currentBet *= 2;
            SaveBalance();
            PlaceDoubleDownChips();
            dealer.PlayerDouble();
            UpdateDoubleDownButton();
        }

    }

    public void Split() {
        if (activeSplitHands.Count >= 4) return; // Max 4 split hands

        // Check if the original hand can be split
        if (playerHand.cards.Count != 2 || playerHand.cards[0].rank != playerHand.cards[1].rank) {
            Debug.Log("Cannot split unless both cards are the same rank!");
            return;
        }

        if (balance < playerHand.bet) {
            Debug.Log("Not enough balance to split.");
            return;
        }

        // Select an available extra hand from the pool
        PlayerHand newHand = extraHands[currentSplitHands];
        currentSplitHands++;

        // Store the second card before removing it
        Card movedCard = playerHand.cards[1];

        // Move one card to the new hand
        playerHand.cards.RemoveAt(1);
        newHand.AddCard(movedCard);

        // Assign the same bet to the new hand
        newHand.bet = playerHand.bet;
        balance -= newHand.bet;

        // Assign a betting GameObject to the new hand
        newHand.bettingArea = Instantiate(playerHand.bettingArea, newHand.transform);

        // Update the active split hands list
        activeSplitHands.Add(newHand);
        dealer.PlayerHit(playerHand);
        RepositionHands();
    }


    private PlayerHand GetCurrentHand() {
        if (currentHandIndex == 0) return playerHand; // Original hand
        return activeSplitHands[currentHandIndex - 1]; // Adjust index for split hands
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

    }

    private void RepositionHands() {
        int totalHands = activeSplitHands.Count + 1;
        float originalX = playerHand.transform.position.x;
        float spacing = 0.5f; // Spacing between cards

        // Max shift left for original hand
        float maxShiftLeft = 0.25f;
        float shiftLeft = Mathf.Min((totalHands - 1) * spacing * 0.5f, maxShiftLeft);

        // Adjust original hand position
        playerHand.transform.position = new Vector3(originalX - shiftLeft, playerHand.transform.position.y, playerHand.transform.position.z);

        // Position each split hand to the right of the adjusted original hand
        for (int i = 0; i < activeSplitHands.Count; i++) {
            float newX = playerHand.transform.position.x + ((i + 1) * spacing);
            activeSplitHands[i].transform.position = new Vector3(newX, activeSplitHands[i].transform.position.y, activeSplitHands[i].transform.position.z);
        }
    }

    public List<PlayerHand> GetAllSplitHands() {
        return activeSplitHands;
    }

    public void UpdateDoubleDownButton() {
        manager.ToggleDoubleDown(playerHand.cards.Count == 2);
    }

    public void ResetOriginalHandPosition() {
        playerHand.transform.position = originalHandPosition;
    }

}
