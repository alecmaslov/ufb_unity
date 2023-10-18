using UnityEngine;
using UnityEngine.UI;

namespace UFB.UI
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class WorldspaceMeshTriangle : MonoBehaviour
    {
        public float triangleWidth = 0.6f;

        private RectTransform _anchor;
        private Transform _target;
        private Mesh _mesh;

        private void OnEnable()
        {
            _mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = _mesh;
            transform.parent = null;
        }

        public void SetAnchor(RectTransform anchor)
        {
            _anchor = anchor;
        }

        public void SetTarget(Transform t)
        {
            _target = t;
        }

        public void SetMaterial(Material material)
        {
            GetComponent<MeshRenderer>().material = material;
        }

        private void Update()
        {
            if (_target == null || _anchor == null)
            {
                return;
            }

            Vector3[] vertices = new Vector3[3];
            vertices[0] = _target.position;

            float xRange = _anchor.rect.max.x - _anchor.rect.min.x;


            float xLeft = _anchor.rect.min.x + (xRange * triangleWidth);
            float xRight = _anchor.rect.max.x - (xRange * triangleWidth);

            Vector2 leftPoint = new(xLeft, _anchor.rect.min.y);
            Vector2 rightPoint = new(xRight, _anchor.rect.min.y);

            vertices[1] = _anchor.TransformPoint(leftPoint);
            vertices[2] = _anchor.TransformPoint(rightPoint);
            int[] triangles = new int[3] { 0, 1, 2 };

            _mesh.Clear();
            _mesh.vertices = vertices;
            _mesh.triangles = triangles;
        }

        public static WorldspaceMeshTriangle Create(Material material, float triangleWidth = 0.6f)
        {
            var triangle = new GameObject("Triangle");
            var meshTriangle = triangle.AddComponent<WorldspaceMeshTriangle>();
            meshTriangle.SetMaterial(material);
            meshTriangle.triangleWidth = triangleWidth;
            triangle.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            triangle.transform.localScale = Vector3.one;
            return meshTriangle;
        }
    }
}
