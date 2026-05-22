using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SoundRadiusMesh : MonoBehaviour
{
    [Header("Sound Ring Settings")]
    public float hearingRadius = 5f;
    public int segmentCount = 40;
    public float ringWidth = 0.15f;

    private Mesh mesh;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void LateUpdate()
    {
        DrawSoundRing();
    }

    private void DrawSoundRing()
    {
        Vector3[] vertices = new Vector3[segmentCount * 2];
        int[] triangles = new int[segmentCount * 6];

        float angleStep = 360f / segmentCount;

        for (int i = 0; i < segmentCount; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;

            float outerX = Mathf.Sin(angle) * hearingRadius;
            float outerZ = Mathf.Cos(angle) * hearingRadius;
            float innerX = Mathf.Sin(angle) * (hearingRadius - ringWidth);
            float innerZ = Mathf.Cos(angle) * (hearingRadius - ringWidth);

            vertices[i * 2] = new Vector3(outerX, 0f, outerZ);
            vertices[i * 2 + 1] = new Vector3(innerX, 0f, innerZ);
        }

        for (int i = 0; i < segmentCount; i++)
        {
            int next = (i + 1) % segmentCount;

            int ti = i * 6;
            triangles[ti] = i * 2;
            triangles[ti + 1] = next * 2;
            triangles[ti + 2] = i * 2 + 1;

            triangles[ti + 3] = next * 2;
            triangles[ti + 4] = next * 2 + 1;
            triangles[ti + 5] = i * 2 + 1;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}