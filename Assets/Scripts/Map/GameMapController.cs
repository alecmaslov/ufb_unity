using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Colyseus;
using UFB.Core;
using UFB.Effects;
using UFB.Entities;
using UFB.Events;
using UFB.Map;
using UFB.Network.RoomMessageTypes;
using UFB.StateSchema;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UFB.Map
{
    public class GameMapController : MonoBehaviourService
    {
        public Dictionary<string, BaseTile> Tiles { get; private set; } =
            new Dictionary<string, BaseTile>();
        private MapState _state;
        private ITileMapGenerator _mapGenerator;

        private void OnEnable()
        {
            EventBus.Subscribe<GameReadyEvent>(OnGameReadyEvent);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameReadyEvent>(OnGameReadyEvent);
        }

        private void OnGameReadyEvent(GameReadyEvent e)
        {
            ServiceLocator.Current.Register(this);
            SpawnBoard(e.room.State.map);
        }

        public void SpawnBoard(MapState mapState)
        {
            if (!TryGetComponent<ITileMapGenerator>(out var mapGenerator))
            {
                throw new Exception(
                    "No map renderer found, attach an IMapRenderer to the GameMapController"
                );
            }
            _mapGenerator = mapGenerator;
            _mapGenerator
                .GenerateTiles(mapState)
                .ContinueWith(
                    (task) =>
                    {
                        if (task.IsFaulted)
                        {
                            Debug.LogError($"Error generating tiles: {task.Exception}");
                            return;
                        }
                        var tiles = task.Result;
                        foreach (var tile in tiles)
                        {
                            Tiles.Add(tile.Id, tile);
                        }

                        SpawnEntities(mapState);
                    }
                );
        }

        private void SpawnEntities(MapState state)
        {
            // Debug.Log($"Spawning entities for map {state.name} | {state.spawnEntities.ToDetailedString()}");
            foreach (var entity in state.spawnEntities.items.Values)
            {
                // var tile = Tiles[entity.tileId];
                Debug.Log($"Spawning entity {entity.prefabAddress}");
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

        public async void SpawnEntity(string prefabAddress, BaseTile tile)
        {
            var task = Addressables.InstantiateAsync(prefabAddress, tile.transform, true);
            var go = await task.Task;
            tile.AttachGameObject(go, true);
        }

        public void ClearBoard()
        {
            Tiles.Clear();
            gameObject.DestroyAllChildren();
            transform.position = Vector3.zero;
            _mapGenerator.Clear();
        }

        public BaseTile RandomTile()
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

        public BaseTile GetTileByCoordinates(Coordinates coordinates)
        {
            var tile = Tiles.Values.FirstOrDefault(t => t.Coordinates.Equals(coordinates));
            if (tile == null)
                Debug.LogError($"Tile not found at coordinates {coordinates.Id}");
            return tile;
        }

        public IEnumerable<BaseTile> GetPathFromCoordinates(IEnumerable<Coordinates> coordinates)
        {
            return coordinates.Select(coord => GetTileByCoordinates(coord));
        }

        /// <summary>
        /// Iterates over tiles
        /// </summary>
        public void IterateTiles(Action<BaseTile> action)
        {
            foreach (BaseTile tile in Tiles.Values)
            {
                action(tile);
            }
        }

        /// <summary>
        /// Iterates over tiles with a normalized index
        /// </summary>
        public void IterateTiles(Action<BaseTile, float> action)
        {
            for (int i = 0; i < Tiles.Count; i++)
            {
                action(Tiles.Values.ElementAt(i), i / Tiles.Count);
            }
        }

        public IEnumerable<BaseTile> GetTilesByIds(IEnumerable<string> ids)
        {
            return ids.Select(id => Tiles[id]);
        }

        public BaseTile[] GetAdjacentTiles(BaseTile tile, bool ignoreWalls = false)
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
