using UnityEngine;
using UFB.Entities;
using System.Collections.Generic;
using UFB.Map;
using UFB.Gameplay;
using System.Linq;
using System;
using UFB.Effects;
using Newtonsoft.Json;
using UFB.Core;
using UFB.StateSchema;
using Colyseus.Schema;
using Colyseus;
using UFB.Network;
using UFB.Events;

namespace UFB.Events
{
    public class RequestPlayerMoveEvent
    {
        public Coordinates Destination { get; private set; }

        public RequestPlayerMoveEvent(Coordinates destination)
        {
            Destination = destination;
        }
    }
}

namespace UFB.Player
{
    [RequireComponent(typeof(EffectsController))]
    public class PlayerManager : MonoBehaviour
    {
        private List<PlayerEntity> _players = new List<PlayerEntity>();
        public EffectsController Effects { get; private set; }
        public PlayerPillarsEffect playerPillarsEffect;

        private readonly string _playerPrefix = "Player__";

        private string _myId;

        private ColyseusRoom<UfbRoomState> _room;
        private MapSchema<PlayerState> _playerStateMap;

        public PlayerState MyPlayerState
        {
            get
            {
                return _playerStateMap[_myId];
            }
        }

        public PlayerEntity MyPlayer
        {
            get
            {
                return GetPlayerById(_myId);
            }
        }

        private void OnEnable()
        {
            EventBus.Subscribe<RequestPlayerMoveEvent>(OnRequestPlayerMove);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<RequestPlayerMoveEvent>(OnRequestPlayerMove);
        }

        public void Initialize(ColyseusRoom<UfbRoomState> room, string myId)
        {
            _room = room;
            _playerStateMap = room.State.players;
            _myId = myId;
            RegisterEffects();
            // iterate playerStateMap to spawn players
            foreach (PlayerState playerState in room.State.players.Values)
            {
                SpawnPlayer(playerState);
            }

            // listen for changes to the player state map
            _playerStateMap.OnAdd((string key, PlayerState playerState) =>
            {
                // check if the player is already spawned
                if (GetPlayerById(playerState.id) != null)
                {
                    Debug.LogError($"Player with id {playerState.id} already spawned");
                    return;
                }

                Debug.Log($"Player {playerState.id} joined the game | Spawning character {playerState.characterId}");
                SpawnPlayer(playerState);

                // here we can choose how to animate it - show the camera for a little 
                // bit as the new player joins


                var playerEntity = GetPlayerById(playerState.id);

                playerEntity.FocusCamera();
                playerEntity.TileAttachable.CurrentTile.SlamDown();
                CoroutineHelpers.DelayedAction(() =>
                {
                    MyPlayer.FocusCamera(); // focus back on the current player
                }, 3000, this); 
            });

            _playerStateMap.OnRemove((string key, PlayerState playerState) =>
            {
                var player = GetPlayerById(playerState.id);
                if (player == null)
                {
                    Debug.LogError($"Player with id {playerState.id} not found");
                    return;
                }
                _players.Remove(player);
                Destroy(player.gameObject);
            });

            room.OnMessage<PlayerMovedMessage>("playerMoved", (message) => {
                var player = GetPlayerById(message.playerId);

                // Debug.Log($"Received playerMove message! {message.Serialize()}");

                if (player == null)
                {
                    Debug.LogError($"Player with id {message.playerId} not found");
                    return;
                }

                // eventually use the PathStep to get the tileId, rather than doing all the coord nonsense

                var coordinates = message.path.Select(p => p.coord.ToCoordinates()).ToList();
                // reverse coords
                coordinates.Reverse();

                player.MoveAlongPathCoords(coordinates, 0.3f);


                // var destination = new Coordinates((int)message.destination.x, (int)message.destination.y);
                // player.ForceMoveToTile(GameManager.Instance.GameBoard.GetTileByCoordinates(destination));
            });
        }


        public PlayerEntity GetPlayerById(string playerId)
        {
            return _players.FirstOrDefault(p => p.PlayerId == playerId);
        }


