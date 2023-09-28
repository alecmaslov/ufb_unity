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
        public int MeshVericesOffset
        {
            get { return _meshVerticesOffset; }
            set { _meshVerticesOffset = value * 24; }
        }

        private int _meshVerticesOffset = 0;
        private bool _recalculateQueued = false;
        private List<Vector3> _currentVertices = new List<Vector3>();

        private void Awake()
        {
            _mesh = GetComponent<MeshFilter>().mesh;
        }

        public void SpawnMeshMap()
        {
            Addressables.LoadAssetAsync<Sprite>("maps/kraken").Completed += (obj) =>
            {
                SpawnTiles(obj.Result);
            };
        }

        public void Clear()
        {
            DestroyImmediate(_mesh);
        }

        private void ClearExistingTiles()
        {
            List<GameObject> tilesToDestroy = new List<GameObject>();
            foreach (Transform t in transform)
            {
                if (t.CompareTag("Tile"))
                {
                    tilesToDestroy.Add(t.gameObject);
                }
            }
            foreach (GameObject tile in tilesToDestroy)
            {
                if (Application.isPlaying)
                    Destroy(tile);
                else
                    DestroyImmediate(tile);
            }
        }

        public MeshMapTile[] SpawnTiles(
            Sprite boardSprite,
            int gridWidth = 26,
            int gridHeight = 26,
            Vector3? tileScalar = null
        )
        {
            MeshMapTile[] tiles = new MeshMapTile[gridHeight * gridWidth];
            boardMaterial.mainTexture = boardSprite.texture;
            List<CombineInstance> combineInstances = new List<CombineInstance>();

            ClearExistingTiles();

            int index = 0;
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    var coordinates = new Coordinates(x, gridHeight - 1 - y);
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.tag = "Tile";
                    cube.transform.parent = transform;
                    cube.transform.localPosition = Vector3.zero;

                    cube.transform.position = new Vector3(
                        x - (gridWidth / 2),
                        0,
                        y - (gridHeight / 2)
                    );
                    cube.transform.localScale = tileScalar ?? Vector3.one;
                    cube.name = coordinates.GameId;

                    MeshFilter cubeMeshFilter = cube.GetComponent<MeshFilter>();

                    // Adjust the UVs of the mesh to map to the correct location in the board texture
                    Vector2[] uvs = cubeMeshFilter.mesh.uv;
                    float tileSize = 1f / Mathf.Max(gridWidth, gridWidth);
                    for (int i = 0; i < uvs.Length; i++)
                    {
                        // uvs[i].x = uvs[i].x * tileSize + x * tileSize;
                        // uvs[i].y = uvs[i].y * tileSize + y * tileSize;
                        uvs[i].x = (1 - uvs[i].x) * tileSize + (x * tileSize); // Flip the x UV coordinate
                        uvs[i].y = (1 - uvs[i].y) * tileSize + (y * tileSize);
                    }
                    cubeMeshFilter.mesh.uv = uvs;
                    CombineInstance instance = new CombineInstance
                    {
                        mesh = cubeMeshFilter.sharedMesh,
                        transform = cubeMeshFilter.transform.localToWorldMatrix
                    };
                    combineInstances.Add(instance);

                    if (Application.isPlaying)
                    {
                        Destroy(cube.GetComponent<MeshRenderer>());
                        Destroy(cube.GetComponent<MeshFilter>());
                    }
                    else
                    {
                        DestroyImmediate(cube.GetComponent<MeshRenderer>());
                        DestroyImmediate(cube.GetComponent<MeshFilter>());
                    }
                    int meshIndexOffset = (x * gridHeight + y) * 24;
                    float height = tileScalar != null ? tileScalar.Value.y : 1f;
                    tiles[index] = new MeshMapTile(
                        x,
                        gridHeight - 1 - y,
                        // gridHeight - 1 - y,
                        height,
                        cube,
                        this,
                        meshIndexOffset
                    );
                    index++;
                }
            }
            MeshFilter combinedMeshFilter = GetComponent<MeshFilter>();
            combinedMeshFilter.mesh.CombineMeshes(combineInstances.ToArray());

            MeshRenderer renderer = GetComponent<MeshRenderer>();
            renderer.material = boardMaterial;

            // place the vertices into the current verts list
            _mesh.GetVertices(_currentVertices);

            return tiles;
        }

        private void Update()
        {
            if (_recalculateQueued)
            {
                _mesh.SetVertices(_currentVertices);
                _mesh.RecalculateBounds();
                _mesh.RecalculateNormals();
                _recalculateQueued = false;
            }
        }

        private readonly int[] topVertices = new int[] { 2, 3, 4, 5, 8, 9, 10, 11, 17, 18, 21, 22 };

        public void SetTileHeight(int startIndex, float newHeight, Vector3? posOffset = null)
        {
            foreach (int vertIdx in topVertices)
            {
                Vector3 vertex = _currentVertices[startIndex + vertIdx];
                vertex.y = newHeight;
                if (posOffset.HasValue)
                    vertex += posOffset.Value;
                _currentVertices[startIndex + vertIdx] = vertex;
            }
            _recalculateQueued = true;
        }

        public void SetTileHeight(int x, int y, float newHeight, Vector3? posOffset = null)
        {
            if (_mesh == null)
            {
                _mesh = GetComponent<MeshFilter>().mesh;
            }

            int gridSize = 26;
            int startIndex = (x + y * gridSize) * 24;
            SetTileHeight(startIndex, newHeight, posOffset);
        }
    }

    // maybe in some cases we'll want a MeshMapTile, but without the overhead of an actual
    // game tile, for example, when previewing the map in a menu or something
    public class MeshMapTile
    {
        public Coordinates Coordinates { get; private set; }
        public float Height { get; set; }
        public int MeshIndex { get; set; }

        public GameObject GameObject { get; private set; }
        private MeshMap _meshMap;

        public MeshMapTile(
            int x,
            int y,
            float height,
            GameObject gameObject,
            MeshMap meshMap,
            int meshIndex
        )
        {
            Coordinates = new Coordinates(x, y);
            Height = height;
            GameObject = gameObject;
            MeshIndex = meshIndex;
            _meshMap = meshMap;
        }

        public void SetHeight(float newHeight)
        {
            // _meshMap.SetTileHeight(Coordinates, newHeight, null);
            _meshMap.SetTileHeight(MeshIndex, newHeight, null);
            Height = newHeight;
        }

        /// hmm should this class have an interpolator in it or not?
    }
}
// public void SetTileHeight(int x, int y, float newHeight, Vector3? posOffset)
// {
//     if (_mesh == null) {
//         _mesh = GetComponent<MeshFilter>().mesh;
//     }
//     // Mesh mesh = _mesh ?? GetComponent<MeshFilter>().mesh; // allows for use in editor
//     Vector3[] vertices = _mesh.vertices;
//     int gridSize = 26; // Assuming a 26x26 grid
//     int startIndex = (x + y * gridSize) * 24; // 24 vertices per Unity default cube

