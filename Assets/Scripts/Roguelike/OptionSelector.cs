using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionSelector : MonoBehaviour
{
    public Button option1Button;
    public Button option2Button;
    public Button option3Button;
    public Button option4Button;
    public Button option5Button;
    public Button confirmButton;

    public GameStateManager stateManager;
    private PlayerAttack playerAttack;

    // private List<Combo> newCombos;
    private int selectedOption = -1; // No option selected initially

    private void OnEnable()
    {
        confirmButton.gameObject.SetActive(false);
        playerAttack = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAttack>();

        // comboList = comboListManager.GetComponent<ComboList>();

        ResetButtonColors();

        option1Button.onClick.RemoveAllListeners();
        option2Button.onClick.RemoveAllListeners();
        option3Button.onClick.RemoveAllListeners();
        option4Button.onClick.RemoveAllListeners();
        option5Button.onClick.RemoveAllListeners();

        // ChooseImages();
        displayCurrentValues();

        option1Button.onClick.AddListener(() => SelectOption(0));
        option2Button.onClick.AddListener(() => SelectOption(1));
        option3Button.onClick.AddListener(() => SelectOption(2));
        option4Button.onClick.AddListener(() => SelectOption(3));
        option5Button.onClick.AddListener(() => SelectOption(4));

        // if (newCombos[0].IsDummy() && newCombos[1].IsDummy() && newCombos[2].IsDummy())
        // {
        //     confirmButton.gameObject.SetActive(true);
        // }
        // confirmButton.gameObject.SetActive(true);
    }

    private void displayCurrentValues(){
        // enum AttackType = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAttack>().AttackType;
        Dictionary<Elements.AttackType, float> currentValues = playerAttack.getUpgrades();
        
        Debug.Log(currentValues);

        TextMeshProUGUI button1Description = option1Button.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI button2Description = option2Button.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI button3Description = option3Button.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI button4Description = option4Button.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI button5Description = option5Button.GetComponentInChildren<TextMeshProUGUI>();

        button1Description.text = currentValues[Elements.AttackType.Normal]     .ToString();
        button2Description.text = currentValues[Elements.AttackType.Fire]       .ToString();
        button3Description.text = currentValues[Elements.AttackType.Water]      .ToString();
        button4Description.text = currentValues[Elements.AttackType.Lightning]  .ToString();
        button5Description.text = currentValues[Elements.AttackType.Leaf]       .ToString();
        

    }

    private void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            int temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    void SelectOption(int option)
    {
        // if (newCombos[option - 1].IsDummy())
        // {
        //     Debug.Log("Cannot select a dummy combo.");
        //     return; // Ignore the selection
        // }

        selectedOption = option;
        Debug.Log("Selected Option: " + selectedOption);

        confirmButton.gameObject.SetActive(true);
        // Update button visuals based on selection
        UpdateButtonColors();
    }

    void UpdateButtonColors()
    {
        // Reset all button colors
        ResetButtonColors();

        // Change color of the selected option (for example, to green)
        switch (selectedOption)
        {
            case 0:
                option1Button.GetComponent<RawImage>().color = Color.green;
                break;
            case 1:
                option2Button.GetComponent<RawImage>().color = Color.green;
                break;
            case 2:
                option3Button.GetComponent<RawImage>().color = Color.green;
                break;
            case 3:
                option4Button.GetComponent<RawImage>().color = Color.green;
                break;
            case 4:
                option5Button.GetComponent<RawImage>().color = Color.green;
                break;
        }
    }

    void ResetButtonColors()
    {
        // Set all button colors to default
        option1Button.GetComponent<RawImage>().color = Color.white;
        option2Button.GetComponent<RawImage>().color = Color.white;
        option3Button.GetComponent<RawImage>().color = Color.white;
        option4Button.GetComponent<RawImage>().color = Color.white;
        option5Button.GetComponent<RawImage>().color = Color.white;

    }

    public void AcceptOption()
    {
        // if (newCombos[selectedOption - 1].IsDummy())
        // {
        //     return;
        // }

        if(selectedOption == -1) {
            return;
        }

        Debug.Log("Selected Option: " + selectedOption);

        playerAttack.upgradeElement((Elements.AttackType) selectedOption, 0.1f);
        // comboList.AddP1SoloSkill(newCombos[selectedOption - 1]);
        // comboList.AddP2SoloSkill(newCombos[selectedOption - 1]);
        
    }
}

