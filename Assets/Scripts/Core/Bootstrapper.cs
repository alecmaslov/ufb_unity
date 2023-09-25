using UFB.Network;
using UFB.Network.RoomMessageTypes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UFB.Core
{
    public static class Bootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static async void Initiailze()
        {   
            var currentScene = SceneManager.GetActiveScene();
// #if UNITY_EDITOR
//             if (currentScene.name != "Game" || currentScene.name != "MainMenu")
//                 return;
// #endif

            ServiceLocator.Initiailze();

            var networkService = await NetworkService.CreateWithConnection();
            var spawnerService = SpawnerService.Instance;
            var gameService = new GameService();

            ServiceLocator.Current.Register(networkService);
            ServiceLocator.Current.Register(spawnerService);
            ServiceLocator.Current.Register(gameService);

            // Application is ready to start, load your main scene.
            // SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
#if UNITY_EDITOR
            if (currentScene.name == "Game")
            {
                // this will cause the game to load with a new created game
                var joinOptions = new UfbRoomJoinOptions();
                joinOptions.characterClass = "ophaia";
                gameService.CreateGame(new UfbRoomCreateOptions(), joinOptions);
            }
#endif
        }
    }
}
