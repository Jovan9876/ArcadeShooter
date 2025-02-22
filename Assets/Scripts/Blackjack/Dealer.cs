using System.Collections;
using UnityEngine;

public class Dealer : BlackJackManager {
    public void PlayTurn() {
        DealerTurn();
        OnDealerTurnEnd();
    }

    private void DealerTurn() {
        while (dealerHand.GetScore() < 17) {
            dealerHand.AddCard(deck.DrawCard(), false);
        }
    }

    public void StartNewRound() {
        StartGame();
    }
}
