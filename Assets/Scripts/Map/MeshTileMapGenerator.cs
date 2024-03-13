using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UFB.StateSchema;
using UnityEngine;

namespace UFB.Map
{
    public interface ITileMapGenerator
    {
        Task<BaseTile[]> GenerateTiles(MapState state);
        void Clear();
    }

    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class MeshTileMapGenerator : MonoBehaviour, ITileMapGenerator
    {
        private Mesh _mesh;
        private bool _recalculateQueued = false;
        private List<Vector3> _currentVertices = new List<Vector3>();

        public async Task<BaseTile[]> GenerateTiles(MapState state)
        {
            Material material = await AddressablesExtensions
                .LoadAssetAsyncDownloadProgress<Material>(
                    state.resourceAddress + "/material",
                    $"Map {state.name}"
                )
                .Task;

            if (material == null)
            {
                throw new Exception($"Could not load material for map {state.name}");
            }
            var tileRenderers = SpawnTiles(material, (int)state.gridWidth, (int)state.gridHeight, new Vector3(1, 0.1f, 1));
            BaseTile[] tiles = new BaseTile[tileRenderers.Length];
            for (int i = 0; i < tileRenderers.Length; i++)
            {
                // here we could switch depending on the tile type
                // eventually abstract this out into a TileFactory

                tiles[i] = tileRenderers[i].GameObject.GetOrAddComponent<BaseTile>();
                // we need to find the tileState with the coordinates
                var tileState = state.TileStateAtCoordinates(tileRenderers[i].Coordinates);
                Debug.Log($"Tile state: {tileState}");
                tiles[i].SetParameters(tileState.ToTileParameters());
                tiles[i].SetRenderer(tileRenderers[i]);

                Debug.Log($"WALLS: {tileState.walls}");
                // Spawn walls
                if (tileState.walls != null)
                {
                    var walls = tileRenderers[i].GameObject.GetOrAddComponent<TileWalls>();

                    Debug.Log($"Spawning walls for tile {tileState.walls.ToBoolArray()}");
                    walls.SpawnWalls(tileState.walls.ToBoolArray());
                }
            }
            return tiles;
        }

        public void Clear()
        {
# if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(_mesh);
            else
                Destroy(_mesh);
# else
            Destroy(_mesh);
# endif
        }

        private ITileRenderer[] SpawnTiles(
            Material boardMaterial,
            int gridWidth = 26,
            int gridHeight = 26,
            Vector3? tileScalar = null
        )
        {
            _mesh = GetComponent<MeshFilter>().mesh;
            MeshMapTileRenderer[] tiles = new MeshMapTileRenderer[gridHeight * gridWidth];
            List<CombineInstance> combineInstances = new List<CombineInstance>();

            // ClearExistingTiles();

            int index = 0;
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    // what might never change is the spawning of the grid
                    // so maybe we pass in an array of GameObjects
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
                    tiles[index] = new MeshMapTileRenderer(
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
            if (_recalculateQueued && _mesh != null)
            {
                _mesh.SetVertices(_currentVertices);
                _mesh.RecalculateBounds();
                _mesh.RecalculateNormals();
                _recalculateQueued = false;
            }
        }

        public void SetTileHeight(int startIndex, float newHeight, Vector3? posOffset = null)
        {
            foreach (int vertIdx in _topVertices)
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

        private readonly int[] _topVertices = new int[]
        {
            2,
            3,
            4,
            5,
            8,
            9,
            10,
            11,
            17,
            18,
            21,
            22
        };
    }
}


// private void ClearExistingTiles()
// {
//     List<GameObject> tilesToDestroy = new List<GameObject>();
//     foreach (Transform t in transform)
//     {
//         if (t.CompareTag("Tile"))
//         {
//             tilesToDestroy.Add(t.gameObject);
//         }
//     }
//     foreach (GameObject tile in tilesToDestroy)
//     {
//         if (Application.isPlaying)
//             Destroy(tile);
//         else
//             DestroyImmediate(tile);
//     }
// }
