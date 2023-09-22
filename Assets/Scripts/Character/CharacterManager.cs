using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using UFB.StateSchema;
using UFB.Entities;
using UFB.Events;
using Colyseus;
using UFB.Network;
using UnityEngine;
using System.Linq;
using UFB.Gameplay;

namespace UFB.Character
{
    public class CharacterManager : MonoBehaviour
    {
        public CharacterController PlayerCharacter => _characters[_playerId];

        [SerializeField] private AssetLabelReference _charactersLabel;
        [SerializeField] private AssetReference _characterPrefab;

        // private List<CharacterController> _characters = new List<CharacterController>();
        private Dictionary<string, CharacterController> _characters = new Dictionary<string, CharacterController>();
        private ColyseusRoom<UfbRoomState> _room;

        private string _playerId; // during zombie mode, we can set this to a different player


        private void OnEnable()
        {
            EventBus.Subscribe<RequestPlayerMoveEvent>(OnRequestPlayerMove);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<RequestPlayerMoveEvent>(OnRequestPlayerMove);
        }

        public void Initialize(ColyseusRoom<UfbRoomState> room, string playerId)
        {

            _playerId = playerId;
            _room = room;

            _characters = new Dictionary<string, CharacterController>();

            foreach(var key in room.State.players.Keys)
            {
                // var player = room.State.players[key];
                // var task = Addressables.InstantiateAsync(_characterPrefab, transform);
                // task.WaitForCompletion();
                // var character = task.Result.GetComponent<CharacterController>();
                // character.Spawn(player.character, player);
                // _characters.Add(key, character);
            }


            room.State.players.OnAdd(OnPlayerAdded);

            room.OnMessage<PlayerMovedMessage>("playerMoved", OnPlayerMoved);

            // message can be scoped on the server to send only to specific client
            room.OnMessage<BecomeZombieMessage>("becomeZombie", (message) => {
                _playerId = message.playerId;
            });
        }

        private void SpawnPlayer()
        {

        }

        private void OnPlayerAdded(string key, PlayerState playerState)
        {
            EventBus.Publish(new ToastMessageEvent($"Player {playerState.id} has joined the game!"));

            // var task = Addressables.InstantiateAsync(_characterPrefab, transform);
            _characterPrefab.InstantiateAsync(transform).Completed += (obj) =>
            {
                var character = obj.Result.GetComponent<CharacterController>();
                // character.Spawn(playerState.character, playerState);
                // _characters.Add(key, character);
            };
        }

        private void OnPlayerMoved(PlayerMovedMessage m)
        {
            var coordinates = m.path.Select(p => p.coord.ToCoordinates());
            coordinates.Reverse();
            var path = GameManager.Instance.GameBoard.GetPathFromCoordinates(coordinates);
            _characters[m.playerId].Movement.MoveAlongPath(path);
        }

        private async void OnRequestPlayerMove(RequestPlayerMoveEvent e)
        {
            Debug.Log($"[PlayerManager] Requesting move to {e.Destination.ToString()}");
            await _room.Send("move", new Dictionary<string, object>() {
                { "destination", e.Destination.ToDictionary() }
            });
        }

    }

}