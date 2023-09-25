using UnityEngine;
using UFB.Core;
using UFB.Network.RoomMessageTypes;
using UFB.Events;
using UFB.Network;
using Colyseus;
using UFB.StateSchema;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

namespace UFB.Events
{
    // public class LeaveGameEvent
    // {
    //     public LeaveGameEvent()
    //     {

    //     }
    // }

    // public class LoadGameEvent
    // {
    //     public LoadGameEvent()
    //     {

    //     }
    // }


    public class RoomReceieveMessageEvent<T> where T : IReceiveMessage
    {
        public T Message { get; private set; }

        public RoomReceieveMessageEvent(T message)
        {
            Message = message;
        }
    }
}

namespace UFB.Core
{
    public class GameService : IService
    {

        public ColyseusRoom<UfbRoomState> Room
        {
            get => _room; private set
            {
                SubscribeRoomEvents(value);
                _room = value;
            }
        }

        private ColyseusRoom<UfbRoomState> _room;


        public GameService()
        {
        }


        public async void JoinGame(string roomId, UfbRoomJoinOptions joinOptions)
        {
            Debug.Log("JoinGame called!");
            var tcs = new TaskCompletionSource<bool>();
            await ServiceLocator.Current.Get<NetworkService>().JoinRoom(roomId, joinOptions, async (room) =>
            {
                tcs.SetResult(await LoadGame(room));
            });
            await tcs.Task;
        }

        public async void CreateGame(UfbRoomCreateOptions createOptions, UfbRoomJoinOptions joinOptions)
        {
            Debug.Log("CreateGame called!");
            var tcs = new TaskCompletionSource<bool>();
            await ServiceLocator.Current.Get<NetworkService>().CreateRoom(createOptions, joinOptions, async (room) =>
            {
                tcs.SetResult(await LoadGame(room));
            });
            await tcs.Task;
        }

        private async Task<bool> LoadGame(ColyseusRoom<UfbRoomState> room)
        {
            Room = room;
            var tcs = new TaskCompletionSource<bool>();
            SceneManager.LoadSceneAsync("Game").completed += (op) =>
            {

                tcs.SetResult(true);
            };
            // consider having a dependency injector that searches for monobehaviours,
            // and initializes them when a message is sent
            // this could effectively signal to classes that rely on "Game" that it has loaded

            return await tcs.Task;

        }

        private void SubscribeRoomEvents(ColyseusRoom<UfbRoomState> room)
        {
            room.OnMessage<NotificationMessage>("notification", (message) =>
            {
                EventBus.Publish(new RoomReceieveMessageEvent<NotificationMessage>(message));
            });
            room.OnLeave += (code) =>
            {
                LoadMainMenu();
            };
        }

        public void LeaveGame()
        {
            Room.Leave(true); // this will trigger the room.OnLeave
        }


        private void LoadMainMenu()
        {
            SceneManager.LoadSceneAsync("MainMenu");
        }
    }

}