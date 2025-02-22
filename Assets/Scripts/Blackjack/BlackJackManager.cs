using UnityEngine;

public class BlackJackManager : MonoBehaviour {
    [SerializeField] protected Deck deck;
    [SerializeField] protected Hand playerHand;
    [SerializeField] protected Hand dealerHand;

    protected void Start() {
        deck.GenerateDeck();
        deck.Shuffle();
        StartGame();
    }

    public void StartGame() {
        dealerHand.ClearHand();
        playerHand.ClearHand();

        deck.BurnCard();

        playerHand.AddCard(deck.DrawCard(), false);
        dealerHand.AddCard(deck.DrawCard(), false);
        playerHand.AddCard(deck.DrawCard(), false);
        dealerHand.AddCard(deck.DrawCard(), true);

        foreach (Card card in playerHand.cards) {
            Debug.Log($"Player Hand: {card}");
        }
        foreach (Card card in dealerHand.cards) {
            Debug.Log($"Player Hand: {card}");
        }

        CheckForBlackjack();
    }


    public void CheckForBlackjack() {

    }

    public void OnDealerTurnEnd() {
        DetermineWinner();
    }

    protected void DetermineWinner() {

    }

    protected void EndGame(string result) {
        Debug.Log(result);
    }
}