//     // invert the index
//     // startIndex = (gridSize * gridSize * 24) - startIndex;


//     // startIndex += _meshVerticesOffset;
//     // startIndex %= vertices.Length;

//     // int gridHeight = 26;
//     // int gridWidth = 26;
//     // int startIndex = (x + (gridHeight - 1 - y) * gridWidth) * 24;

//     // Debug.Log($"Tile ({x}, {y}) | Expected Start Index: {startIndex}");

//     for (int i = 0; i < 24; i++)
//     {
//         int vertexIndex = startIndex + i;
//         Vector3 vertex = vertices[vertexIndex];

//         vertex += posOffset ?? Vector3.zero;

//         // valid indexes to change height for:
//         // 2,3,4,5,8,9,10,11,17,18,21,22

//         if (
//             i == 2
//             || i == 3
//             || i == 4
//             || i == 5
//             || i == 8
//             || i == 9
//             || i == 10
//             || i == 11
//             || i == 17
//             || i == 18
//             || i == 21
//             || i == 22
//         )
//         {
//             vertex.y = newHeight;
//         }
//         vertices[vertexIndex] = vertex;
//     }

//     _mesh.vertices = vertices;
//     _mesh.RecalculateBounds();
//     _mesh.RecalculateNormals();
// }
//  public MeshMapTile[] SpawnTiles(
//             Sprite boardSprite,
//             int gridWidth = 26,
//             int gridHeight = 26,
//             Vector3? tileScalar = null
//         )
//         {
//             MeshMapTile[] tiles = new MeshMapTile[gridHeight * gridWidth];
//             boardMaterial.mainTexture = boardSprite.texture;
//             List<CombineInstance> combineInstances = new List<CombineInstance>();

