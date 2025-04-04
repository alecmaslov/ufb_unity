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
using System;
using System.Collections;
using UFB.Map;
using UFB.Network.RoomMessageTypes;
using UFB.Core;
using System.Threading.Tasks;
using Colyseus.Schema;
using UFB.Interactions;
using UFB.Items;

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
        public Character.CharacterController controller;
        public bool isPlayer = false; // lets us set zombie
    }

    public class ChangeCharacterStateEvent
    {
        public CharacterState state;
        public bool isPlayer = false; // lets us set zombie
    }

    // public class CharacterStatsChangedEvent
    // {
    //     public CharacterState state;

    //     public CharacterStatsChangedEvent(CharacterState state)
    //     {
    //         this.state = state;
    //     }
    // }
}

namespace UFB.Character
{
    // will need to be used even before game scene starts, since main menu
    // will require access to characters
    public class CharacterManager : MonoBehaviourService
    {
        public static CharacterManager Instance { get; private set; }
        public CharacterController PlayerCharacter => _characters[_playerCharacterId];
        public CharacterController SelectedCharacter => _characters[_selectedCharacterId];

        // public MapSchema<CharacterState> State { get; private set; }
        public List<string> monsterKeys = new List<string>();

        [SerializeField]
        private GameObject _characterPrefab;

        [SerializeField]
        private MovePanel movePanel;

        [SerializeField]
        private SpawnPanel spawnPanel;


        private Dictionary<string, CharacterController> _characters =
            new Dictionary<string, CharacterController>();

        private string _playerCharacterId;
        private string _selectedCharacterId; // during zombie mode, we can set this to a different player

        private MapSchema<CharacterState> _characterStates;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
        }

        private void Start()
        {
            if (PlayerPrefs.GetInt("roomJoinOption") == 1)
            {
                StartCoroutine(GetRoomDataFromServer());
            }
        }

        private void OnEnable()
        {
            ServiceLocator.Current.Register(this);

            var gameService = ServiceLocator.Current.Get<GameService>();
            _playerCharacterId = ServiceLocator.Current.Get<NetworkService>().ClientId;
            // _selectedCharacterId = _playerCharacterId;

            if (gameService.Room == null)
            {
                Debug.LogError("Room is null");
                return;
            }

            _characters = new Dictionary<string, CharacterController>();
            _characterStates = gameService.RoomState.characters;

            _characterStates.OnAdd(OnCharacterAdded);
            _characterStates.OnRemove(OnCharacterRemoved);

            gameService.SubscribeToRoomMessage<CharacterMovedMessage>(
                GlobalDefine.SERVER_MESSAGE.CHARACTER_MOVED,
                OnCharacterMoved
            );

            gameService.SubscribeToRoomMessage<BecomeZombieMessage>(
                "becomeZombie",
                (message) =>
                {
                    _selectedCharacterId = message.playerId;
                }
            );

            /*_characterStates.ForEach(
                (key, character) =>
                {
                    character.stats.OnChange(() =>
                    {
                        Debug.Log($"key: {key}, coin: {character.stats.coin}");
                        EventBus.Publish(new ChangeCharacterStateEvent
                        {
                            state = character,
                            isPlayer = character.id == _selectedCharacterId
                        });
                    });
                }
            );*/

            _characterStates.OnChange(
                (newState, oldState) => {
                    /// subscribe to changes in player stats
                }
            );

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

        private void SetSelectedCharacter(string characterId)
        {
            _selectedCharacterId = characterId;

            CharacterController character = _characters[characterId];

            movePanel.character = character;
            spawnPanel.character = character;
            if (InteractionManager.Instance.isSpawn && character.State.type == (int)USER_TYPE.USER && PlayerPrefs.GetInt("roomJoinOption") != 1)
            {
                character.transform.position = new Vector3(-100, -100, 100);
            }
            /*EventBus.Publish(
                new SetCameraPresetStateEvent
                {
                    presetState = CameraController.PresetState.TopDown
                }
            );*/

            // now it's up to any listeners to register events with these
            EventBus.Publish(
                new SelectedCharacterEvent
                {
                    controller = _characters[characterId],
                    isPlayer = characterId == _playerCharacterId
                }
            );

            EventBus.Publish(
                new ChangeCharacterStateEvent
                {
                    state = character.State,
                    isPlayer = true
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
            Debug.Log($"[CharacterManager] Player {characterState.id} has joined the game! class : {characterState.characterClass}, type : class : {characterState.type}");

            try
            {
                if(characterState.type == (int)USER_TYPE.MONSTER)
                {
                    monsterKeys.Add(characterState.id);
                }

                UfbCharacter ufbCharacter = await LoadCharacter(characterState.characterClass);
                GameObject templateCharacter = Instantiate(_characterPrefab, transform);
                var character = templateCharacter.GetComponent<CharacterController>();

                // if it's an NPC, don't play the intro
                await character.Initialize(ufbCharacter, characterState, true);
                _characters.Add(characterState.id, character);

                character.transform.localEulerAngles = new Vector3(0, 180, 0);

                if (character.Id == _playerCharacterId)
                {
                    SetSelectedCharacter(character.Id);
                } else
                {
                    //character.gameObject.SetActive(false);
                }
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
            try
            {
                var task = Addressables.LoadAssetAsync<UfbCharacter>("UfbCharacter/" + characterId);
                EventBus.Publish(new DownloadProgressEvent(task, $"Character {characterId}"));
                await task.Task;

                if (task.Status == AsyncOperationStatus.Failed)
                    throw new Exception(
                        $"Failed to load character {characterId}: {task.OperationException.Message}"
                    );
                return task.Result;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw new Exception($"Failed to load character {characterId}: {e.Message}");
            }
        }

        private void OnCharacterMoved(CharacterMovedMessage m)
        {
            // movePanel.OnCharacterMoved(m);
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

        public void OnSetCharacterTilePosition(SetCharacterPositionMessage m)
        {
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

        public void OnSelectCharacter(string key)
        {
            if(_selectedCharacterId == key)
            {
                key = _playerCharacterId;
            }
            SetSelectedCharacter(key);

            CameraManager.instance.SetTarget(_characters[key].transform);
        }

        public CharacterController GetCharacterFromId(string id)
        {
            if(_characters.ContainsKey(id))
            {
                return _characters[id];
            } 
            else
            {
                return null;
            }
        }

        public Dictionary<string, CharacterController> GetCharacterList()
        {
            return _characters; 
        }

        IEnumerator GetRoomDataFromServer()
        {
            yield return new WaitForSeconds(1);
            
            EventBus.Publish(
                RoomSendMessageEvent.Create(
                    GlobalDefine.CLIENT_MESSAGE.GET_ROOM_DATA,
                    new RequestCharacterId
                    {
                        characterId = _playerCharacterId,
                    }
                )
            );
            
        }
    }
}
