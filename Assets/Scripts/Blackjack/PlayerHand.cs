using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour {
    public List<Card> cards = new List<Card>();
    public GameObject bettingArea;
    public int bet = 0;
    public bool isStood = false;

    public void AddCard(Card card, bool last = false) {
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
        return cards.Count == 2 && GetScore() == 21;
    }

    public void IncrementBet(int amount) {
        bet += amount;
    }

    public void DecrementBet(int amount) {
        bet -= amount;
    }

    public void Reset() {
        bet = 0;
    }

}