//             ClearExistingTiles();

//             int index = 0;
//             for (int x = 0; x < gridWidth; x++)
//             {
//                 for (int y = 0; y < gridHeight; y++)
//                 {
//                     var coordinates = new Coordinates(x, gridHeight - 1 - y);
//                     Debug.Log($"Spawning tile {x},{y} | {coordinates.GameId}");
//                     GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

//                     cube.tag = "Tile";
//                     cube.transform.parent = transform;
//                     cube.transform.localPosition = Vector3.zero;
//                     cube.transform.localScale = Vector3.one;
//                     cube.name = coordinates.GameId;

//                     MeshFilter cubeMeshFilter = cube.GetComponent<MeshFilter>();

//                     // Adjust the vertices of the mesh to the correct position
//                     Vector3 positionOffset = new Vector3(
//                         x - (gridWidth / 2),
//                         0,
//                         y - (gridHeight / 2)
//                     );

//                     Vector3[] vertices = cubeMeshFilter.mesh.vertices;
//                     for (int i = 0; i < vertices.Length; i++)
//                     {
//                         vertices[i] += positionOffset;
//                         if (tileScalar.HasValue)
//                         {
//                             vertices[i] = Vector3.Scale(vertices[i], tileScalar.Value);
//                         }
//                     }
//                     cubeMeshFilter.mesh.vertices = vertices;

//                     // Adjust the UVs of the mesh to map to the correct location in the board texture
//                     Vector2[] uvs = cubeMeshFilter.mesh.uv;
//                     float tileSize = 1f / Mathf.Max(gridWidth, gridHeight);
//                     for (int i = 0; i < uvs.Length; i++)
//                     {
//                         uvs[i].x = (1 - uvs[i].x) * tileSize + x * tileSize;
//                         uvs[i].y = (1 - uvs[i].y) * tileSize + y * tileSize;
//                     }
//                     cubeMeshFilter.mesh.uv = uvs;

//                     CombineInstance instance = new CombineInstance
//                     {
//                         mesh = cubeMeshFilter.sharedMesh,
//                         transform = cubeMeshFilter.transform.localToWorldMatrix
//                     };
//                     combineInstances.Add(instance);

//                     cube.transform.position = new Vector3(
//                         x - (gridWidth / 2),
//                         0,
//                         y - (gridHeight / 2)
//                     );

//                     if (Application.isPlaying)
//                     {
//                         Destroy(cube.GetComponent<MeshRenderer>());
//                         Destroy(cube.GetComponent<MeshFilter>());
//                     }
//                     else
//                     {
//                         DestroyImmediate(cube.GetComponent<MeshRenderer>());
//                         DestroyImmediate(cube.GetComponent<MeshFilter>());
//                     }

//                     float height = tileScalar != null ? tileScalar.Value.y : 1f;
//                     tiles[index] = new MeshMapTile(x, gridHeight - 1 - y, height, cube, this);
//                     index++;
//                 }
//             }
//             MeshFilter combinedMeshFilter = GetComponent<MeshFilter>();
//             combinedMeshFilter.mesh.CombineMeshes(combineInstances.ToArray());

//             MeshRenderer renderer = GetComponent<MeshRenderer>();
//             renderer.material = boardMaterial;

//             return tiles;
//         }
