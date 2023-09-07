using UnityEngine;
using System.Collections.Generic;
using UFB.Map;
using UFB.Player;
using System.Linq;
using System;
using UFB.Effects;

namespace UFB.Entities
{

    public class GameBoard : MonoBehaviour
    {
        public string MapName { get; set; }
        public EffectsController Effects { get; private set; }
        public int Dimensions { get => _map.Dimensions; }

        private List<TileEntity> _tiles = new List<TileEntity>();
        private UFBMap _map;

        public RippleTilesEffect RippleTilesEffect;

        private void Start()
        {
            RegisterEffects();
        }

        private void RegisterEffects()
        {
            Effects = GetComponent<EffectsController>();
            Effects.RegisterEffect("RandomTileStretch", new RandomTileStretchEffect(this, 1.5f));
            Effects.RegisterEffect("ResetTiles", new ResetTilesEffect(this, 0.5f));
            TileEntity centerTile = GetTileByCoordinates(new Coordinates(_map.Dimensions / 2, _map.Dimensions / 2));
            RippleTilesEffect = new RippleTilesEffect(this, centerTile, 10f);
            Effects.RegisterEffect("RippleTiles", RippleTilesEffect);
        }

        public void ClearBoard()
        {
            _tiles.Clear();
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
        }


        public void SpawnBoard(string mapName)
        {
            gameObject.name = $"GameBoard__{mapName}";
            MapName = mapName;
            var mapInfo = Resources.Load($"Maps/{mapName}/map") as TextAsset;
            _map = MapParser.Parse(mapInfo);

            if (_map == null)
            {
                Debug.LogError($"Map {mapName} not found");
                return;
            }


            foreach (GameTile tile in _map.Tiles)
            {
                _tiles.Add(SpawnTile(tile));
            }

            // normalize the board position to 0,0,0
            // Vector3 center = _tiles.Aggregate(Vector3.zero, (acc, tile) => acc + tile.transform.position) / _tiles.Count;
            // transform.Translate(-center, Space.World);
            transform.Translate(-_map.Dimensions / 2, 0, -_map.Dimensions / 2, Space.World);


            // transform.position = -center;
        }

        public Coordinates RandomCoordinates()
        {
            return new Coordinates(UnityEngine.Random.Range(0, _map.Dimensions), UnityEngine.Random.Range(0, _map.Dimensions));
        }

        public TileEntity RandomTile()
        {
            return GetTileByCoordinates(RandomCoordinates());
        }

        public TileEntity SpawnTile(GameTile tile)
        {
            var tilePrefab = Resources.Load("Prefabs/Tile") as GameObject;
            var tileObject = Instantiate(tilePrefab, this.transform);
            TileEntity tileEntity = tileObject.GetComponent<TileEntity>();
            tileEntity.Initialize(tile, this);
            return tileEntity;
        }

        public void SpawnEntity(string prefabName, GameTile tile) // change this to TileEntity
        {
            var prefab = Resources.Load($"Prefabs/{prefabName}") as GameObject;
            var entityObject = Instantiate(prefab, this.transform);
            TileAttachable entity = entityObject.GetComponent<TileAttachable>();
            entity.AttachToTile(GetTileById(tile.Id));
            // Debug.Log($"Spawned {prefabName} on tile {tile.Id}");
        }

        public void SpawnEntity(string prefabName, Coordinates coordinates)
        {
            SpawnEntity(prefabName, GetTileByCoordinates(coordinates).GameTile);
        }


        // think about making a class called EntitySpawner, that can randomly spawn entities
        // throughout the map
        public void SpawnEntitiesRandom(string prefabName, int count)
        {
            for (int i = 0; i < count; i++)
            {
                SpawnEntity(prefabName, RandomTile().GameTile);
            }
        }

        public TileEntity GetTileById(string id)
        {
            return _tiles.Find(tile => tile.GameTile.Id == id);
        }

        public TileEntity GetTileByCoordinates(Coordinates coordinates)
        {
            return _tiles.Find(tile => tile.GameTile.Coordinates.Equals(coordinates));
        }

        /// <summary>
        /// Iterates over tiles
        /// </summary>
        public void IterateTiles(Action<TileEntity> action)
        {
            foreach (TileEntity tile in _tiles)
            {
                action(tile);
            }
        }

        /// <summary>
        /// Iterates over tiles with a normalized index
        /// </summary>
        public void IterateTiles(Action<TileEntity, float> action)
        {
            for (int i = 0; i < _tiles.Count; i++)
            {
                action(_tiles[i], i / _tiles.Count);
            }
        }


        public TileEntity[] GetAdjacentTiles(TileEntity tile, bool ignoreWalls = false)
        {
            var adjacent = tile.Coordinates.Adjacent(0, _map.Dimensions - 1, 0, _map.Dimensions - 1);
            var tiles = adjacent.Select(coords => GetTileByCoordinates(coords));
            // if (!ignoreWalls) {
            //     tiles = tiles.Where(t => !t.BlockedByWall(tile));
            // }
            return tiles.ToArray();
        }


        public List<TileEntity> Pathfind(Coordinates start, Coordinates end)
        {
            return Pathfinder.FindTilePath(GetTileByCoordinates(start), GetTileByCoordinates(end), this);
        }

        public List<TileEntity> Pathfind(TileEntity start, TileEntity end)
        {
            return Pathfinder.FindTilePath(start, end, this);
        }

        /// <summary>
        /// Runs a ripple effect around a specific tile
        /// <summary>
        public void RunRippleEffect(TileEntity tile)
        {
            RippleTilesEffect.ExecuteOnTile(tile);
        }

    }
}