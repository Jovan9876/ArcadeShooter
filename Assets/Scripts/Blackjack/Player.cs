using UnityEngine;

public class Player : MonoBehaviour {
    [SerializeField] private PlayerHand playerHand;
    [SerializeField] private Dealer dealer;

    public void Hit() {
        if (playerHand.GetScore() >= 21) {
            Stand();
        } else {
            dealer.PlayerHit();
        }
    }

    public void Stand() {
        dealer.PlayerStand();
    }
}
