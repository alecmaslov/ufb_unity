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

namespace UFB.Player
{

    [RequireComponent(typeof(EffectsController))]
    public class PlayerManager : MonoBehaviour
    {
        private List<PlayerEntity> _players = new List<PlayerEntity>();
        public EffectsController Effects { get; private set; }
        public PlayerPillarsEffect playerPillarsEffect;

        private readonly string _playerPrefix = "Player__";

        private void Start()
        {
            RegisterEffects();
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

            var tile = GameController.Instance.GameBoard.GetTileByCoordinates(coordinates);

            playerEntity.Initialize("Player", GameController.Instance.GameBoard, tile, playerId);
            _players.Add(playerEntity);
            Debug.Log("Player spawned");
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
            playerEntity.Initialize(characterName, GameController.Instance.GameBoard, tile, playerId);
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
                var tile = GameController.Instance.GameBoard.GetTileByCoordinates(playerConfiguration.TileCoordinates);
                if (tile == null)
                {
                    Debug.LogError($"Tile with coordinates {playerConfiguration.TileCoordinates} not found");
                    continue;
                }
                player.ForceMoveToTile(tile);
            }
        }
    }
}
