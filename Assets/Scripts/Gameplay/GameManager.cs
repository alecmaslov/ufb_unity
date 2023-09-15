using UFB.Network;
using UnityEngine;
using UFB.Player;
using UFB.Entities;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UFB.StateSchema;
using Colyseus;
using UnityEngine.AddressableAssets;
using UFB.Events;

namespace UFB.Gameplay
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public PlayerManager PlayerManager { get; private set; }
        public NetworkManager NetworkManager { get; private set; }
        public GameBoard GameBoard { get; private set; }

        public delegate void OnGameLoadedHandler();
        public event OnGameLoadedHandler OnGameLoaded;

        public AssetReference gameBoardPrefab;
        public AssetReference playerManagerPrefab;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            // if the current scene is not the main menu, then we need to load it
            var currentScene = SceneManager.GetActiveScene();
            if (currentScene.name != "MainMenu")
            {
                SceneManager.LoadSceneAsync("MainMenu");
            }
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private async void Start()
        {
            NetworkManager = await NetworkManager.CreateWithConnection();
        }

        public async Task CreateNewGame(UfbRoomCreateOptions createOptions, UfbRoomJoinOptions joinOptions)
        {
            await NetworkManager.CreateRoom(createOptions, joinOptions, LoadGame);
        }

        public async Task JoinGame(string roomId, UfbRoomJoinOptions joinOptions)
        {
            await NetworkManager.JoinRoom(roomId, joinOptions, LoadGame);
        }

        public void LeaveGame()
        {
            Debug.Log($"[GameManager] Leaving current game for MainMenu");
            NetworkManager.LeaveRoom();
            SceneManager.LoadSceneAsync("MainMenu");
        }

        private void LoadGame(ColyseusRoom<UfbRoomState> room)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync("Game");
            op.completed += (AsyncOperation obj) =>
            {
                EventBus.Publish(new ToastMessageEvent($"Joined room {room.RoomId}"));
                SubscribeRoomEvents(room);
                OnGameLoaded?.Invoke();

                Addressables.InstantiateAsync(gameBoardPrefab).Completed += (obj) =>
                {
                    GameBoard = GameObjectExtensions.GetOrAddComponent<GameBoard>(obj.Result);
                    GameBoard.Initialize(room.State.map);
                    // GameBoard.SpawnEntitiesRandom("chest", 20); // this will happen inside gameBoard
                
                    // we have to wait for the gameboard first, then we can create the PlayerManager
                    Addressables.InstantiateAsync(playerManagerPrefab).Completed += (obj) =>
                    {
                        PlayerManager = GameObjectExtensions.GetOrAddComponent<PlayerManager>(obj.Result);
                        PlayerManager.Initialize(room, NetworkManager.ClientId);
                        PlayerManager.MyPlayer.FocusCamera();
                    };
                };


            };
        }

        private void SubscribeRoomEvents(ColyseusRoom<UfbRoomState> room)
        {
            room.OnMessage<NotificationMessage>("notification", (message) =>
            {
                EventBus.Publish(new ToastMessageEvent(message.message));
            });

            room.OnLeave += (code) =>
            {
                EventBus.Publish(new ToastMessageEvent("You have been left the room."));
                LeaveGame();
            };
        }



        public void TileClicked(TileEntity tile)
        {
            Debug.Log($"Tile clicked: {tile.Coordinates}");
            PlayerManager.MyPlayer.ForceMoveToTileAnimate(tile);
        }
    }

}