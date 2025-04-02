using Unity.VisualScripting;
using UnityEngine;

public class PlaneScroll : MonoBehaviour
{
    public float scrollSpeed = -5f;
    void Update()
    {
        Vector3 currentPosition = transform.position;
        currentPosition.z -= scrollSpeed * Time.deltaTime;
        transform.position = currentPosition;
    }
}