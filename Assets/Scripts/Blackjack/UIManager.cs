using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    [Header("Betting UI")]
    [SerializeField] private GameObject bettingPanel; // chips and deal button

    [Header("Gameplay UI")]
    [SerializeField] private GameObject gameplayPanel; // Hit, Stand, Double Down, Split buttons


    private void Start() {
        ShowBettingUI();
    }

    public void ShowBettingUI() {
        bettingPanel.SetActive(true);
        gameplayPanel.SetActive(false);
    }

    public void ShowGameplayUI() {
        bettingPanel.SetActive(false);
        gameplayPanel.SetActive(true);
    }
}
