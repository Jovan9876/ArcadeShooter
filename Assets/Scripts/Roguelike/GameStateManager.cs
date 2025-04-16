using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public int levelUpThreshold = 100;
    
    //Player script here
    // public PlayerManager playerManager;

    //Canvas for UI selection
    public GameObject canvas;

    public int level = 1;
    public int currentExp = 0;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvas = GameObject.FindGameObjectWithTag("Canvas");
    }

    // Update is called once per frame
    void Update()
    {
        checkLevelUp();

        // ADd exp on screen touch
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            addExp(50);
        }
    }

    public void addExp(int newExp){
        currentExp += newExp;
        checkLevelUp();
    }

    private void checkLevelUp(){
        if(currentExp >= levelUpThreshold){
            levelUp();
        }
    }

    private void levelUp(){
        Debug.Log("Player level up");

        //Reset currentExp and increase levelup threshold
        currentExp -= levelUpThreshold;
        level++;
        levelUpThreshold += 10;

        //Level up logic
        StartCoroutine(HandleLevelUp());

    }

    private IEnumerator HandleLevelUp(){
        UnityEngine.Debug.Log("Level: " + level + "\nCurrXP: " + currentExp + "\nNextLevel: " + levelUpThreshold);

        // Pauses the game
        Time.timeScale = 0;

        // Activate level up window
        canvas.transform.Find("LevelUpWindow").gameObject.SetActive(true);
        while (canvas.transform.Find("LevelUpWindow").gameObject.activeSelf){
            yield return null;
        }
        
        //Resumes the game
        Time.timeScale = 1;
    }
}
