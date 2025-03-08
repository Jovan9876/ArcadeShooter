using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using System.Threading.Tasks;
using static UnityEngine.Rendering.GPUSort;
using System;
using System.Collections.Generic;

public class Dealer : BlackjackManager {

    private void Start() {
        NewDeck();
        StartNewShoe();
    }

    private void StartNewShoe() {
        //ClearHands();
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

    private void HandleEndOfRound() {
        //StartCoroutine(EndRound());

        EndRound();
    }

    public void PlayerStand() {
        PlayTurn();
    }

    public void PlayTurn() {
        DealerTurn();
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
        await Task.Delay(2000);
        DetermineWinner();
        await Task.Delay(1000);
        ClearHands();
        //HideUI();
        StartNewRound();
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
