using UnityEngine;

public class CameraMovement : MonoBehaviour {
    [Header("Movement Settings")]
    public float moveSpeed = 5f;        // Speed at which the player moves

    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 2f; // How sensitive the mouse is

    [Header("References")]
    public Transform cameraTransform;   // Assign the child Camera transform here

    private float xRotation = 0f;       // Tracks vertical rotation (pitch)

    void Start() {
        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update() {
        // -------- 1. Mouse Look --------
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate the player (capsule) around its Y-axis (horizontal look)
        transform.Rotate(Vector3.up * mouseX);

        // Rotate the camera around X-axis (vertical look)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Prevent flipping
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // -------- 2. WASD Movement --------
        float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right arrows
        float moveZ = Input.GetAxis("Vertical");   // W/S or Up/Down arrows

        // Move in the direction the player is facing
        Vector3 moveDir = transform.right * moveX + transform.forward * moveZ;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }
}
