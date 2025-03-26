using UnityEngine;

public class Chip : MonoBehaviour {
    public Player player;
    public bool IsPlaced = false;
    public bool IsBet = false;
    public int chipValue;
    public int chipIndex;

    public void BetOrRemoveChip() {
        if (IsPlaced && !IsBet) {
            player.RemoveBet(this);
        } else if (!IsPlaced) {
            player.PlaceBet(this);
        }
    }
}

