using UFB.Network;
using UnityEngine;

namespace UFB.Gameplay {

    public class GameController : MonoBehaviour {
        // uses a server connection (or an internal state manager)
        // to determine the state of the game. GameController is the 
        // main API through which either a game manager or a server 
        // connection provides commands to the game

        public static GameController Instance { get; private set; }
        public ServerConnection ServerConnection { get; private set; }

        private void Awake() {
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void InitializeGame()
        {
            // this is where we would initialize the game
            // and set up the game state
        }
    }

}