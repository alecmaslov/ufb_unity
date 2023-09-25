using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

namespace UFB.Map
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class MeshMap : MonoBehaviour
    {
        public Material boardMaterial; // Set your 2D board image texture to this material
        private Mesh _mesh;

        private void Awake()
        {
            _mesh = GetComponent<MeshFilter>().mesh;
        }

        public void SpawnMeshMap()
        {
            Addressables.LoadAssetAsync<Sprite>("maps/kraken").Completed += (obj) =>
            {
                SpawnMeshMap(obj.Result);
            };
        }

        public void Clear()
        {
            DestroyImmediate(_mesh);
        }

        public MeshMapTile[] SpawnMeshMap(Sprite boardSprite, int gridHeight = 26, int gridWidth = 26)
        {
            MeshMapTile[] tiles = new MeshMapTile[gridHeight * gridWidth];
            boardMaterial.mainTexture = boardSprite.texture;
            List<CombineInstance> combineInstances = new List<CombineInstance>();

            int index = 0;
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.parent = transform;
                    cube.transform.position = new Vector3(
                        x - (gridWidth / 2),
                        0,
                        y - (gridHeight / 2)
                    );
                    MeshFilter cubeMeshFilter = cube.GetComponent<MeshFilter>();

                    // Adjust the UVs of the mesh to map to the correct location in the board texture
                    Vector2[] uvs = cubeMeshFilter.mesh.uv;
                    float tileSize = 1f / Mathf.Max(gridWidth, gridWidth);
                    for (int i = 0; i < uvs.Length; i++)
                    {
                        // uvs[i].x = uvs[i].x * tileSize + x * tileSize;
                        uvs[i].x = (1 - uvs[i].x) * tileSize + x * tileSize; // Flip the x UV coordinate
                        uvs[i].y = (1 - uvs[i].y) * tileSize + y * tileSize;
                    }
                    cubeMeshFilter.mesh.uv = uvs;
                    CombineInstance instance = new CombineInstance
                    {
                        mesh = cubeMeshFilter.sharedMesh,
                        transform = cubeMeshFilter.transform.localToWorldMatrix
                    };
                    combineInstances.Add(instance);

                    // Destroy the cube GameObject, as we no longer need it
                    // DestroyImmediate(cube);

                    Destroy(cube.GetComponent<MeshRenderer>());
                    Destroy(cube.GetComponent<MeshFilter>());

                    tiles[index] = new MeshMapTile(x, y, 1f, cube, this);
                    index++;
                }
            }
            MeshFilter combinedMeshFilter = GetComponent<MeshFilter>();
            combinedMeshFilter.mesh.CombineMeshes(combineInstances.ToArray());

            MeshRenderer renderer = GetComponent<MeshRenderer>();
            renderer.material = boardMaterial;

            return tiles;
        }

        public void SetTileHeight(Coordinates coordinates, float newHeight, Vector3? posOffset)
        {
            SetTileHeight(coordinates.Y, coordinates.X, newHeight, posOffset);
        }

        public void SetTileHeight(int x, int y, float newHeight, Vector3? posOffset)
        {
            Mesh mesh = _mesh ?? GetComponent<MeshFilter>().mesh; // allows for use in editor
            Vector3[] vertices = mesh.vertices;
            int gridSize = 26; // Assuming a 26x26 grid
            int startIndex = (x + y * gridSize) * 24; // 24 vertices per Unity default cube
            for (int i = 0; i < 24; i++)
            {
                int vertexIndex = startIndex + i;
                Vector3 vertex = vertices[vertexIndex];

                vertex += posOffset ?? Vector3.zero;

                // valid indexes to change height for:
                // 2,3,4,5,8,9,10,11,17,18,21,22

                if (
                    i == 2
                    || i == 3
                    || i == 4
                    || i == 5
                    || i == 8
                    || i == 9
                    || i == 10
                    || i == 11
                    || i == 17
                    || i == 18
                    || i == 21
                    || i == 22
                )
                {
                    vertex.y = newHeight;
                }
                vertices[vertexIndex] = vertex;
            }

            mesh.vertices = vertices;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
        }
    }

    // maybe in some cases we'll want a MeshMapTile, but without the overhead of an actual
    // game tile, for example, when previewing the map in a menu or something
    public class MeshMapTile
    {
        // public int X { get; set; }
        // public int Y { get; set; }
        public Coordinates Coordinates { get; private set; }
        public float Height { get; set; }


        public GameObject GameObject { get; private set; }
        private MeshMap _meshMap;

        public MeshMapTile(int x, int y, float height, GameObject gameObject, MeshMap meshMap)
        {
            Coordinates = new Coordinates(x, y);
            Height = height;
            GameObject = gameObject;
            _meshMap = meshMap;
        }

        public void SetHeight(float newHeight)
        {
            _meshMap.SetTileHeight(Coordinates, newHeight, null);
            Height = newHeight;
        }

        /// hmm should this class have an interpolator in it or not?
    }
}
