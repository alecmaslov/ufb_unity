using UnityEngine;
using System.Collections.Generic;
using UFB.Map;
using System.Linq;

namespace UFB.Entities {

    // handles spawning in tiles, managing them,
    // and anything related to any parent gameobject
    // of the tiles
    public class GameBoard : MonoBehaviour
    {
        public string MapName { get; set; }
        private List<TileEntity> _tiles = new List<TileEntity>();
        private List<PlayerEntity> _players = new List<PlayerEntity>();

        public void SpawnBoard(string mapName)
        {
            MapName = mapName;
            var mapInfo = Resources.Load($"Maps/{mapName}/map") as TextAsset;
            UFBMap map = MapParser.Parse(mapInfo);

            foreach(GameTile tile in map.Tiles)
            {
                _tiles.Add(SpawnTile(tile));
            }

            // normalize the board position to 0,0,0
            transform.Translate(-26 / 2, 0, -26 / 2, Space.World);
        }

        public void SpawnPlayer(string characterName) {
            // _players.Add(player);
            var playerPrefab = Resources.Load("Prefabs/Player") as GameObject;
            var playerObject = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            playerObject.transform.parent = this.transform;
            PlayerEntity playerEntity = playerPrefab.GetComponent<PlayerEntity>();
            playerEntity.CharacterName = characterName;
            _players.Add(playerEntity);
        }

        public TileEntity SpawnTile(GameTile tile) {
            var tilePrefab = Resources.Load("Prefabs/Tile") as GameObject;
            var tileObject = Instantiate(tilePrefab, new Vector3(tile.Coordinates.X, 0, tile.Coordinates.Y), Quaternion.identity);
            tileObject.transform.parent = this.transform;
            // rotate tileObject by 270 degrees on y axis
            tileObject.transform.Rotate(0, 270, 0, Space.Self);
            TileEntity tileEntity = tilePrefab.GetComponent<TileEntity>();
            var texture = tile.GetTexture(MapName);
            var color = tile.GetColor();
            tileEntity.Initialize(tile, texture, color);
            return tileEntity;
        }

        public TileEntity GetTileById(string id) {
            return _tiles.Find(tile => tile.GameTile.Id == id);
        }

    }
}