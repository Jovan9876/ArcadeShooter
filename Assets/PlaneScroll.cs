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

        // Teleports to beginning when end of map reached
        if (currentPosition.z <= minZ)
        {
            currentPosition.z = maxZ;
        }

        // Clamp x positions
        currentPosition.x = Mathf.Clamp(currentPosition.x, minX, maxX);

        transform.position = currentPosition;
    }
}
