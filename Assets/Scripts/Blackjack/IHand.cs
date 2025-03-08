public interface IHand {
    void AddCard(Card card, bool faceDown);
    int GetScore();
    bool HasBlackjack();
}