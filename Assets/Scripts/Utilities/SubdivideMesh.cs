using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UFB.Utilities
{
    using UnityEngine;

    [RequireComponent(typeof(MeshFilter))]
    public class SubdivideMesh : MonoBehaviour
    {
        public void Subdivide()
        {
            var clone = Instantiate(gameObject);

            Mesh mesh = clone.GetComponent<MeshFilter>().mesh;

            Vector3[] oldVertices = mesh.vertices;
            int[] oldTriangles = mesh.triangles;

            Vector3[] newVertices = new Vector3[oldTriangles.Length];
            int[] newTriangles = new int[oldTriangles.Length * 4];

            int newIndex = 0;
            for (int i = 0; i < oldTriangles.Length; i += 3)
            {
                Vector3 v0 = oldVertices[oldTriangles[i]];
                Vector3 v1 = oldVertices[oldTriangles[i + 1]];
                Vector3 v2 = oldVertices[oldTriangles[i + 2]];

                Vector3 m0 = (v0 + v1) * 0.5f;
                Vector3 m1 = (v1 + v2) * 0.5f;
                Vector3 m2 = (v2 + v0) * 0.5f;

                newVertices[i] = v0;
                newVertices[i + 1] = v1;
                newVertices[i + 2] = v2;

                int currentVertex = oldVertices.Length + i;
                newTriangles[newIndex++] = oldTriangles[i];
                newTriangles[newIndex++] = currentVertex;
                newTriangles[newIndex++] = currentVertex + 2;

                newTriangles[newIndex++] = oldTriangles[i + 1];
                newTriangles[newIndex++] = currentVertex + 1;
                newTriangles[newIndex++] = currentVertex;

                newTriangles[newIndex++] = oldTriangles[i + 2];
                newTriangles[newIndex++] = currentVertex + 2;
                newTriangles[newIndex++] = currentVertex + 1;

                newTriangles[newIndex++] = currentVertex;
                newTriangles[newIndex++] = currentVertex + 1;
                newTriangles[newIndex++] = currentVertex + 2;
            }

            mesh.vertices = newVertices;
            mesh.triangles = newTriangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }
    }
}
