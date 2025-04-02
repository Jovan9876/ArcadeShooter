using UnityEngine;

public class PlaneScroll : MonoBehaviour
{
    public GameObject[] planes; // Two or more plane objects side-by-side
    public float scrollSpeed = 2f;
    public float planeLength = 50f; // Distance between resets (match plane size in Z)

    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;

        foreach (GameObject plane in planes)
        {
            plane.GetComponent<PrefabSpawner>().SpawnAll();
        }
    }

    void Update()
    {
        foreach (GameObject plane in planes)
        {
            // Scroll plane forward along Z-axis
            plane.transform.Translate(Vector3.forward * scrollSpeed * Time.deltaTime);

            // Recycle the plane if it's too far ahead of the camera
            if (plane.transform.position.z - cam.position.z > planeLength)
            {
                plane.transform.position -= new Vector3(0, 0, planeLength * planes.Length);

                // Refresh contents
                plane.GetComponent<PrefabSpawner>().SpawnAll();
            }
        }

        // Move the camera with the scrolling
        cam.Translate(Vector3.forward * scrollSpeed * Time.deltaTime);
    }
}

