using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using System.Threading.Tasks;
using static UnityEngine.Rendering.GPUSort;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.XR;

// Inherits from BlackjackManager to access shared game logic like deck handling and winner evaluation
public class Dealer : BlackjackManager {

    private List<GameObject> winningChips = new List<GameObject>();
    private bool newShoe = true;

    // Indicates if it's currently the player's turn (used to prevent premature dealer logic)
    public bool isPlayerTurn { get; private set; } = true;

    public void DealCards() {
        // Begins a new round or shoe depending on the flag.
        gameStarted = true;
        if (newShoe) {
            NewDeck();
            StartNewShoe();
        } else {
            StartNewRound();
        }
    }

    async private Task StartNewShoe() {
        // Starts a new shoe by clearing hands and discards, burning a card, and dealing cards
        newShoe = false;
        await ClearHands(false);
        await ClearDiscardPile();

        // Burn the top card of the deck
        Card burnCard = deck.BurnCard();
        burnCard.transform.SetParent(discardPile.transform, false);
        burnCard.transform.localPosition = new Vector3(0, 0, 0);
        burnCard.transform.localRotation = Quaternion.Euler(90, 0, 0);
        burnCard.transform.localScale = new Vector3(5f, 5f, 5f);

        // Deal initial cards
        player.playerHand.AddCard(deck.DrawCard(), false);
        dealerHand.AddCard(deck.DrawCard(), false);
        player.playerHand.AddCard(deck.DrawCard(), false);
        dealerHand.AddCard(deck.DrawCard(), true);

        await CheckBlackjack();
    }


    public void PlayerHit(PlayerHand currentHand) {
        // Handles the player's hit action by drawing and adding a card
        if (!isPlayerTurn) return;
        if (!gameStarted) return;
        if (currentHand.GetScore() >= 21) return;

        Card drawnCard = deck.DrawCard();
        currentHand.AddCard(drawnCard, false);

        Debug.Log($"Player drew: {drawnCard.rank} of {drawnCard.suit}");
        Debug.Log($"Player Score After Hit: {currentHand.GetScore()}");

        // Auto-stand if bust
        if (currentHand.GetScore() > 21) {
            player.Stand();
        }

    }
    async public Task PlayerStand() {
        // Ends the player's turn and starts the dealer's turn
        isPlayerTurn = false;
        await PlayTurn();
    }

    async public Task PlayerDouble(PlayerHand hand) {
        // Handles the double down logic: draw one card and auto-stand
        if (!gameStarted) return;
        if (!isPlayerTurn) return;

        Card drawnCard = deck.DrawCard();
        hand.AddCard(drawnCard, true);

        Debug.Log($"Player doubled down and drew: {drawnCard.rank} of {drawnCard.suit}");
        Debug.Log($"Player Score After Double: {hand.GetScore()}");

        // Automatically stand after doubling
        player.Stand();
    }

    async private Task HandleEndOfRound() {
        await EndRound();
    }


    async public Task PlayTurn() {
        // Begins the dealer's turn logic
        await DealerTurn();
    }

    async private Task DealerTurn() {
        // Dealer flips their second card and draws until they reach 17 or higher
        dealerHand.FlipOver();

        while (dealerHand.GetScore() < 17 || (dealerHand.GetScore() == 17 && dealerHand.HasSoft17())) {
            await Task.Delay(1000);
            dealerHand.AddCard(deck.DrawCard(), false);
        }
        await HandleEndOfRound();
    }

    async private Task EndRound() {
        // Final logic for comparing hands and awarding payouts
        await Task.Delay(1500);

        List<PlayerHand> allHands = new List<PlayerHand> { player.playerHand };
        allHands.AddRange(player.activeSplitHands);

        foreach (PlayerHand hand in allHands) {
            float multiplier = DetermineWinner(hand);
            Debug.Log($"Hand evaluated with multiplier: {multiplier}");

            // Add chips to won hands
            if (multiplier > 1f) {
                PlaceWinningChips(multiplier, hand);
            }

            // Apply balance update
            player.ApplyHandPayout(hand, multiplier);

            // Clear chips if player lost
            if (multiplier == 0f) {
                ClearHandChips(hand);
            }

        }

        await Task.Delay(1000);
        await ClearChips();
        await ClearHands();
        StartBettingPhase();

    }

    private void ClearHandChips(PlayerHand hand) {
        List<GameObject> chipsToClear = new List<GameObject>();

        foreach (Transform chip in hand.bettingArea.transform) {
            chipsToClear.Add(chip.gameObject);
        }

        foreach (GameObject chip in chipsToClear) {
            Destroy(chip);
            player.placedChips.Remove(chip);
        }
    }

    async private Task ClearChips() {
        // Destroys all chip GameObjects at the end of a round (both bets and winnings)
        // Destroy Player's Split Hands Bets
        List<GameObject> playerSplitBettingAreas = new List<GameObject>();
        foreach (PlayerHand hand in player.activeSplitHands) {
            playerSplitBettingAreas.Add(hand.bettingArea);
            hand.bettingArea = null;
        }
        foreach (GameObject child in playerSplitBettingAreas) {
            Destroy(child);
        }

        // Destroy Player's Original Hand Bet
        foreach (Transform chip in player.playerHand.bettingArea.transform) {
            GameObject chipObject = chip.gameObject;
            Destroy(chipObject);
        }

        foreach (GameObject chip in winningChips) {
            Destroy(chip);
        }
        winningChips.Clear();
        player.placedChips.Clear();
    }

