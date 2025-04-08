using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Threading.Tasks;
using Unity.VisualScripting;

// Handles player logic including betting, dealing, hitting, standing, doubling down,
// splitting hands, and managing chip placements and payouts.
public class Player : MonoBehaviour {

    [Header("References")]
    [SerializeField] private Dealer dealer;
    [SerializeField] private UIManager manager;
    [SerializeField] public PlayerHand playerHand;
    [SerializeField] public GameObject bettingArea;
    [SerializeField] private GameObject[] chipPrefabs;

    // Runtime Data
    private PlayerData playerData;
    private Vector3 originalHandPosition;

    [Header("Hand Management")]
    public List<PlayerHand> extraHands = new List<PlayerHand>();                   // Pool of reusable hands for splits
    public List<PlayerHand> activeSplitHands = new List<PlayerHand>();             // Hands currently in play due to splits
    public int currentSplitHands = 0;                                              // Number of splits done this round
    public int currentHandIndex = 0;                                               // Index of the active hand being played

    [Header("Betting")]
    public int currentBet = 0;
    public bool betPlaced = false;
    public List<GameObject> placedChips = new List<GameObject>(); // List of chips currently placed on the table

    private void Start() {
        playerData = SaveSystem.LoadProgress();
        if (playerData == null) {
            Debug.LogWarning("PlayerData not found. Creating new instance.");
            playerData = new PlayerData();
        }
        originalHandPosition = playerHand.transform.position;
    }

    private void Update() {
        manager.UpdatePlayerBalance(playerData.balance);
        manager.UpdateBetAmount(currentBet);
        betPlaced = currentBet > 0;
    }

    private void SaveBalance() {
        // Save the players current balance and progress
        SaveSystem.SaveProgress(playerData);
    }

    public void PlaceBet(Chip chip) {
        // Places a chip on the table if player has enough money
        int chipValue = chip.chipValue;
        if (chipValue > playerData.balance) {
            Debug.Log("Not enough playerData.balance to place bet.");
            return;
        }
        playerHand.IncrementBet(chipValue);
        currentBet += chipValue;

        CommitGamble(chipValue);

        PlaceBetOnTable(chip);
        Debug.Log($"Player placed a bet of {chipValue}. New playerData.balance: {playerData.balance}");
    }

    public void RemoveBet(Chip chip) {
        // Removes a chip from the table and refunds the player
        if (placedChips.Contains(chip.gameObject)) {
            int chipValue = chip.chipValue;
            placedChips.Remove(chip.gameObject);
            Destroy(chip.gameObject);
            playerHand.DecrementBet(chipValue);
            currentBet -= chipValue;

            RefundGamble(chipValue);

            SaveBalance();
            Debug.Log($"Removed a {chip.chipValue} chip. New playerData.balance: {playerData.balance}");
            ReorderChips();
        }
    }

    private void PlaceBetOnTable(Chip chip) {
        // Visually positions the placed chips
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
        // Reorders the placed chips so the highest value chip is always at the bottom ASC order
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
        playerHand.DecrementBet(playerHand.bet);
        manager.ShowBettingUI();
    }



    public void Deal() {
        // Deal cards for a round of blackjack
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
        UpdateSplitButton();
    }

    public void Hit() {
        // Deal a card to the players hand
        if (!dealer.isPlayerTurn) return;
        if (!betPlaced) return;

        // Get the currently active hand
        PlayerHand currentHand = GetCurrentHand();

        if (currentHand.GetScore() >= 21) {
            Stand();
            return;
        }

        dealer.PlayerHit(currentHand);
        UpdateDoubleDownButton();
        UpdateSplitButton();
    }

    public void Stand() {
        // Stand the current hand and then let dealer play his hand
        PlayerHand currentHand = GetCurrentHand();

        // Prevent spamming Stand on the same hand
        if (currentHand.isStood) {
            Debug.Log("This hand has already stood.");
            return;
        }

        currentHand.isStood = true;

        if (currentHandIndex < activeSplitHands.Count) {
            // Move to the next split hand
            currentHandIndex++;
            PlayerHand nextHand = GetCurrentHand();
            dealer.PlayerHit(nextHand);
            UpdateSplitButton();
            UpdateDoubleDownButton();
        } else {
            // If all hands have played, dealer takes their turn
            manager.HideGameplayUI();
            dealer.PlayerStand();
        }
    }

