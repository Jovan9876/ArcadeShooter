using UnityEngine;

// Represents an individual betting chip that can be placed or removed during the betting phase.
public class Chip : MonoBehaviour {
    [Header("Chip Settings")]
    public int chipValue;       // The value of this chip
    public int chipIndex;       // Unique index to help identify the chip in list of prefabs

    [Header("Chip State")]
    public bool IsPlaced = false;   // True if the chip is currently on the betting area
    public bool IsBet = false;      // True if the chip has been locked in as part of the final bet

    [Header("References")]
    public Player player;       // Reference to the Player placing or removing the chip

    public void BetOrRemoveChip() {
        // Toggles the chip between being placed or removed depending on its state
        if (IsPlaced && !IsBet) {
            player.RemoveBet(this);
        } else if (!IsPlaced) {
            player.PlaceBet(this);
        }
    }
}

