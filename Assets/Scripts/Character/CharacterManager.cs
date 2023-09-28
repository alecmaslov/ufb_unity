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
using UFB.Player;
using System;
using UFB.Map;
using UFB.Network.RoomMessageTypes;
using UFB.Core;
using System.Threading.Tasks;
using Colyseus.Schema;

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

    public class SelectedCharacterEvent
    {
        public UfbCharacter character;
        public CharacterState state;
        public bool isPlayerControlled; // lets us set zombie
    }
}

namespace UFB.Character
{
    // will need to be used even before game scene starts, since main menu
    // will require access to characters
    public class CharacterManager : MonoBehaviourService
    {
        public CharacterController PlayerCharacter => _characters[_characterId];

        // public MapSchema<CharacterState> State { get; private set; }

        [SerializeField]
        private AssetReference _characterPrefab;

        private Dictionary<string, CharacterController> _characters =
            new Dictionary<string, CharacterController>();

        private string _characterId;
        private string _playerId; // during zombie mode, we can set this to a different player

        private void OnEnable()
        {
            ServiceLocator.Current.Register(this);

            var gameService = ServiceLocator.Current.Get<GameService>();
            var clientId = ServiceLocator.Current.Get<NetworkService>().ClientId;

            if (gameService.Room == null)
            {
                Debug.LogError("Room is null");
                return;
            }

            _characterId = gameService.RoomState.playerCharacters[clientId]; // we have to translate this to characterId

            gameService.RoomState.characters.OnAdd(OnCharacterAdded);
            gameService.RoomState.characters.OnRemove(OnCharacterRemoved);

            gameService.SubscribeToRoomMessage<CharacterMovedMessage>(
                "characterMoved",
                OnCharacterMoved
            );

            gameService.SubscribeToRoomMessage<BecomeZombieMessage>(
                "becomeZombie",
                (message) =>
                {
                    _playerId = message.playerId;
                }
            );

            _characters = new Dictionary<string, CharacterController>();

            // message can be scoped on the server to send only to specific client
            // EventBus.Subscribe<RequestCharacterMoveEvent>(OnRequestCharacterMove);

            // for now, do this for default behavior, but eventually this will be triggered by some UI handler
            EventBus.Subscribe<TileClickedEvent>(
                (e) =>
                {
                    EventBus.Publish(new RequestCharacterMoveEvent(e.tile.Coordinates));
                }
            );
        }

        private void OnDisable()
        {
            // EventBus.Unsubscribe<RequestCharacterMoveEvent>(OnRequestCharacterMove);

            ServiceLocator.Current.Unregister<CharacterManager>();
        }

        private async void OnCharacterAdded(string key, CharacterState characterState)
        {
            EventBus.Publish(
                new ToastMessageEvent($"Player {characterState.id} has joined the game!")
            );
            Debug.Log($"[CharacterManager] Player {characterState.id} has joined the game!");

            try
            {
                UfbCharacter ufbCharacter = await LoadCharacter(characterState.characterClass);
                GameObject characterObject = await _characterPrefab
                    .InstantiateAsync(transform)
                    .Task;

                var character = characterObject.GetComponent<CharacterController>();

                // if it's an NPC, don't play the intro
                await character.Initialize(ufbCharacter, characterState, true);
                _characters.Add(characterState.id, character);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private void OnCharacterRemoved(string key, CharacterState characterState)
        {
            EventBus.Publish(
                new ToastMessageEvent($"Player {characterState.id} has left the game!")
            );
            Destroy(_characters[characterState.id].gameObject);
            _characters.Remove(characterState.id);
        }

        private async Task<UfbCharacter> LoadCharacter(string characterId)
        {
            var task = Addressables.LoadAssetAsync<UfbCharacter>("UfbCharacter/" + characterId);
            await task.Task;
            if (task.Status == AsyncOperationStatus.Failed)
                throw new Exception(
                    $"Failed to load character {characterId}: {task.OperationException.Message}"
                );
            return task.Result;
        }

        private void OnCharacterMoved(CharacterMovedMessage m)
        {
            // var coordinates = m.path.Select(p => p.coord.ToCoordinates());
            var tileIds = m.path.Select(p => p.tileId);
            tileIds.Reverse();
            /// select all tiles from tiles using tileIds

            var path = ServiceLocator.Current.Get<GameBoard>().GetTilesByIds(tileIds);

            Debug.Log($"[CharacterManager] Moving character {m.characterId} along path");
            var task = _characters[m.characterId].MoveAlongPath(path);

            task.ContinueWith(
                (t) =>
                {
                    EventBus.Publish(new CharacterPlacedEvent(_characters[m.characterId]));
                }
            );
        }

        // private async void OnRequestCharacterMove(RequestCharacterMoveEvent e)
        // {
        //     Debug.Log($"[PlayerManager] Requesting move to {e.Destination.ToString()}");
        //     await _room.Send(
        //         "move",
        //         new Dictionary<string, object>() { { "destination", e.Destination.ToDictionary() } }
        //     );
        // }

        // public void SavePlayerConfiguration(string fileName)
        // {

        //     // this will eventually be handled by the Player object, which PlayerEntity has a reference
        //     // to. It will handle loading/unloading the JSON into the player state. For now, quick solution
        //     var json = JsonConvert.SerializeObject(_players.Select(p => new PlayerConfiguration
        //     {
        //         CharacterName = p.CharacterName,
        //         TileCoordinates = p.CurrentTile.Coordinates
        //     }).ToList());

        //     // save the json
        //     ApplicationData.SaveJSON(json, "gamestate/player-config", fileName + ".json");
        // }


        //         public void LoadPlayerConfiguration(string fileName)
        // {
        //     // load the json
        //     var playerConfigurations = ApplicationData.LoadJSON<List<PlayerConfiguration>>("gamestate/player-config", fileName + ".json");

        //     // iterate through the players and set their tile coordinates
        //     foreach (var playerConfiguration in playerConfigurations)
        //     {
        //         var player = _players.FirstOrDefault(p => p.CharacterName == playerConfiguration.CharacterName);
        //         if (player == null)
        //         {
        //             Debug.LogError($"Player with character name {playerConfiguration.CharacterName} not found");
        //             continue;
        //         }
        //         var tile = GameManager.Instance.GameBoard.GetTileByCoordinates(playerConfiguration.TileCoordinates);
        //         if (tile == null)
        //         {
        //             Debug.LogError($"Tile with coordinates {playerConfiguration.TileCoordinates} not found");
        //             continue;
        //         }
        //         player.ForceMoveToTile(tile);
        //     }
        // }
    }
}
