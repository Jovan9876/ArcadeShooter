using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using System.Threading.Tasks;
using static UnityEngine.Rendering.GPUSort;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Dealer : BlackjackManager {

    private List<GameObject> winningChips = new List<GameObject>();
    private bool newShoe = true;

    public void DealCards() {
        gameStarted = true;
        if (newShoe) {
            NewDeck();
            StartNewShoe();
        } else {
            StartNewRound();
        }
    }

    private void StartNewShoe() {
        newShoe = false;
        ClearDiscardPile();

        Card burnCard = deck.BurnCard();
        burnCard.transform.SetParent(discardPile.transform, false);
        burnCard.transform.localPosition = new Vector3(0, 0, 0);
        burnCard.transform.localRotation = Quaternion.Euler(90, 0, 0);
        burnCard.transform.localScale = new Vector3(5f, 5f, 5f);


        playerHand.AddCard(deck.DrawCard(), false);
        dealerHand.AddCard(deck.DrawCard(), false);
        playerHand.AddCard(deck.DrawCard(), false);
        dealerHand.AddCard(deck.DrawCard(), true);

        CheckBlackjack();
    }


    public void PlayerHit() {
        Card drawnCard = deck.DrawCard();

        playerHand.AddCard(drawnCard, false);
        Debug.Log($"Player drew: {drawnCard.rank} of {drawnCard.suit}");
        Debug.Log($"Player Score After Hit: {playerHand.GetScore()}");

        if (playerHand.GetScore() > 21) {
            PlayerStand();
        }
    }

    async private Task HandleEndOfRound() {
        await EndRound();
    }

    async public Task PlayerStand() {
        await PlayTurn();
    }

    async public Task PlayTurn() {
        await DealerTurn();
    }

    async private Task DealerTurn() {
        dealerHand.FlipOver();

        while (dealerHand.GetScore() < 17 || (dealerHand.GetScore() == 17 && dealerHand.HasSoft17())) {
            await Task.Delay(1000);
            dealerHand.AddCard(deck.DrawCard(), false);
        }
        HandleEndOfRound();
    }

    async private Task EndRound() {
        float multiplier = DetermineWinner();
        await Task.Delay(1500);

        if (multiplier == 0f) {
            PlayerLose();
        } else if (multiplier == 1f) {
            PlayerPush();
        } else if (multiplier == 2f) {
            PlayerWin(multiplier);
        } else if (multiplier == 2.5f) {
            PlayerWin(multiplier);
        }

    }

    private void ClearChips() {
        foreach (GameObject chip in player.placedChips) {
            Destroy(chip);
        }
        player.placedChips.Clear();

        foreach (GameObject chip in winningChips) {
            Destroy(chip);
        }
        winningChips.Clear();
    }

    async public Task PlayerWin(float multiplier) {

        PlaceWinningChips(multiplier);
        player.WinBet(multiplier);
        await Task.Delay(1000);
        ClearChips();
        ClearHands();

    }

    async public Task PlayerLose() {
        await Task.Delay(1000);
        ClearChips();
        ClearHands();
        player.LoseBet();
    }

    async public Task PlayerPush() {
        await Task.Delay(1000);
        ClearChips();
        ClearHands();
        player.PushBet();
    }

    protected void PlaceWinningChips(float multiplier) {

        foreach (GameObject chip in player.placedChips) {
            int chipValue = chip.GetComponent<Chip>().chipValue;
            int totalChipsToPlace = Mathf.RoundToInt(multiplier - 1);

            for (int i = 0; i < totalChipsToPlace; i++) {
                GameObject winningChip = Instantiate(chip, player.bettingArea.transform, false);
                winningChip.transform.localPosition = new Vector3(0.3f, chip.transform.localPosition.y + 0.02f, 0f);
                winningChip.transform.localRotation = Quaternion.Euler(90, 0, 0);

                Chip chipComponent = winningChip.GetComponent<Chip>();
                if (chipComponent != null) {
                    chipComponent.chipValue = chipValue;
                }

                winningChips.Add(winningChip);

            }
        }
    }


    private void ClearHands() {
        // Clear Player's Hand
        List<Transform> playerChildren = new List<Transform>();
        foreach (Transform child in playerHand.transform) {
            playerChildren.Add(child);
        }
        foreach (Transform child in playerChildren) {
            float offsetY = 0.01f * discardPile.transform.childCount;
            child.SetParent(discardPile.transform, false);
            child.localPosition = new Vector3(0, offsetY, 0);
            child.localRotation = Quaternion.Euler(90, 0, 0);
            child.localScale = new Vector3(5f, 5f, 5f);
        }
        playerHand.cards.Clear();

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

    private void ClearDiscardPile() {
        List<Transform> discardChildren = new List<Transform>();
        foreach (Transform child in discardPile.transform) {
            discardChildren.Add(child);
        }
        foreach (Transform child in discardChildren) {
            Destroy(child.gameObject);
        }
    }

    public void StartBettingPhase() {
        gameStarted = false;
        player.ResetBet();
        Debug.Log("Place your bets before dealing.");
    }


    private void StartNewRound() {

        if (deck.NeedsNewShoe()) {
            NewDeck();
            StartNewShoe();
        }

        playerHand.AddCard(deck.DrawCard(), false);
        dealerHand.AddCard(deck.DrawCard(), false);
        playerHand.AddCard(deck.DrawCard(), false);
        dealerHand.AddCard(deck.DrawCard(), true);
        CheckBlackjack();
    }

    private void CheckBlackjack() {
        if (dealerHand.ShowingFaceOrAce()) {
            Debug.Log("DEALER SHOWING FACE CHECKING BJ");
            if (dealerHand.HasBlackjack()) {
                dealerHand.FlipOver();
                if (playerHand.HasBlackjack()) {
                    Debug.Log("Both BJ push");
                } else {
                    Debug.Log("Dealer BJ win player lose");
                }
                HandleEndOfRound();
                return;
            }
        }
        if (playerHand.HasBlackjack()) {
            Debug.Log("Player BJ win!");
            PlayerStand();
        }
    }

}
