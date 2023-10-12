using UnityEngine;
using System.Collections.Generic;
using UFB.Map;
using System.Linq;
using System;
using UFB.Effects;
using UFB.StateSchema;
using UFB.Core;
using Colyseus;
using UnityEngine.AddressableAssets;
using UFB.Events;
using UFB.Network.RoomMessageTypes;
using UnityEngine.ResourceManagement.AsyncOperations;
using UFB.Entities;

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
            // maybe make a global way of sending message requests to the room? <- THIS
            // EventBus.Publish(
            //     new RoomSendMessageEvent(
            //         "move",
            //         new RequestMoveMessage { tileId = e.tile.Id, destination = e.tile.Coordinates }
            //     )
            // );
        }

        public void SpawnBoard(MapState state)
        {
            Debug.Log($"Spawning board {state.name}");

            ClearBoard();

            var task = Addressables.LoadAssetAsync<Sprite>(state.resourceAddress);

            EventBus.Publish(new DownloadProgressEvent(task, $"Map {state.name}"));

            // load the image of the map and spawn it
            task.Completed += (obj) =>
            {
                // catch handle load errors
                if (obj.Status == AsyncOperationStatus.Failed)
                {
                    Debug.LogError($"Failed to load map {state.name}");
                    return;
                }
                LoadBoardFromSprite(obj.Result, state);
                SpawnEntities(state);
                _state = state;
            };
        }

        private void LoadBoardFromSprite(Sprite boardSprite, MapState state)
        {
            var meshMapTiles = GetComponent<MeshMap>()
                .SpawnTiles(
                    boardSprite,
                    (int)state.gridWidth,
                    (int)state.gridHeight,
                    new Vector3(1, 0.1f, 1)
                );

            foreach (var meshMapTile in meshMapTiles)
            {
                var tile = meshMapTile.GameObject.AddComponent<Tile>();
                tile.Initialize(meshMapTile, state.TileStateAtCoordinates(meshMapTile.Coordinates));
                Tiles.Add(tile.Id, tile);
            }
        }

        private void SpawnEntities(MapState state)
        {
            // Debug.Log($"Spawning entities for map {state.name} | {state.spawnEntities.ToDetailedString()}");
            foreach (var entity in state.spawnEntities.items.Values)
            {
                // var tile = Tiles[entity.tileId];
                Debug.Log(
                    $"Spawning entity {entity.prefabAddress}"
                );
                // SpawnEntity(entity.prefabAddress, tile);
                SpawnEntity(entity);
            }
        }

        public async void SpawnEntity(StateSchema.SpawnEntity spawnEntity)
        {
            try
            {
                var tile = Tiles[spawnEntity.tileId];
                var task = Addressables.InstantiateAsync(
                    spawnEntity.prefabAddress,
                    tile.transform,
                    true
                );
                var go = await task.Task;
                tile.AttachGameObject(go, true);
                go.GetComponent<ISpawnableEntity>().Initialize(spawnEntity);
            }
            catch (Exception e)
            {
                Debug.LogError(
                    $"Failed to spawn entity {spawnEntity.prefabAddress} at tile {spawnEntity.tileId}"
                );
                Debug.LogError(e);
            }
        }

        public async void SpawnEntity(string prefabAddress, Tile tile)
        {
            var task = Addressables.InstantiateAsync(prefabAddress, tile.transform, true);
            var go = await task.Task;
            tile.AttachGameObject(go, true);
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
