using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {
    public List<Card> cards = new List<Card>();

    public void AddCard(Card card, bool faceDown) {
        cards.Add(card);
    }

    public void ClearHand() {
        cards.Clear();
    }

    public int GetScore() {
        int score = 0;
        int aceCount = 0;

        foreach (Card card in cards) {
            int cardValue = card.GetValue(score);
            score += cardValue;
            if (card.rank == CardEnums.Rank.Ace) {
                aceCount++;
            }
        }

        while (score > 21 && aceCount > 0) {
            score -= 10;
            aceCount--;
        }

        return score;
    }
}
