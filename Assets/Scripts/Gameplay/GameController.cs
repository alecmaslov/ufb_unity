using UFB.Network;
using UnityEngine;
using UFB.Map;
using UFB.Player;
using UFB.Entities;

namespace UFB.Gameplay {

    public class GameController : MonoBehaviour {
        // uses a server connection (or an internal state manager)
        // to determine the state of the game. GameController is the 
        // main API through which either a game manager or a server 
        // connection provides commands to the game


        public static GameController Instance { get; private set; }
        public ServerConnection ServerConnection { get; private set; }
        public GameBoard GameBoard { get { return _gameBoard; } }

        [SerializeField] private string _mapName = "kraken";
        [SerializeField] private GameBoard _gameBoard;
        private PlayerManager _playerManager;

        private void Awake() {
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (_gameBoard == null) {
                var gameBoardPrefab = Resources.Load("Prefabs/GameBoard") as GameObject;
                var gameBoardObject = Instantiate(gameBoardPrefab, Vector3.zero, Quaternion.identity);
                _gameBoard = GameObjectExtensions.GetOrAddComponent<GameBoard>(gameBoardObject);
            }

            var playerManagerPrefab = Resources.Load("Prefabs/PlayerManager") as GameObject;
            var playerManagerObject = Instantiate(playerManagerPrefab, Vector3.zero, Quaternion.identity);
            _playerManager = GameObjectExtensions.GetOrAddComponent<PlayerManager>(playerManagerObject);

            _gameBoard.SpawnBoard(_mapName);
        }

        public void InitializeGame()
        {
            // this is where we would initialize the game
            // and set up the game state
        }
    }

}