using UnityEngine;

public class BlackjackManager : MonoBehaviour {
    [SerializeField] protected Deck deck;
    [SerializeField] protected GameObject discardPile;
    [SerializeField] protected Player player;
    [SerializeField] protected PlayerHand playerHand;
    [SerializeField] protected DealerHand dealerHand;

    protected bool gameStarted = false;
    private int[] chipValues = { 5, 25, 100, 500, 1000 };

    protected void NewDeck() {
        deck.GenerateDeck();
        deck.Shuffle();
    }

    protected void StartBettingPhase() {
        gameStarted = false;
        player.ResetBet();
        Debug.Log("Place your bets before dealing.");
    }

    public void PlaceBet(int chipIndex) {
        if (gameStarted) return;
        int chipValue = chipValues[chipIndex];
        Debug.Log($"Placing bet {chipValue}");
        player.PlaceBet(chipValue);
    }

    protected void DetermineWinner() {
        int playerScore = playerHand.GetScore();
        int dealerScore = dealerHand.GetScore();

        Debug.Log($"Player Final Score: {playerScore}");
        Debug.Log($"Dealer Final Score: {dealerScore}");

        //if (playerHand.cards.Count == 2 && playerScore == 21) {
        //    playerBJ.transform.gameObject.SetActive(true);
        //} else if (playerScore > 21) {
        //    playerBusts.transform.gameObject.SetActive(true);
        //} else if (dealerScore > 21) {
        //    dealerBusts.transform.gameObject.SetActive(true);
        //} else if (playerScore > dealerScore) {
        //    playerWins.transform.gameObject.SetActive(true);
        //} else if (dealerScore > playerScore) {
        //    dealerWins.transform.gameObject.SetActive(true);
        //} else {
        //    push.transform.gameObject.SetActive(true);
        //}
    }

    //protected void HideUI() {
    //    playerBusts.transform.gameObject.SetActive(false);
    //    dealerBusts.transform.gameObject.SetActive(false);
    //    playerWins.transform.gameObject.SetActive(false);
    //    dealerWins.transform.gameObject.SetActive(false);
    //    push.transform.gameObject.SetActive(false);
    //    playerBJ.transform.gameObject.SetActive(false);
    //}

}
