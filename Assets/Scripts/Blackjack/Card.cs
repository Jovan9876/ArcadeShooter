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
        switch (rank) {
            case Rank.Jack:
                return 10;
            case Rank.Queen:
                return 10;
            case Rank.King:
                return 10;
            case Rank.Ace:
                return (currentScore + 11 > 21) ? 1 : 11;

            default:
                return (int)rank;
        }
    }
}