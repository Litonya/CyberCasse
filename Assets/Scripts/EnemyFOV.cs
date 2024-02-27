using UnityEngine;

public class EnemyFOV : MonoBehaviour
{
    Mesh mesh;

    Vector3 origin = Vector3.zero;

    float startAngle;

    [SerializeField] MeshFilter meshFilter;
    [SerializeField] int rayCount = 2;
    [SerializeField] float fieldOfView = 90f;
    [SerializeField] float distance = 50f;
    [SerializeField] Vector3 offset;
    [SerializeField] LayerMask targetMask;
    [SerializeField] LayerMask layerMask;

    public bool IsTarget { get; private set; }

    private void Start()
    {
        mesh = new Mesh();
        meshFilter.mesh = mesh;
    }

    private void LateUpdate()
    {
        IsTarget = false;

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        var angle = startAngle;
        var angleIncrease = fieldOfView / rayCount;

        var vertices = new Vector3[rayCount + 1 + 1];
        var uv = new Vector2[vertices.Length];
        var triangles = new int[rayCount * 3];

        var vertexIndex = 1;
        var trianglesIndex = 0;

        vertices[0] = origin;

        for (int i = 0; i <= rayCount; i++)
        {
            var hit = PhysicsX.Raycast(origin, MathfX.AngleToVector3D(angle), layerMask, distance, debug: true);
            var vertex = hit.collider ? hit.point : origin + MathfX.AngleToVector3D(angle + transform.eulerAngles.y) * distance;

            if (hit.collider != null && CompareLayer(hit.collider.gameObject.layer, targetMask))
                IsTarget = true;

            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[trianglesIndex + 0] = 0;
                triangles[trianglesIndex + 1] = vertexIndex - 1;
                triangles[trianglesIndex + 2] = vertexIndex;

                trianglesIndex += 3;
            }

            vertexIndex++;

            angle -= angleIncrease;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }

    public void SetOrigin(Vector3 origin)
    {
        this.origin = origin + offset;
    }

    public void SetDirection(Vector3 direction)
    {
        startAngle = MathfX.VectorToAngle3D(direction) + fieldOfView / 2f;
    }

    bool CompareLayer(LayerMask layer, LayerMask layerMask)
    {
        return layerMask == (layerMask | (1 << layer));
    }

    public static class MathfX
    {
        public static Vector3 AngleToVector2D(float angleDeg)
        {
            var angleRad = angleDeg * Mathf.Deg2Rad;
            return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0f);
        }

        public static Vector3 AngleToVector3D(float angleDeg)
        {
            var angleRad = angleDeg * Mathf.Deg2Rad;
            return new Vector3(Mathf.Cos(angleRad), 0f, Mathf.Sin(angleRad));
        }

        public static float VectorToAngle2D(Vector3 vector)
        {
            vector.Normalize();

            var angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;

            if (angle < 0)
                angle += 360;

            return angle;
        }

        public static float VectorToAngle3D(Vector3 vector)
        {
            vector.Normalize();

            var angle = Mathf.Atan2(vector.z, vector.x) * Mathf.Rad2Deg;

            // if (angle < 0)
            //     angle += 360;

            return angle;
        }
    }

    public static class PhysicsX
    {
        public static (RaycastHit hit, Ray ray) MouseHit(LayerMask layerMask, float maxDistance = 1000f, bool debug = false)
        {
            return MouseHit(Camera.main, layerMask, maxDistance, debug);
        }

        public static (RaycastHit hit, Ray ray) MouseHit(Camera cam, LayerMask layerMask, float maxDistance = 1000f, bool debug = false)
        {
            var mousePos = Input.mousePosition;
            var ray = cam.ScreenPointToRay(mousePos);
            var hit = new RaycastHit();

            Physics.Raycast(ray.origin, ray.direction, out hit, maxDistance, layerMask);

            if (debug)
            {
                Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.red);
            }

            return (hit, ray);
        }

        public static RaycastHit Raycast(Vector3 origin, Vector3 direction, LayerMask layerMask, float maxDistance = 10f, bool debug = false)
        {
            var ray = new Ray(origin, direction);
            var hit = new RaycastHit();

            Physics.Raycast(ray.origin, ray.direction, out hit, maxDistance, layerMask);

            if (debug)
            {
                Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.red);
            }

            return hit;
        }
    }
}