        // spawns player randomly
        public void SpawnRandomPlayer(string playerId, Coordinates coordinates)
        {
            var allPrefabs = Resources.LoadAll("Players", typeof(GameObject));
            var playerPrefabs = allPrefabs.Where(p => p.name.StartsWith(_playerPrefix)).ToList();
            // randomly select a player prefab
            var playerPrefab = playerPrefabs[UnityEngine.Random.Range(0, playerPrefabs.Count)] as GameObject;

            if (playerPrefab == null)
            {
                Debug.LogError("Player prefab not found");
                return;
            }
            var playerObject = Instantiate(playerPrefab, this.transform);
            PlayerEntity playerEntity = playerObject.GetComponent<PlayerEntity>();

            var tile = GameManager.Instance.GameBoard.GetTileByCoordinates(coordinates);

            playerEntity.Initialize("Player", GameManager.Instance.GameBoard, tile, playerId);
            _players.Add(playerEntity);
            Debug.Log("Player spawned");
        }

        public void SpawnPlayer(PlayerState playerState)
        {
            var coordinates = new Coordinates((int)playerState.x, (int)playerState.y);
            var tile = GameManager.Instance.GameBoard.GetTileByCoordinates(coordinates);
            SpawnPlayer(playerState.characterId, tile, playerState.id);
        }

        public void SpawnPlayer(string characterName, TileEntity tile, string playerId = null)
        {
            var playerPrefab = Resources.Load($"Players/{_playerPrefix}{characterName}") as GameObject;
            if (playerPrefab == null)
            {
                Debug.LogError($"Player prefab not found for character name {characterName}");
                return;
            }
            var playerObject = Instantiate(playerPrefab, this.transform);
            PlayerEntity playerEntity = playerObject.GetComponent<PlayerEntity>();
            playerEntity.Initialize(characterName, GameManager.Instance.GameBoard, tile, playerId);
            _players.Add(playerEntity);
            Debug.Log("Player " + playerEntity.CharacterName + " spawned");
        }

        public void MovePlayerToTile(PlayerEntity player, TileEntity tile)
        {
            player.TryMoveToTile(tile);
        }

        private void RegisterEffects()
        {
            Effects = GetComponent<EffectsController>();
            playerPillarsEffect = new PlayerPillarsEffect(this);
            Effects.RegisterEffect("PillarsRise", playerPillarsEffect);
        }

        public void IteratePlayers(Action<PlayerEntity> action)
        {
            foreach (var player in _players)
            {
                action(player);
            }
        }

        public void SavePlayerConfiguration(string fileName)
        {

            // this will eventually be handled by the Player object, which PlayerEntity has a reference
            // to. It will handle loading/unloading the JSON into the player state. For now, quick solution
            var json = JsonConvert.SerializeObject(_players.Select(p => new PlayerConfiguration
            {
                CharacterName = p.CharacterName,
                TileCoordinates = p.CurrentTile.Coordinates
            }).ToList());

            // save the json
            ApplicationData.SaveJSON(json, "gamestate/player-config", fileName + ".json");
        }

        public void LoadPlayerConfiguration(string fileName)
        {
            // load the json
            var playerConfigurations = ApplicationData.LoadJSON<List<PlayerConfiguration>>("gamestate/player-config", fileName + ".json");

            // iterate through the players and set their tile coordinates
            foreach (var playerConfiguration in playerConfigurations)
            {
                var player = _players.FirstOrDefault(p => p.CharacterName == playerConfiguration.CharacterName);
                if (player == null)
                {
                    Debug.LogError($"Player with character name {playerConfiguration.CharacterName} not found");
                    continue;
                }
                var tile = GameManager.Instance.GameBoard.GetTileByCoordinates(playerConfiguration.TileCoordinates);
                if (tile == null)
                {
                    Debug.LogError($"Tile with coordinates {playerConfiguration.TileCoordinates} not found");
                    continue;
                }
                player.ForceMoveToTile(tile);
            }
        }


        /// <summary>
        /// Requests that the player move to the given destination.
        /// </summary>
        /// <param name="destination"></param>
        private async void OnRequestPlayerMove(RequestPlayerMoveEvent e)
        {
            Debug.Log($"[PlayerManager] Requesting move to {e.Destination.ToString()}");
            await _room.Send("move", new Dictionary<string, object>() {
                { "destination", e.Destination.ToDictionary() }
            });

        }
    }
}
