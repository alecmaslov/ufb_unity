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
using UFB.Player;
using System;
using UFB.Map;
using UFB.Network.RoomMessageTypes;
using UFB.Core;

namespace UFB.Events
{
    public class RequestCharacterMoveEvent
    {
        public Coordinates Destination { get; private set; }
        public RequestCharacterMoveEvent(Coordinates destination)
        {
            Destination = destination;
        }
    }

    public class CharacterPlacedEvent
    {
        public Character.CharacterController character;
        public CharacterPlacedEvent(Character.CharacterController character)
        {
            this.character = character;
        }
    }
}

namespace UFB.Character
{

    // will need to be used even before game scene starts, since main menu
    // will require access to characters
    public class CharacterManager : MonoBehaviourService
    {
        public CharacterController PlayerCharacter => _characters[_characterId];

        [SerializeField] private AssetReference _characterPrefab;

        private Dictionary<string, CharacterController> _characters = new Dictionary<string, CharacterController>();
        private ColyseusRoom<UfbRoomState> _room;

        private string _characterId;
        private string _playerId; // during zombie mode, we can set this to a different player


        private void OnEnable()
        {
            _room = ServiceLocator.Current.Get<GameService>().Room;
            if (_room == null) { Debug.LogError("Room is null"); return; }

            var clientId = ServiceLocator.Current.Get<NetworkService>().ClientId;
            _characterId = _room.State.playerCharacters[clientId]; // we have to translate this to characterId

            _room.State.characters.OnAdd(OnPlayerAdded);
            _room.State.characters.OnRemove(OnCharacterRemoved);
            _room.OnMessage<CharacterMovedMessage>("characterMoved", OnCharacterMoved);
            _room.OnMessage<BecomeZombieMessage>("becomeZombie", (message) =>
            {
                _playerId = message.playerId;
            });

            _characters = new Dictionary<string, CharacterController>();
            
            // message can be scoped on the server to send only to specific client
            EventBus.Subscribe<RequestCharacterMoveEvent>(OnRequestCharacterMove);

            // for now, do this for default behavior, but eventually this will be triggered by some UI handler
            EventBus.Subscribe<TileClickedEvent>((e) =>
            {
                EventBus.Publish(new RequestCharacterMoveEvent(e.tile.Coordinates));
            });


            ServiceLocator.Current.Register(this);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<RequestCharacterMoveEvent>(OnRequestCharacterMove);


            ServiceLocator.Current.Unregister<CharacterManager>();
        }

        private void OnPlayerAdded(string key, CharacterState characterState)
        {
            EventBus.Publish(new ToastMessageEvent($"Player {characterState.id} has joined the game!"));
            Debug.Log($"[CharacterManager] Player {characterState.id} has joined the game!");

            // Load the UfbCharacter asset
            LoadCharacter(characterState.characterClass, (ufbCharacter) =>
            {
                _characterPrefab.InstantiateAsync(transform).Completed += (obj) =>
                {
                    Debug.Log($"Instantiated character prefab: {obj.Result.name}");
                    if (obj.Status == AsyncOperationStatus.Failed)
                        throw new Exception($"Failed to instantiate character prefab: {obj.OperationException.Message}");
                    var character = obj.Result.GetComponent<CharacterController>();
                    var task = character.Spawn(ufbCharacter, characterState);
                    task.ContinueWith((t) =>
                    {
                        _characters.Add(characterState.id, character);
                        character.ForceMoveToTile(
                            // we should really be getting the tile id here
                            // grrr
                            ServiceLocator.Current.Get<GameBoard>().GetTileByCoordinates(characterState.coordinates.ToCoordinates())
                            // TileEntity.FromCoordinates(characterState.coordinates.ToCoordinates())
                        );
                    });
                };
            },
            (task) =>
            {
                Debug.LogError($"Failed to load character {characterState.characterClass}");
            });
        }

        private void OnCharacterRemoved(string key, CharacterState characterState)
        {
            EventBus.Publish(new ToastMessageEvent($"Player {characterState.id} has left the game!"));
            Destroy(_characters[characterState.id].gameObject);
            _characters.Remove(characterState.id);
        }


        private void LoadCharacter(
            string characterId,
            Action<UfbCharacter> onComplete,
            Action<AsyncOperationHandle<UfbCharacter>> onError)
        {
            var task = Addressables.LoadAssetAsync<UfbCharacter>("UfbCharacter/" + characterId);
            task.Completed += (obj) =>
            {
                if (obj.Status == AsyncOperationStatus.Failed)
                    onError(obj);
                else
                    onComplete(obj.Result);
            };
        }

        private void OnCharacterMoved(CharacterMovedMessage m)
        {
            var coordinates = m.path.Select(p => p.coord.ToCoordinates());
            coordinates.Reverse();
            var path = GameManager.Instance.GameBoard.GetPathFromCoordinates(coordinates);
            var task = _characters[m.playerId].MoveAlongPath(path);
            task.ContinueWith((t) =>
            {
                EventBus.Publish(new CharacterPlacedEvent(_characters[m.playerId]));
            });
        }

        private async void OnRequestCharacterMove(RequestCharacterMoveEvent e)
        {
            Debug.Log($"[PlayerManager] Requesting move to {e.Destination.ToString()}");
            await _room.Send("move", new Dictionary<string, object>() {
                { "destination", e.Destination.ToDictionary() }
            });
        }

    }

}