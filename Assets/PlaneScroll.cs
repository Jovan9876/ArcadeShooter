using Unity.VisualScripting;
using UnityEngine;

public class PlaneScroll : MonoBehaviour
{
    public float scrollSpeed = 2.5f;

    // Set bounds
    public float minX = 100f;
    public float maxX = 180f;
    public float minZ = -370f;
    public float maxZ = -40f;

    void Update()
    {
        Vector3 currentPosition = transform.position;

        // Scroll on z-axis
        currentPosition.z -= scrollSpeed * Time.deltaTime;

        // Clamp x and z positions
        currentPosition.x = Mathf.Clamp(currentPosition.x, minX, maxX);
        currentPosition.z = Mathf.Clamp(currentPosition.z, minZ, maxZ);

        transform.position = currentPosition;
    }
}
