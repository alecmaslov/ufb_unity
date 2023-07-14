using UnityEngine;
using UFB.Entities;
using System.Collections.Generic;
using UFB.Map;
using UFB.Gameplay;

namespace UFB.Player {
    public class PlayerManager : MonoBehaviour {
        private List<PlayerEntity> _players = new List<PlayerEntity>();

        private readonly string _playerPrefix = "Player__";

        public void SpawnPlayer(string characterName, TileEntity tile) {
            var playerPrefab = Resources.Load($"Players/{_playerPrefix}{characterName}") as GameObject;
            if (playerPrefab == null) {
                Debug.LogError($"Player prefab not found for character name {characterName}");
                return;
            }
            var playerObject = Instantiate(playerPrefab, this.transform);
            PlayerEntity playerEntity = playerObject.GetComponent<PlayerEntity>();
            playerEntity.Initialize(characterName, GameController.Instance.GameBoard, tile);

            _players.Add(playerEntity);
            Debug.Log("Player " + playerEntity.CharacterName + " spawned");
        }

        public void MovePlayerToTile(PlayerEntity player, TileEntity tile) {
            player.TryMoveToTile(tile);
        }
    }
}
