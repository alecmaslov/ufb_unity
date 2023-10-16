using UnityEngine;
using UnityEngine.UI;

namespace UFB.UI
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class WorldspaceMeshTriangle : MonoBehaviour
    {
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
            vertices[1] = _anchor.TransformPoint(_anchor.rect.min);
            vertices[2] = _anchor.TransformPoint(
                new Vector2(_anchor.rect.max.x, _anchor.rect.min.y)
            );
            int[] triangles = new int[3] { 0, 1, 2 };

            _mesh.Clear();
            _mesh.vertices = vertices;
            _mesh.triangles = triangles;
        }

        public static WorldspaceMeshTriangle Create(Material material)
        {
            var triangle = new GameObject("Triangle");
            var meshTriangle = triangle.AddComponent<WorldspaceMeshTriangle>();
            meshTriangle.SetMaterial(material);
            triangle.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            triangle.transform.localScale = Vector3.one;
            return meshTriangle;
        }
    }
}
