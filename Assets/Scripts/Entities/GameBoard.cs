using UnityEngine;
using System.Collections.Generic;
using UFB.Map;
using UFB.Player;
using System.Linq;
using System;
using UFB.Effects;
using UFB.StateSchema;
using UFB.Core;
using Colyseus;
using UnityEngine.AddressableAssets;
using Unity.VisualScripting;

namespace UFB.Entities
{
    [RequireComponent(typeof(EffectsController), typeof(MeshMap))]
    public class GameBoard : MonoBehaviourService
    {
        public MapState State { get; private set; }
        public Dictionary<string, Tile> Tiles { get; private set; } =
            new Dictionary<string, Tile>();
        public EffectsController Effects { get; private set; }
        public int Dimensions => _map.Dimensions;

        public RippleTilesEffect RippleTilesEffect;

        private UFBMap _map;
        private ColyseusRoom<UfbRoomState> _room;

        private void OnEnable()
        {
            _room = ServiceLocator.Current.Get<GameService>().Room;
            State = _room.State.map;
            SpawnBoard();
            ServiceLocator.Current.Register(this);
        }

        private void OnDisable()
        {
            ServiceLocator.Current.Unregister<GameBoard>();
        }

        public void SpawnBoard()
        {
            Debug.Log($"Spawning board {State.name}");

            ClearBoard();

            // load the image of the map and spawn it
            Addressables.LoadAssetAsync<Sprite>(State.resourceAddress).Completed += (obj) =>
            {
                if (obj.Result == null)
                {
                    Debug.LogError($"Failed to load map {State.name}");
                    return;
                }

                var meshMapTiles = GetComponent<MeshMap>().SpawnMeshMap(obj.Result);
                // , (int)State.gridHeight, (int)State.gridWidth);

                foreach (var meshMapTile in meshMapTiles)
                {
                    var tile = meshMapTile.GameObject.AddComponent<Tile>();
                    tile.Initialize(
                        meshMapTile,
                        State.TileStateAtCoordinates(meshMapTile.Coordinates) 
                    );
                    Tiles.Add(tile.Id, tile);
                }
            };
        }

        public void ClearBoard()
        {
            Tiles.Clear();
            List<GameObject> children = new List<GameObject>();

            foreach (Transform child in transform)
            {
                children.Add(child.gameObject);
            }

            foreach (GameObject child in children)
            {
                if (Application.isEditor)
                    DestroyImmediate(child);
                else
                    Destroy(child);
            }

            transform.position = Vector3.zero;
            GetComponent<MeshMap>().Clear();
        }

        public Tile RandomTile()
        {
            return GetTileByCoordinates(
                Coordinates.Random(
                    (int)Mathf.Floor(State.gridWidth),
                    (int)Mathf.Floor(State.gridHeight)
                )
            );
        }

        public void SpawnEntity(string prefabAddress, Tile tile) =>
            Addressables.InstantiateAsync(prefabAddress, tile.transform);

        // think about making a class called EntitySpawner, that can randomly spawn entities
        // throughout the map
        public void SpawnEntitiesRandom(string prefabName, int count)
        {
            for (int i = 0; i < count; i++)
            {
                SpawnEntity(prefabName, RandomTile());
            }
        }

        public Tile GetTileByCoordinates(Coordinates coordinates)
        {
            var tile = Tiles.Values.FirstOrDefault(t => t.Coordinates.Equals(coordinates));
            if (tile == null)
                Debug.LogError($"Tile not found at coordinates {coordinates.Id}");
            return tile;
        }

        public IEnumerable<Tile> GetPathFromCoordinates(IEnumerable<Coordinates> coordinates)
        {
            return coordinates.Select(coord => GetTileByCoordinates(coord));
        }

        /// <summary>
        /// Iterates over tiles
        /// </summary>
        public void IterateTiles(Action<Tile> action)
        {
            foreach (Tile tile in Tiles.Values)
            {
                action(tile);
            }
        }

        /// <summary>
        /// Iterates over tiles with a normalized index
        /// </summary>
        public void IterateTiles(Action<Tile, float> action)
        {
            for (int i = 0; i < Tiles.Count; i++)
            {
                action(Tiles.Values.ElementAt(i), i / Tiles.Count);
            }
        }

        public Tile[] GetAdjacentTiles(Tile tile, bool ignoreWalls = false)
        {
            var adjacent = tile.Coordinates.Adjacent(
                0,
                _map.Dimensions - 1,
                0,
                _map.Dimensions - 1
            );
            var tiles = adjacent.Select(coords => GetTileByCoordinates(coords));
            // if (!ignoreWalls) {
            //     tiles = tiles.Where(t => !t.BlockedByWall(tile));
            // }
            return tiles.ToArray();
        }

        /// <summary>
        /// Runs a ripple effect around a specific tile
        /// <summary>
        public void RunRippleEffect(Tile tile) => RippleTilesEffect.ExecuteOnTile(tile);

        private void RegisterEffects()
        {
            Effects = GetComponent<EffectsController>();
            Effects.RegisterEffect("RandomTileStretch", new RandomTileStretchEffect(this, 1.5f));
            Effects.RegisterEffect("ResetTiles", new ResetTilesEffect(this, 0.5f));
            Tile centerTile = GetTileByCoordinates(
                new Coordinates(_map.Dimensions / 2, _map.Dimensions / 2)
            );
            RippleTilesEffect = new RippleTilesEffect(this, centerTile, 10f);
            Effects.RegisterEffect("RippleTiles", RippleTilesEffect);
        }

        public Vector3 CoordinatesToPosition(Coordinates coords) =>
            new Vector3(Dimensions - coords.X, 0f, coords.Y);
    }
}


// public TileEntity SpawnTile(GameTile tile)
// {
//     var tilePrefab = Resources.Load("Prefabs/Tile") as GameObject;
//     var tileObject = Instantiate(tilePrefab, this.transform);
//     TileEntity tileEntity = tileObject.GetComponent<TileEntity>();
//     tileEntity.Initialize(tile, this);
//     return tileEntity;
// }
// private List<TileEntity> _tiles = new List<TileEntity>();

// public void Initialize(MapState mapState)
// {
//     ClearBoard();
//     Debug.Log($"Initializing GameBoard with map {mapState.name}");
//     SpawnBoard(mapState.name);
//     RegisterEffects();

//     if (Instance != null)
//     {
//         Debug.LogWarning($"GameBoard already exists, destroying {gameObject.name}");
//         Destroy(gameObject);
//     }
//     Instance = this;
// }
// public void SpawnBoard(string mapName)
// {
//     gameObject.name = $"GameBoard__{mapName}";
//     MapName = mapName;

//     // @streamingassets
//     var mapInfo = Resources.Load($"Maps/{mapName}/map") as TextAsset;

//     _map = MapParser.Parse(mapInfo);

//     if (_map == null)
//     {
//         Debug.LogError($"Map {mapName} not found");
//         return;
//     }

//     foreach (GameTile tile in _map.Tiles)
//     {
//         _tiles.Add(SpawnTile(tile));
//     }

//     // normalize the board position to 0,0,0
//     // Vector3 center = _tiles.Aggregate(Vector3.zero, (acc, tile) => acc + tile.transform.position) / _tiles.Count;
//     // transform.Translate(-center, Space.World);
//     transform.Translate(-_map.Dimensions / 2, 0, -_map.Dimensions / 2, Space.World);

//     // transform.position = -center;
// }
