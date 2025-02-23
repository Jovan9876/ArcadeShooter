using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Dealer : BlackjackManager {

    private void Start() {
        NewDeck();
        StartNewShoe();
    }

    private void StartNewShoe() {
        ClearHands();

        deck.BurnCard();

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
            HandleEndOfRound();
        }
    }

    private void HandleEndOfRound() {
        DetermineWinner();
        StartCoroutine(EndRound());

        //EndRound();
    }

    public void PlayerStand() {
        dealerHand.FlipOver();
        PlayTurn();
    }

    public void PlayTurn() {
        DealerTurn();
        HandleEndOfRound();
    }

    private void DealerTurn() {
        while (dealerHand.GetScore() < 17 || (dealerHand.GetScore() == 17 && dealerHand.HasSoft17())) {
            dealerHand.AddCard(deck.DrawCard(), false);
        }
    }

    private IEnumerator EndRound() {
        ClearHands();
        yield return new WaitForSeconds(2f);

        HideUI();
        StartNewRound();
    }

    private void ClearHands() {
        dealerHand.ClearHand();
        playerHand.ClearHand();
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
            HandleEndOfRound();
        }
    }

}
