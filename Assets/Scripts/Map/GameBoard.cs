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
using UFB.Events;
using UFB.Network.RoomMessageTypes;

namespace UFB.Map
{
    [RequireComponent(typeof(MeshMap))]
    public class GameBoard : MonoBehaviourService
    {
        public Dictionary<string, Tile> Tiles { get; private set; } =
            new Dictionary<string, Tile>();
        public RippleTilesEffect RippleTilesEffect;
        private MapState _state;

        private void OnEnable()
        {
            ServiceLocator.Current.Register(this);
            SpawnBoard(ServiceLocator.Current.Get<GameService>().RoomState.map);
            EventBus.Subscribe<TileClickedEvent>(OnTileClickedEvent);
            // RegisterEffects();
        }

        private void OnDisable()
        {
            ServiceLocator.Current.Unregister<GameBoard>();
            EventBus.Unsubscribe<TileClickedEvent>(OnTileClickedEvent);
        }

        private void OnTileClickedEvent(TileClickedEvent e)
        {
            // depending on the selection mode of the GameBoard, it may or may not
            // trigger a player move

            // switch some tile click mode to determine if this is what should happen?

            // maybe make a global way of sending message requests to the room? <- THIS
            EventBus.Publish(
                new RoomSendMessageEvent(
                    "move",
                    new RequestMoveMessage { tileId = e.tile.Id, destination = e.tile.Coordinates }
                )
            );
        }

        public void SpawnBoard(MapState state)
        {
            Debug.Log($"Spawning board {state.name}");

            ClearBoard();

            // load the image of the map and spawn it
            Addressables.LoadAssetAsync<Sprite>(state.resourceAddress).Completed += (obj) =>
            {
                var meshImage = obj.Result;
                if (meshImage == null)
                {
                    Debug.LogError($"Failed to load map {state.name}");
                    return;
                }

                var meshMapTiles = GetComponent<MeshMap>()
                    .SpawnTiles(
                        obj.Result,
                        (int)state.gridWidth,
                        (int)state.gridHeight,
                        new Vector3(1, 0.1f, 1)
                    );

                foreach (var meshMapTile in meshMapTiles)
                {
                    var tile = meshMapTile.GameObject.AddComponent<Tile>();
                    tile.Initialize(
                        meshMapTile,
                        state.TileStateAtCoordinates(meshMapTile.Coordinates)
                    );
                    Tiles.Add(tile.Id, tile);
                }
            };

            _state = state;
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
                    (int)Mathf.Floor(_state.gridWidth),
                    (int)Mathf.Floor(_state.gridHeight)
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

        public IEnumerable<Tile> GetTilesByIds(IEnumerable<string> ids)
        {
            return ids.Select(id => Tiles[id]);
        }

        public Tile[] GetAdjacentTiles(Tile tile, bool ignoreWalls = false)
        {
            var adjacent = tile.Coordinates.Adjacent(
                0,
                (int)_state.gridWidth - 1,
                0,
                (int)_state.gridHeight - 1
            );
            var tiles = adjacent.Select(coords => GetTileByCoordinates(coords));
            return tiles.ToArray();
        }
    }
}
