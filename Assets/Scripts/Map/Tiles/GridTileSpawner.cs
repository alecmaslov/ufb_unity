using UnityEngine;

namespace UFB.Map
{

    public abstract class TileSpawner
    {
        public abstract ITile[] SpawnTiles();

        // it would be nice if we had a generic, and we could spawn tiles based on the generic
        public virtual T[] SpawnTileGrid<T>(int gridHeight, int gridWidth, Vector3 tileScale, Transform parent)
            where T : ITile
        {
            var tiles = new T[gridHeight * gridWidth];

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    var coordinates = new Coordinates(x, y);
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.parent = parent;
                    cube.transform.localPosition = Vector3.zero;
                    cube.transform.position = new Vector3(
                        x - (gridWidth / 2),
                        0,
                        y - (gridHeight / 2)
                    );
                    cube.transform.localScale = tileScale;
                    cube.name = coordinates.GameId;

                    var tile = cube.AddComponent(typeof(T));
                    // tiles[x * gridHeight + y] = tile;
                }
            }
            return tiles;
        }
    }

    // actually we can think of this as a voxel spawner / tile spawner
    // maybe this can be generic, and the MeshMapRenderer can use a specific implementation
    public class GridTileSpawner : ITileSpawner
    {
        private int _gridHeight;
        private int _gridWidth;
        private Vector3 _tileScale;
        private Transform _parent;

        public GridTileSpawner(int gridHeight, int gridWidth, Vector3 tileScale, Transform parent)
        {
            _gridHeight = gridHeight;
            _gridWidth = gridWidth;
            _tileScale = tileScale;
            _parent = parent;
        }

        // maybe this takes in the state, then spawns the tile and walls

        public ITile[] SpawnTiles()
        {
            var tiles = new BaseTile[_gridHeight * _gridWidth];

            for (int x = 0; x < _gridWidth; x++)
            {
                for (int y = 0; y < _gridHeight; y++)
                {
                    var coordinates = new Coordinates(x, y);
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.parent = _parent;
                    cube.transform.localPosition = Vector3.zero;
                    cube.transform.position = new Vector3(
                        x - (_gridWidth / 2),
                        0,
                        y - (_gridHeight / 2)
                    );
                    cube.transform.localScale = _tileScale;
                    cube.name = coordinates.GameId;

                    BaseTile tile = cube.GetOrAddComponent<BaseTile>();
                }
            }
            return tiles;
        }
    }
}
