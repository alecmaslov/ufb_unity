using UFB.Network;
using UnityEngine;
using UFB.Map;
using UFB.Player;
using UFB.Entities;
using System;
using UFB.Effects;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Threading.Tasks;
using UFB.StateSchema;

namespace UFB.Gameplay
{
    // when GameController is awakened, it uses this static class to parameterize
    // public static class GameControllerParameters
    // {
    //     public static readonly string gameId;
    // }


    // this controller can be parameterized by an external api like from javascript,
    // or be created by the menu
    // this should all be parameterized by an external api essentially, since a new player
    // might join an existing game, and they need to know the parameters for this
    public class GameController : MonoBehaviour
    {
        // uses a server connection (or an internal state manager)
        // to determine the state of the game. GameController is the 
        // main API through which either a game manager or a server 
        // connection provides commands to the game

        public static GameController Instance { get; private set; }


        public UfbRoomClient RoomClient { get; private set; }
        public UfbApiClient ApiClient { get; private set; }


        public GameBoard GameBoard { get { return _gameBoard; } }
        public PlayerManager PlayerManager { get { return _playerManager; } }


        [SerializeField] private string _mapName = "kraken";
        private GameBoard _gameBoard;
        private PlayerManager _playerManager;


        public delegate void OnConnectHandler();
        public delegate void OnGameStartHandler();
        public event OnConnectHandler OnConnect;
        public event OnGameStartHandler OnGameStart;


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

        // verifies a connection with the server
        // maybe this should be called at start?
        public async Task Initialize()
        {
            Debug.Log("Initializing game controller!");
            ApiClient = new UfbApiClient("api.thig.io", 8080);
            bool success = await ApiClient.RegisterClient();
            if (!success)
            {
                Debug.Log("Failed to register client!");
                // show some error in the interface
                return;
            }
            RoomClient = new UfbRoomClient(ApiClient);
            RoomClient.OnRoomJoined += OnRoomJoined;
            OnConnect?.Invoke();
        }

        public async Task CreateNewGame(UfbRoomOptions ufbRoomOptions)
        {           
            if (!ApiClient.IsRegistered)
            {
                Debug.LogError("Client is not registered!");
                throw new Exception("Client is not registered!");
            }


            // RoomClient.OnRoomJoined += OnRoomJoined;
            await RoomClient.CreateRoom(ufbRoomOptions);
        }

        public async Task JoinGame(string roomId)
        {
            if (!ApiClient.IsRegistered)
            {
                throw new Exception("Client is not registered!");
            }
            await RoomClient.JoinRoom(roomId);
        }

        public void LeaveGame()
        {
            RoomClient.LeaveRoom();
            SceneManager.LoadSceneAsync("MainMenu");
        }

        public void OnRoomJoined(UfbRoomState roomState)
        {
            Debug.Log("Room joined!");
            AsyncOperation op = SceneManager.LoadSceneAsync("Game");

            // we are going to have to get the roomState
            op.completed += (AsyncOperation obj) =>
            {
                Debug.Log("Scene loaded!");

                var gameBoardPrefab = Resources.Load("Prefabs/GameBoard") as GameObject;
                var gameBoardObject = Instantiate(gameBoardPrefab, Vector3.zero, Quaternion.identity);
                _gameBoard = GameObjectExtensions.GetOrAddComponent<GameBoard>(gameBoardObject);

                var playerManagerPrefab = Resources.Load("Prefabs/PlayerManager") as GameObject;
                var playerManagerObject = Instantiate(playerManagerPrefab, Vector3.zero, Quaternion.identity);
                _playerManager = GameObjectExtensions.GetOrAddComponent<PlayerManager>(playerManagerObject);

                Debug.Log("Spawning board! " + roomState.map + " | PLAYERS " + roomState.players.Count + " | " + roomState.players.Keys.ToString());
                Debug.Log("RoomState map NAME: " +  roomState.map.name);
                // _gameBoard.SpawnBoard(roomState.map.name);
                _gameBoard.SpawnBoard("kraken");
                _gameBoard.SpawnEntitiesRandom("chest", 20);

                OnGameStart?.Invoke();
            };
        }
    }

}