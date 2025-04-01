using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class FoxTailWaver : MonoBehaviour
{
    [Range(0, 5)] public float waveSpeed = 1f;
    [Range(0, 0.5f)] public float waveAmount = 0.1f;

    private Vector3[] _baseVertices;
    private Mesh _mesh;

    void Start()
    {
        _mesh = GetComponent<MeshFilter>().mesh;
        _baseVertices = _mesh.vertices;
    }

    void Update()
    {
        Vector3[] vertices = _baseVertices.Clone() as Vector3[];

        for (int i = 0; i < vertices.Length; i++)
        {
            // Apply wave based on vertex Y position (tail length)
            float wave = Mathf.Sin(Time.time * waveSpeed + vertices[i].y * 5) * waveAmount;
            vertices[i].x += wave;
        }

        _mesh.vertices = vertices;
        _mesh.RecalculateNormals();
    }
}