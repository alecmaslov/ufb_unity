using UFB.Network;
using UFB.Network.RoomMessageTypes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UFB.Core
{
    public static class Bootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static async void Initiailze()
        {
            ServiceLocator.Initiailze();

            var networkService = new NetworkService(new UfbApiClient(GlobalDefine.API_URL, GlobalDefine.API_PORT, GlobalDefine.isHttps));
            var spawnerService = SpawnerService.Instance;
            var gameService = new GameService();

            ServiceLocator.Current.Register(networkService);
            ServiceLocator.Current.Register(spawnerService);
            ServiceLocator.Current.Register(gameService);


            return;
            // we must wait until we are connected to try and perform any other actions
            //await networkService.Connect();

            var currentScene = SceneManager.GetActiveScene();

#if UNITY_EDITOR
            if (currentScene.name == "Game")
            {
                var joinOptions = new UfbRoomJoinOptions();
                joinOptions.characterClass = "ophaia";
                gameService.CreateGame(new UfbRoomCreateOptions(), joinOptions);
            }
#endif
        }
    }
}
