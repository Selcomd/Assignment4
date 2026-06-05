using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VisionConeMesh : MonoBehaviour
{
    [Header("Cone Settings")]
    public float viewDistance = 10f;
    public float viewAngle = 90f;
    public int rayCount = 30;
    public LayerMask obstacleMask;

    private Mesh mesh;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void LateUpdate()
    {
        DrawVisionCone();
    }

    private void DrawVisionCone()
    {
        Vector3[] vertices = new Vector3[rayCount + 2];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = Vector3.zero;

        float angleStep = viewAngle / rayCount;
        float startAngle = -viewAngle / 2f;

        for (int i = 0; i <= rayCount; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector3 direction = AngleToDirection(angle);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(direction), out hit, viewDistance, obstacleMask))
            {
                vertices[i + 1] = direction * hit.distance;
            }
            else
            {
                vertices[i + 1] = direction * viewDistance;
            }
        }

        for (int i = 0; i < rayCount; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    private Vector3 AngleToDirection(float angleDegrees)
    {
        float rad = angleDegrees * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad));
    }
}