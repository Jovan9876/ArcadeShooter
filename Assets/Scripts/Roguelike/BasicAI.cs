using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAI : MonoBehaviour
{
    private GameObject playerObj = null;
    public float speed = 2.0f;
    public float normalSpeed = 2.0f;

    // Returns the distance between the object the script is attached to, and the targetObject
    // Takes in a single GameObject and returns a float representing the distance
    private float findDistance(GameObject targetObject){
        return Vector3.Distance(transform.position, targetObject.transform.position);
    }

    // Start is called before the first frame update
    void Start()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        
        // Trying without transform.LookAt

        Vector3 playerPosition = new Vector3(playerObj.transform.position.x, transform.position.y, playerObj.transform.position.z);

        transform.position = Vector3.MoveTowards(transform.position, playerPosition, speed * Time.deltaTime);
    }
}
