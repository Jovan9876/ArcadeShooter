using UnityEngine;

// Manages the flow of the Blackjack game: deck generation, betting phase, and determining winners.
public class BlackjackManager : MonoBehaviour {

    [Header("Game References")]
    [SerializeField] protected Deck deck;                 // Reference to the deck used for gameplay
    [SerializeField] protected GameObject discardPile;    // Reference to the discard pile object
    [SerializeField] protected Player player;             // Reference to the player
    [SerializeField] protected PlayerHand playerHand;     // Reference to the player's hand
    [SerializeField] protected DealerHand dealerHand;     // Reference to the dealer's hand
    
    private Dealer dealer;                                // Internal reference to the dealer logic
    protected bool gameStarted = false;                   // Whether a game round is in progress

    private void Awake() {
        dealer = GetComponent<Dealer>();
    }

    protected void NewDeck() {
        // Creates a new deck and shuffles it
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
