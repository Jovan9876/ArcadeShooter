using UnityEngine;

public class BlackjackManager : MonoBehaviour {
    [SerializeField] protected Deck deck;
    [SerializeField] protected PlayerHand playerHand;
    [SerializeField] protected DealerHand dealerHand;

    [SerializeField] private TMPro.TMP_Text dealerWins;
    [SerializeField] private TMPro.TMP_Text dealerBusts;
    [SerializeField] private TMPro.TMP_Text playerWins;
    [SerializeField] private TMPro.TMP_Text playerBusts;
    [SerializeField] private TMPro.TMP_Text playerBJ;
    [SerializeField] private TMPro.TMP_Text push;

    protected void NewDeck() {
        deck.GenerateDeck();
        deck.Shuffle();
    }

    protected void DetermineWinner() {
        int playerScore = playerHand.GetScore();
        int dealerScore = dealerHand.GetScore();

        Debug.Log($"Player Final Score: {playerScore}");
        Debug.Log($"Dealer Final Score: {dealerScore}");

        if (playerHand.cards.Count == 2 && playerScore == 21) {
            playerBJ.transform.gameObject.SetActive(true);
        } else if (playerScore > 21) {
            playerBusts.transform.gameObject.SetActive(true);
        } else if (dealerScore > 21) {
            dealerBusts.transform.gameObject.SetActive(true);
        } else if (playerScore > dealerScore) {
            playerWins.transform.gameObject.SetActive(true);
        } else if (dealerScore > playerScore) {
            dealerWins.transform.gameObject.SetActive(true);
        } else {
            push.transform.gameObject.SetActive(true);
        }
    }

    protected void HideUI() {
        playerBusts.transform.gameObject.SetActive(false);
        dealerBusts.transform.gameObject.SetActive(false);
        playerWins.transform.gameObject.SetActive(false);
        dealerWins.transform.gameObject.SetActive(false);
        push.transform.gameObject.SetActive(false);
        playerBJ.transform.gameObject.SetActive(false);
    }

}
