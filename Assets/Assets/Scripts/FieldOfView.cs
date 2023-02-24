using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField]
    [Tooltip("colliding layer")]
    private LayerMask layerMask;

    private Mesh mesh;
    Vector3 origin;
    private float fov = 60f;
    private float startingAngle;
    private bool detected;
    // Start is called before the first frame update
    void Start()
    {
        origin = Vector3.zero;
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }
    void Update()
    { 
        int rayCount = 50;
        float angle = startingAngle;
        float angleIncrease = fov / rayCount;
        float viewDistance = 7f;
        Vector3[] vertices = new Vector3[rayCount + 2];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];
        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex;
            RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, GetVectorFromAngle(angle), viewDistance, layerMask);
            if (raycastHit2D.collider == null)
            {
                vertex = origin + GetVectorFromAngle(angle) * viewDistance;
            } else
            {
                vertex = raycastHit2D.point;
                if (raycastHit2D.collider.gameObject.CompareTag("Player"))
                {
                    detected = true;
                }

            }
            vertices[vertexIndex] = vertex;
            if (i > 0)
            {
                triangles[triangleIndex] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;
                triangleIndex += 3;
            }
            vertexIndex++;
            angle -= angleIncrease;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }

    private Vector3 GetVectorFromAngle(float angle)
    {
        float anglerad = angle * (Mathf.PI / 180);
        return new Vector3(Mathf.Cos(anglerad), Mathf.Sin(anglerad));
    }

    private float GetAngleFromVector(Vector3 dir) 
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0)
        {
            n += 360;
        }
        return n;
    }

    public void SetOrigin(Vector3 origin)
    {
        this.origin = origin;
    }
    public void SetAimDirection(Vector3 aimDirection)
    {
        startingAngle = GetAngleFromVector(aimDirection) + fov / 2f;
    }
    public bool fovDetected()
    {

        return detected;
    }
}
