using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MapPlane : MonoBehaviour
{
    public int gridSize = 26;
    public int subdivisions = 26;
    public float size = 10.0f;
    public Sprite boardSprite;
    public Material boardMaterial; // Set your 2D board image texture to this material

    private void Start()
    {
        Generate();
    }

    public void Generate()
    {
        GetComponent<MeshFilter>().mesh = GeneratePlane(subdivisions, size);
        // save the mesh as an asset
#if UNITY_EDITOR
        AssetDatabase.CreateAsset(
            GetComponent<MeshFilter>().mesh,
            "Assets/Meshes/GeneratedMesh.asset"
        );
#endif
    }

    Mesh GeneratePlane(int subdivisions, float size)
    {
        Mesh m = new Mesh();

        int hCount2 = subdivisions + 1;
        int vCount2 = subdivisions + 1;
        int numT = subdivisions * subdivisions * 6;
        int[] triangles = new int[numT];
        Vector3[] vertices = new Vector3[hCount2 * vCount2];
        Vector2[] uv = new Vector2[hCount2 * vCount2];

        int index = 0;
        float uvFactor = 1.0f / subdivisions;
        float sizeFactor = size / subdivisions;
        for (float y = 0.0f; y < vCount2; y++)
        {
            for (float x = 0.0f; x < hCount2; x++)
            {
                vertices[index] = new Vector3(
                    x * sizeFactor - size / 2f,
                    0,
                    y * sizeFactor - size / 2f
                );
                uv[index++] = new Vector2(x * uvFactor, y * uvFactor);
            }
        }

        index = 0;
        for (int y = 0; y < subdivisions; y++)
        {
            for (int x = 0; x < subdivisions; x++)
            {
                triangles[index] = (y * hCount2) + x;
                triangles[index + 1] = ((y + 1) * hCount2) + x;
                triangles[index + 2] = (y * hCount2) + x + 1;

                triangles[index + 3] = ((y + 1) * hCount2) + x;
                triangles[index + 4] = ((y + 1) * hCount2) + x + 1;
                triangles[index + 5] = (y * hCount2) + x + 1;
                index += 6;
            }
        }

        m.vertices = vertices;
        m.uv = uv;
        m.triangles = triangles;
        m.RecalculateNormals();

        return m;
    }
}
