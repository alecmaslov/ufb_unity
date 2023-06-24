using UnityEngine;
using System.Collections.Generic;
using UFB.Map;
using UFB.Player;
using System.Linq;
using System;

namespace UFB.Entities {

    public class GameBoard : MonoBehaviour
    {
        public string MapName { get; set; }
        private List<TileEntity> _tiles = new List<TileEntity>();
        
        public void SpawnBoard(string mapName)
        {
            gameObject.name = $"GameBoard__{mapName}";
            MapName = mapName;
            var mapInfo = Resources.Load($"Maps/{mapName}/map") as TextAsset;
            UFBMap map = MapParser.Parse(mapInfo);

            foreach(GameTile tile in map.Tiles)
            {
                _tiles.Add(SpawnTile(tile));
            }

            // normalize the board position to 0,0,0
            transform.Translate(-map.Dimensions / 2, 0, -map.Dimensions / 2, Space.World);
        }

        public TileEntity SpawnTile(GameTile tile) {
            var tilePrefab = Resources.Load("Prefabs/Tile") as GameObject;
            var tileObject = Instantiate(tilePrefab, new Vector3(tile.Coordinates.X, 0, tile.Coordinates.Y), Quaternion.identity);
            tileObject.transform.parent = this.transform;
            // rotate tileObject by 270 degrees on y axis
            tileObject.transform.Rotate(0, 270, 0, Space.Self);
            TileEntity tileEntity = tileObject.GetComponent<TileEntity>();
            Debug.Log("TILE ENTITY: " + tileEntity + " tile.gettexture " + tile.GetTexture(MapName) + " tile.getcolor " + tile.GetColor());
            tileEntity.Initialize(tile, tile.GetTexture(MapName), tile.GetColor());
            return tileEntity;
        }

        public TileEntity GetTileById(string id) {
            return _tiles.Find(tile => tile.GameTile.Id == id);
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

    }
}