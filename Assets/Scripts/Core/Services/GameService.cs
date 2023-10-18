using UnityEngine;
using UFB.Core;
using UFB.Network.RoomMessageTypes;
using UFB.Events;
using UFB.Network;
using Colyseus;
using UFB.StateSchema;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System;
using NativeWebSocket;

namespace UFB.Events
{
    public class RoomReceieveMessageEvent<T>
        where T : IReceiveMessage
    {
        public T Message { get; private set; }

        public RoomReceieveMessageEvent(T message)
        {
            Message = message;
        }
    }

    public class RoomSendMessageEvent
    {
        public string MessageType { get; private set; }
        public ISendMessage Message { get; private set; }

        public RoomSendMessageEvent(string messageType, ISendMessage message)
        {
            MessageType = messageType;
            Message = message;
        }

        public static RoomSendMessageEvent Create<T>(string messageType, T message)
            where T : ISendMessage
        {
            return new RoomSendMessageEvent(messageType, message);
        }
    }

    public class GameReadyEvent
    {
        public ColyseusRoom<UfbRoomState> room;

        public GameReadyEvent(ColyseusRoom<UfbRoomState> room)
        {
            this.room = room;
        }
    }
}

namespace UFB.Core
{
    public class GameService : IService
    {
        public ColyseusRoom<UfbRoomState> Room
        {
            get => _room;
            private set
            {
                SubscribeRoomEvents(value);
                _room = value;
            }
        }

        public UfbRoomState RoomState => Room.State;

        // I don't think any individual object should have access to the full room
        // instead, the room should accept messages via the EventBus
        private ColyseusRoom<UfbRoomState> _room;

        public GameService() { }

        public async void JoinGame(
            string roomId,
            UfbRoomJoinOptions joinOptions,
            Action onJoinError = null
        )
        {
            try
            {
                var tcs = new TaskCompletionSource<bool>();
                await ServiceLocator.Current
                    .Get<NetworkService>()
                    .JoinRoom(
                        roomId,
                        joinOptions,
                        async (room) =>
                        {
                            tcs.SetResult(await LoadGame(room));
                        }
                    );
                await tcs.Task;
            }
            catch (Exception e)
            {
                Debug.Log("Error joining room: " + e.Message);
                onJoinError?.Invoke();
            }
        }

        public async void CreateGame(
            UfbRoomCreateOptions createOptions,
            UfbRoomJoinOptions joinOptions
        )
        {
            Debug.Log("CreateGame called!");
            var tcs = new TaskCompletionSource<bool>();
            await ServiceLocator.Current
                .Get<NetworkService>()
                .CreateRoom(
                    createOptions,
                    joinOptions,
                    async (room) =>
                    {
                        tcs.SetResult(await LoadGame(room));
                    }
                );
            await tcs.Task;
        }

        private async Task<bool> LoadGame(ColyseusRoom<UfbRoomState> room)
        {
            Room = room;
            var tcs = new TaskCompletionSource<bool>();
            SceneManager.LoadSceneAsync("Game").completed += (op) =>
            {
                EventBus.Publish(new GameReadyEvent(room));
                tcs.SetResult(true);
            };
            // consider having a dependency injector that searches for monobehaviours,
            // and initializes them when a message is sent
            // this could effectively signal to classes that rely on "Game" that it has loaded

            return await tcs.Task;
        }

        public void SubscribeToRoomMessage<T>(string type, Action<T> action)
            where T : IReceiveMessage
        {
            Room.OnMessage(type, action);
        }

        /// <summary>
        /// Subscribe to events that are handled by this class
        /// </summary>
        /// <param name="room"></param>
        private void SubscribeRoomEvents(ColyseusRoom<UfbRoomState> room)
        {
            EventBus.Subscribe<RoomSendMessageEvent>(OnRoomSendMessageEvent);

            room.OnMessage<NotificationMessage>(
                "notification",
                (message) =>
                {
                    Debug.Log("Received notification message! " + message.message);
                    EventBus.Publish(new RoomReceieveMessageEvent<NotificationMessage>(message));
                }
            );

            room.OnLeave += (code) =>
            {
                WebSocketCloseCode closeCode = WebSocketHelpers.ParseCloseCodeEnum(code);
                EventBus.Unsubscribe<RoomSendMessageEvent>(OnRoomSendMessageEvent);
                LoadMainMenu();
            };
        }

        private void OnRoomSendMessageEvent(RoomSendMessageEvent e)
        {
            Debug.Log(
                "Sending message to room: " + e.MessageType + " | " + e.Message.ToDetailedString()
            );
            Room.Send(e.MessageType, e.Message);
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
