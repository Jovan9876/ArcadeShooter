using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    //Enemy damage
    public int damageAmount = 0;

    public int maxMobHealth = 100;
    public int currentHealth;

    public int mobExp = 25;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     currentHealth = maxMobHealth;   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage){
        currentHealth -= damage;

        if(currentHealth <= 0) {
            deathSequence();
        }
    }

    private void deathSequence(){
        
    }

}