    public void DoubleDown() {
        // Double your bet for 1 final card
        if (!dealer.isPlayerTurn) return;
        PlayerHand currentHand = GetCurrentHand();

        // Only allow if hand has exactly 2 cards and player has enough playerData.balance
        if (currentHand.cards.Count == 2 && playerData.balance >= currentHand.bet) {
            currentBet += currentHand.bet;
            currentHand.IncrementBet(currentHand.bet); // Double the bet

            CommitGamble(currentHand.bet);

            SaveBalance();

            PlaceDoubleDownChips(currentHand); // Pass current hand's betting area
            dealer.PlayerDouble(currentHand);     // Deal only one card
        }

    }

    public void Split() {
        // Split your hand into two hands if the cards are of the same value
        if (!dealer.isPlayerTurn) return;
        if (activeSplitHands.Count >= 4) return; // Max 4 split hands

        // Check if the original hand can be split
        if (playerHand.cards.Count != 2 || playerHand.cards[0].rank != playerHand.cards[1].rank) {
            Debug.Log("Cannot split unless both cards are the same rank!");
            return;
        }

        if (playerData.balance < GetCurrentHand().bet) {
            Debug.Log("Not enough playerData.balance to split.");
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
        int tempBet = GetCurrentHand().bet;
        CommitGamble(tempBet);
        newHand.IncrementBet(tempBet);
        currentBet += newHand.bet;

        // Assign a betting GameObject to the new hand
        newHand.bettingArea = Instantiate(playerHand.bettingArea, newHand.transform);

        // Update the active split hands list
        activeSplitHands.Add(newHand);
        dealer.PlayerHit(playerHand);
        RepositionHands();
    }


    private PlayerHand GetCurrentHand() {
        // Gets the current active hand and returns it
        if (currentHandIndex == 0) return playerHand; // Original hand
        return activeSplitHands[currentHandIndex - 1]; // Adjust index for split hands
    }


    private void PlaceDoubleDownChips(PlayerHand hand) {
        // Positions the double down chips behind the original bet

        List<GameObject> originalChips = new List<GameObject>(); // Store original chips separately

        foreach (Transform chip in hand.bettingArea.transform) {
            originalChips.Add(chip.gameObject);
        }

        foreach (GameObject chip in originalChips) {
            Chip chipComponent = chip.GetComponent<Chip>();
            if (chipComponent == null) continue;

            GameObject chipInstance = Instantiate(chipPrefabs[chipComponent.chipIndex], hand.bettingArea.transform, false);
            Destroy(chipInstance.GetComponentInChildren<UniversalAdditionalLightData>());
            Destroy(chipInstance.GetComponentInChildren<Light>());

            chipInstance.transform.localPosition = chip.transform.localPosition + new Vector3(0, 0, -0.23f);
            chipInstance.transform.localRotation = chip.transform.localRotation;

            placedChips.Add(chipInstance);
        }
    }

    private void RepositionHands() {
        // Handles the spacing between all split hands
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

    public void ApplyHandPayout(PlayerHand hand, float multiplier) {
        // Calculate won amount
        if (multiplier == 0f) {
            Debug.Log($"Hand lost. Bet was {hand.bet}");
        } else if (multiplier == 1f) {
            Debug.Log($"Hand pushed. Returning {hand.bet}");
            playerData.balance += hand.bet;
        } else {
            int winnings = Mathf.RoundToInt(hand.bet * multiplier);
            Debug.Log($"Hand won. Payout: {winnings} for bet {hand.bet}");
            playerData.blackjackProgress.AddWinnings(winnings - hand.bet);
            playerData.balance += winnings;
        }
        SaveBalance();
    }

    public List<PlayerHand> GetAllSplitHands() {
        // Get how many split hands are active
        return activeSplitHands;
    }

    public void UpdateDoubleDownButton() {
        // Show / Hide the double down button if it is or is not possible to double
        PlayerHand currentHand = GetCurrentHand();
        manager.ToggleDoubleDown(currentHand.cards.Count == 2 && playerData.balance >= currentHand.bet);
    }

    public void UpdateSplitButton() {
        // Show / Hide the double down button if it is or is not possible to split
        PlayerHand currentHand = GetCurrentHand();
        manager.ToggleSplit(currentHand.cards.Count == 2 &&
                    currentHand.cards[0].rank == currentHand.cards[1].rank &&
                    playerData.balance >= currentHand.bet);
    }

    public void ResetOriginalHandPosition() {
        // After a split reset the original hands position
        playerHand.transform.position = originalHandPosition;
    }

    private void CommitGamble(int amount) {
        playerData.balance -= amount;
        playerData.blackjackProgress.AddGamble(amount);
    }

    private void RefundGamble(int amount) {
        playerData.balance += amount;
        playerData.blackjackProgress.RemoveGamble(amount);
    }

}
