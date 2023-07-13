using UnityEngine;
using System.Collections.Generic;
using UFB.Map;
using UFB.Player;
using System.Linq;
using System;
using UFB.Effects;

namespace UFB.Entities {

    public class GameBoard : MonoBehaviour
    {
        public string MapName { get; set; }
        public EffectsController Effects { get; private set; }
        private List<TileEntity> _tiles = new List<TileEntity>();
        private UFBMap _map;


        private void Start()
        {
            Effects = GetComponent<EffectsController>();
            Effects.RegisterEffect("RandomTileStretch", new RandomTileStretchEffect(this, 1.5f));
            Effects.RegisterEffect("ResetTiles", new ResetTilesEffect(this, 0.5f));
        }

        
        public void SpawnBoard(string mapName)
        {
            gameObject.name = $"GameBoard__{mapName}";
            MapName = mapName;
            var mapInfo = Resources.Load($"Maps/{mapName}/map") as TextAsset;
            _map = MapParser.Parse(mapInfo);

            if (_map == null) {
                Debug.LogError($"Map {mapName} not found");
                return;
            }

            foreach(GameTile tile in _map.Tiles)
            {
                _tiles.Add(SpawnTile(tile));
            }

            // normalize the board position to 0,0,0
            transform.Translate(-_map.Dimensions / 2, 0, -_map.Dimensions / 2, Space.World);
        }

        public Coordinates RandomCoordinates()
        {
            return new Coordinates(UnityEngine.Random.Range(0, _map.Dimensions), UnityEngine.Random.Range(0, _map.Dimensions));
        }

        public TileEntity RandomTile()
        {
            return GetTileByCoordinates(RandomCoordinates());
        }

        public TileEntity SpawnTile(GameTile tile) {
            var tilePrefab = Resources.Load("Prefabs/Tile") as GameObject;
            var tileObject = Instantiate(tilePrefab, this.transform);
            TileEntity tileEntity = tileObject.GetComponent<TileEntity>();
            tileEntity.Initialize(tile, tile.GetTexture(MapName), tile.GetColor());
            return tileEntity;
        }

        public TileEntity GetTileById(string id) {
            return _tiles.Find(tile => tile.GameTile.Id == id);
        }

        public TileEntity GetTileByCoordinates(Coordinates coordinates) {
            return _tiles.Find(tile => tile.GameTile.Coordinates.Equals(coordinates));
        }

        /// <summary>
        /// Iterates over tiles
        /// </summary>
        public void IterateTiles(Action<TileEntity> action) {
            foreach (TileEntity tile in _tiles) {
                action(tile);
            }
        }

        /// <summary>
        /// Iterates over tiles with a normalized index
        /// </summary>
        public void IterateTiles(Action<TileEntity, float> action) {
            for (int i = 0; i < _tiles.Count; i++) {
                action(_tiles[i], i/_tiles.Count);
            }
        }

        
        public TileEntity[] GetAdjacentTiles(TileEntity tile, bool ignoreWalls = false) {
            var adjacent = tile.Coordinates.Adjacent(0, _map.Dimensions - 1, 0, _map.Dimensions - 1);
            var tiles = adjacent.Select(coords => GetTileByCoordinates(coords));
            if (!ignoreWalls) {
                tiles = tiles.Where(t => !t.BlockedByWall(tile));
            }
            return tiles.ToArray();
        }


        public List<TileEntity> Pathfind(Coordinates start, Coordinates end)
        {
            return Pathfinder.FindTilePath(GetTileByCoordinates(start), GetTileByCoordinates(end), this);
        }

    }
}

// return tile.Coordinates.Adjacent(0, _map.Dimensions - 1, 0, _map.Dimensions - 1)
//     .Select(coords => GetTileByCoordinates(coords))
//     .ToArray();
// Coordinates[] adjacentCoords = tile.Coordinates.Adjacent(0, _map.Dimensions - 1, 0, _map.Dimensions - 1);
// List<TileEntity> adjacentTiles = new List<TileEntity>();
// foreach (Coordinates coords in adjacentCoords) {
//     adjacentTiles.Add(GetTileByCoordinates(coords));
// }
// return adjacentTiles.ToArray();
// return _tiles.FindAll(t => adjacentCoords.Contains(t.Coordinates)).ToArray();