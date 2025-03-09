using UnityEngine;
using UnityEngine.UI;

public class BetChip : MonoBehaviour {
    public BlackjackManager blackjackManager;

    public void PlaceBet(int chipIndex) { 
        blackjackManager.PlaceBet(chipIndex);
    }

}
