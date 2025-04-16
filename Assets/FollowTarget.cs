using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;    // Assign your fox character here
    public Vector3 offset = new Vector3(0, 10, 0); // Position camera above target
    public float smoothSpeed = 0.125f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        transform.rotation = Quaternion.Euler(90f, 180f, 0f); // Top-down view
    }
}
