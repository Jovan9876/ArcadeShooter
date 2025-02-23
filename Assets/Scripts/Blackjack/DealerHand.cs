using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DealerHand : MonoBehaviour, IHand {
    public List<Card> cards = new List<Card>();

    public void AddCard(Card card, bool faceDown) {
        cards.Add(card);

        card.transform.SetParent(transform, true);

        float offsetX = -60f * cards.Count;
        float offsetY = -100f * cards.Count;

        card.transform.localPosition = new Vector3(offsetX, offsetY, 0);

        if (faceDown) {
            card.transform.localRotation = Quaternion.Euler(0, 0, 0);
        } else {
            card.transform.localRotation = Quaternion.Euler(180, 0, 0);
        }
    }

    public void FlipOver() {
        cards[1].transform.localRotation = Quaternion.Euler(180, 0, 0);
    }

    public void ClearHand() {
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }

        cards.Clear();
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

    public bool HasSoft17() {
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
            if (score + 11 == 17) {
                return true; // SOFT
            }
            score += 1;
        }

        return false;
    }

    public bool HasBlackjack() {
        return cards.Count == 2 && GetScore() == 21;
    }

    public bool ShowingFaceOrAce() {
        return cards.Count == 2 && (cards[0].rank == CardEnums.Rank.Ace || cards[0].GetValue(0) == 10);
    }
}