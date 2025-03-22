using UnityEngine;

public class BlackjackManager : MonoBehaviour {
    [SerializeField] protected Deck deck;
    [SerializeField] protected GameObject discardPile;
    [SerializeField] protected Player player;
    [SerializeField] protected PlayerHand playerHand;
    [SerializeField] protected DealerHand dealerHand;
    private Dealer dealer;


    protected bool gameStarted = false;

    private void Awake() {
        dealer = GetComponent<Dealer>();
    }

    protected void NewDeck() {
        deck.GenerateDeck();
        deck.Shuffle();
    }

    protected void StartBettingPhase() {
        gameStarted = false;
        player.ResetBet();
        Debug.Log("Place your bets before dealing.");
    }

    protected float DetermineWinner(PlayerHand hand) {
        int playerScore = hand.GetScore();
        int dealerScore = dealerHand.GetScore();

        Debug.Log($"Checking hand. Player Score: {playerScore}, Dealer Score: {dealerScore}");

        if (hand.HasBlackjack() && !dealerHand.HasBlackjack()) {
            // Player wins with Blackjack 2.5x payout
            Debug.Log("Player wins with Blackjack!");
            return 2.5f;
        } else if (playerScore > 21) {
            // Player busts Player loses bet
            Debug.Log("Player busts, dealer wins.");
            return 0f;
        } else if (dealerScore > 21) {
            // Dealer busts Player wins 2x payout
            Debug.Log("Dealer busts, player wins!");
            return 2.0f;
        } else if (playerScore > dealerScore) {
            // Player wins 2x payout
            Debug.Log("Player wins!");
            return 2.0f;
        } else if (dealerScore > playerScore) {
            Debug.Log("Dealer wins.");
            return 0f;
        } else {
            // Push (Tie) Bet is returned
            Debug.Log("It's a push! Player gets their bet back.");
            return 1f;
        }

    }

}
