using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public int levelUpThreshold = 100;

    public int level = 1;
    public int currentExp = 0;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        checkLevelUp();
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
        currentExp -= levelUpThreshold;

        //Level up logic
    }
}
