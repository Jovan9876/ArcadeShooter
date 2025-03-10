using UnityEngine;

public class Chip : MonoBehaviour {
    public Player player;
    public bool IsPlaced = false;
    public int chipValue;
    public int chipIndex;

    public void BetOrRemoveChip() {
        if (IsPlaced) {
            player.RemoveBet(this);
        } else {
            player.PlaceBet(this);
        }
    }
}

