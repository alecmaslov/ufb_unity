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
using Unity.VisualScripting;
using UFB.UI;
using Colyseus;
using Unity;
using UnityEngine.AddressableAssets;

namespace UFB.Gameplay
{
    public class GameManager : MonoBehaviour
    {

        public static GameManager Instance { get; private set; }

        public PlayerManager PlayerManager { get; private set; }
        public NetworkManager NetworkManager { get; private set; }
        public GameBoard GameBoard { get; private set; }
        public UIManager UIManager { get; private set; }

        public delegate void OnGameLoadedHandler();
        public event OnGameLoadedHandler OnGameLoaded;

        public AssetReference gameBoardPrefab;
        public AssetReference playerManagerPrefab;

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
            UIManager = new UIManager(this);
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
            Debug.Log($"[GameManager] Room joined, loading game...");
            AsyncOperation op = SceneManager.LoadSceneAsync("Game");
            op.completed += (AsyncOperation obj) =>
            {
                Debug.Log($"Scene loaded with {room.State.players.Count} players!");
                OnGameLoaded?.Invoke();

                Addressables.InstantiateAsync(gameBoardPrefab).Completed += (obj) =>
                {
                    GameBoard = GameObjectExtensions.GetOrAddComponent<GameBoard>(obj.Result);
                    GameBoard.Initialize(room.State.map);
                    // GameBoard.SpawnEntitiesRandom("chest", 20);
                    Debug.Log($"GameBoard loaded");

                    // we have to wait for the gameboard first, then we can create the PlayerManager
                    Addressables.InstantiateAsync(playerManagerPrefab).Completed += (obj) =>
                    {
                        PlayerManager = GameObjectExtensions.GetOrAddComponent<PlayerManager>(obj.Result);
                        PlayerManager.Initialize(room, NetworkManager.ClientId);
                        Debug.Log($"PlayerManager loaded");
                        PlayerManager.MyPlayer.FocusCamera();
                    };
                };


            };
        }



        public void TileClicked(TileEntity tile)
        {
            Debug.Log($"Tile clicked: {tile.Coordinates}");
            PlayerManager.MyPlayer.ForceMoveToTileAnimate(tile);
        }
    }

}