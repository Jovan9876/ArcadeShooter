using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using System.Threading.Tasks;

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