    protected void PlaceWinningChips(float multiplier, PlayerHand hand) {
        // Instantiates extra chips to visually show the player’s winnings

        List<GameObject> allBetChips = new List<GameObject>();

        // Get all chips from this hand's betting area
        foreach (Transform chipTransform in hand.bettingArea.transform) {
            GameObject chipObj = chipTransform.gameObject;
            allBetChips.Add(chipObj);
        }

        foreach (GameObject chip in allBetChips) {
            Chip chipComponent = chip.GetComponent<Chip>();
            if (chipComponent == null) continue;

            int chipValue = chipComponent.chipValue;
            int totalChipsToPlace = Mathf.RoundToInt(multiplier - 1); // Extra chips only

            for (int i = 0; i < totalChipsToPlace; i++) {
                GameObject winningChip = Instantiate(chip, hand.bettingArea.transform, false);

                // Set winning chips to the side of the bet chips
                winningChip.transform.localPosition = chip.transform.localPosition + new Vector3(0.25f, 0, 0);
                winningChip.transform.localRotation = chip.transform.localRotation;

                Chip newChipComponent = winningChip.GetComponent<Chip>();
                if (newChipComponent != null) {
                    newChipComponent.chipValue = chipValue;
                }

                winningChips.Add(winningChip);
            }
        }
    }


    async private Task ClearHands(bool shouldClearBet = true) {
        // Moves all cards to the discard pile and resets hands.
        // Clear Player's Original Hand
        List<Transform> playersOriginalChildren = new List<Transform>();
        foreach (Transform child in playerHand.transform) {
            if (child.name != "Bet") {
                playersOriginalChildren.Add(child);
            }
        }
        foreach (Transform child in playersOriginalChildren) {
            float offsetY = 0.01f * discardPile.transform.childCount;
            child.SetParent(discardPile.transform, false);
            child.localPosition = new Vector3(0, offsetY, 0);
            child.localRotation = Quaternion.Euler(90, 0, 0);
            child.localScale = new Vector3(5f, 5f, 5f);
        }
        playerHand.cards.Clear();
        if (shouldClearBet) {
            playerHand.Reset();
        }
        playerHand.isStood = false;

        // Move player's original hand back to its original position
        player.ResetOriginalHandPosition();

        // Clear Player's Split Hands
        List<Transform> playerSplitChildren = new List<Transform>();
        foreach (PlayerHand hand in player.activeSplitHands) {
            foreach (Transform child in hand.transform) {
                if (child.name != "Bet") {
                    playerSplitChildren.Add(child);
                }
            }
            hand.cards.Clear();
            if (shouldClearBet) {
                hand.Reset();
            }
            hand.isStood = false;
        }
        foreach (Transform child in playerSplitChildren) {
            float offsetY = 0.01f * discardPile.transform.childCount;
            child.SetParent(discardPile.transform, false);
            child.localPosition = new Vector3(0, offsetY, 0);
            child.localRotation = Quaternion.Euler(90, 0, 0);
            child.localScale = new Vector3(5f, 5f, 5f);
        }

        player.activeSplitHands.Clear();
        player.currentHandIndex = 0; // Reset back to the first hand
        player.currentSplitHands = 0; // Reset back to the first hand


        // Clear Dealer's Hand
        List<Transform> dealerChildren = new List<Transform>();
        foreach (Transform child in dealerHand.transform) {
            dealerChildren.Add(child);
        }
        foreach (Transform child in dealerChildren) {
            float offsetY = 0.01f * discardPile.transform.childCount;
            child.SetParent(discardPile.transform, false);
            child.localPosition = new Vector3(0, offsetY, 0);
            child.localRotation = Quaternion.Euler(90, 0, 0);
            child.localScale = new Vector3(5f, 5f, 5f);
        }
        dealerHand.cards.Clear();
    }

    async private Task ClearDiscardPile() {
        // Destroys all cards in the discard pile
        List<Transform> discardChildren = new List<Transform>();
        foreach (Transform child in discardPile.transform) {
            discardChildren.Add(child);
        }
        foreach (Transform child in discardChildren) {
            Destroy(child.gameObject);
        }
    }

    public void StartBettingPhase() {
        // Resets for a new betting phase
        gameStarted = false;
        player.ResetBet();
        Debug.Log("Place your bets before dealing.");
    }


    async private Task StartNewRound() {
        // Starts a normal new round (not a new shoe)
        isPlayerTurn = true;
        if (deck.NeedsNewShoe()) {
            NewDeck();
            StartNewShoe();
            return;
        }

        player.playerHand.AddCard(deck.DrawCard(), false);
        dealerHand.AddCard(deck.DrawCard(), false);
        player.playerHand.AddCard(deck.DrawCard(), false);
        dealerHand.AddCard(deck.DrawCard(), true);
        await CheckBlackjack();
    }

    async private Task CheckBlackjack() {
        // Checks for player or dealer Blackjack before the turn proceeds
        if (dealerHand.HasBlackjack()) {
            dealerHand.FlipOver();
            if (playerHand.HasBlackjack()) {
                Debug.Log("Both BJ push");
            } else {
                Debug.Log("Dealer BJ win player lose");
                HandleEndOfRound();
            }
        }
        if (player.playerHand.HasBlackjack()) {
            Debug.Log("Player BJ win!");
            PlayerStand();
        }
    }

}
