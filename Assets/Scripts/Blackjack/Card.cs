using UnityEngine;
using static CardEnums;

public class Card : MonoBehaviour {
    public Rank rank;
    public Suit suit;

    public void Initialize(Suit suit, Rank rank) {
        this.suit = suit;
        this.rank = rank;
    }

    public int GetValue(int currentScore) {
        if (rank == Rank.Ace) {
            return (currentScore + 11 > 21) ? 1 : 11;
        }
        return (int)rank;
    }
}