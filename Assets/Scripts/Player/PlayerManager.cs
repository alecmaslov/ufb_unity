using UnityEngine;
using UFB.Entities;
using System.Collections.Generic;

namespace UFB.Player {
    public class PlayerManager : MonoBehaviour {
        private List<PlayerEntity> _players = new List<PlayerEntity>();

        private readonly string _playerPrefix = "Player__";

        public void SpawnPlayer(string characterName) {
            var playerPrefab = Resources.Load($"Players/{_playerPrefix}{characterName}") as GameObject;
            var playerObject = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            playerObject.transform.parent = this.transform;
            PlayerEntity playerEntity = playerPrefab.GetComponent<PlayerEntity>();
            playerEntity.CharacterName = characterName;
            _players.Add(playerEntity);
        }
    }
}
