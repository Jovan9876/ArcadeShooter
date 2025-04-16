using System.Collections.Generic;
using UnityEngine;

// Manages the cards, bet, and state of a single blackjack hand of the player
public class PlayerHand : MonoBehaviour {

    [Header("Hand Settings")]
    public List<Card> cards = new List<Card>();       // List of cards in this hand
    public GameObject bettingArea;                    // Reference to where chips are placed

    [Header("Betting Info")]
    public int bet = 0;                               // Current bet amount on this hand
    public bool isStood = false;                      // Whether the player has chosen to stand

    public void AddCard(Card card, bool last = false) {
        // Adds a card to the hand a positions it
        cards.Add(card);

        card.transform.SetParent(transform, true);

        float offsetX = 0.1f * cards.Count;
        float offsetY = 0.01f * cards.Count;
        float offsetZ = 0.1f * cards.Count;

        if (last) {
            card.transform.localRotation = Quaternion.Euler(-90, 90, 0);
        } else {
            card.transform.localRotation = Quaternion.Euler(-90, 0, 0);
        }
        card.transform.localPosition = new Vector3(offsetX, offsetY, offsetZ);
    }

    public int GetScore() {
        // Calculates current score of the hand
        int score = 0;
        int aceCount = 0;

        foreach (Card card in cards) {

            if (card.rank == CardEnums.Rank.Ace) {
                ++aceCount;
            } else {
                score += card.GetValue(score);
            }

        }
        for (int i = 0; i < aceCount; ++i) {
            if (score + 11 > 21) {
                score += 1;
            } else {
                score += 11;
            }
        }

        return score;
    }

    public bool HasBlackjack() {
        // Checks if the hand is a natural blackjack (21 with 2 cards)
        return cards.Count == 2 && GetScore() == 21;
    }

    public void IncrementBet(int amount) {
        // Increases the current bet
        bet += amount;
    }

    public void DecrementBet(int amount) {
        // Decreases the current bet
        bet -= amount;
    }

    public void Reset() {
        // Resets the bet
        bet = 0;
    }

}