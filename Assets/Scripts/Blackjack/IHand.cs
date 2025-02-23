public interface IHand {
    void AddCard(Card card, bool faceDown);
    void ClearHand();
    int GetScore();
    bool HasBlackjack();
}