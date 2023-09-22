using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Colyseus;
using UFB.StateSchema;
using System;
using UFB.Events;

namespace UFB.Events
{
    public class RoomJoinedEvent
    {
        public ColyseusRoom<UfbRoomState> RoomState { get; private set; }

        public RoomJoinedEvent(ColyseusRoom<UfbRoomState> roomState)
        {
            RoomState = roomState;
        }
    }

    public class NetworkManagerReadyEvent
    {
        public UFB.Network.NetworkManager NetworkManager { get; private set; }

        public NetworkManagerReadyEvent(UFB.Network.NetworkManager networkManager)
        {
            NetworkManager = networkManager;
        }
    }

    public class RoomLeftEvent
    {
        public int Code { get; private set; }

        public RoomLeftEvent(int code)
        {
            Code = code;
        }
    }


    public class RoomErrorEvent
    {
        public int Code { get; private set; }
        public string Message { get; private set; }

        public RoomErrorEvent(int code, string message)
        {
            Code = code;
            Message = message;
        }
    }

    public class RoomNotificationEvent
    {

    }

    public class NetworkManagerDisconnectedEvent
    {
        public int Code { get; private set; }
        public string Message { get; private set; }

        public NetworkManagerDisconnectedEvent(int code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}

namespace UFB.Network
{
    public class NetworkManager
    {

        public static NetworkManager Instance;

        public UfbApiClient ApiClient { get; private set; }
        public ColyseusRoom<UfbRoomState> Room { get; private set; }
        public string ClientId { get { return ApiClient.ClientId; } }

        private ColyseusClient _colyseusClient;

        private readonly string _roomType = "ufbRoom";


        public NetworkManager(UfbApiClient apiClient)
        {
            ApiClient = apiClient;
            if (!ApiClient.IsRegistered)
            {
                throw new Exception("ApiClient is not registered!");
            }
            var wsURL = ApiClient.GetUrlWithProtocol("wss://");
            _colyseusClient = new ColyseusClient(wsURL);
            Instance = this;
            EventBus.Publish(new NetworkManagerReadyEvent(this));
        }

        public static async Task<NetworkManager> CreateWithConnection()
        {
            var ApiClient = new UfbApiClient("api.thig.io", 8080);
            try
            {
                await ApiClient.RegisterClient();
                return new NetworkManager(ApiClient);
            }
            catch (Exception e)
            {
                EventBus.Publish(new NetworkManagerDisconnectedEvent(-1, "Failed to connect to host"));
                throw e;
            }
        }

        public async Task JoinRoom(
            string roomId,
            UfbRoomJoinOptions joinRoomOptions,
            Action<ColyseusRoom<UfbRoomState>> onFirstStateChange
        )
        {
            if (Room != null)
            {
                throw new Exception("Tried to join a room while already in a room!");
            }
            joinRoomOptions.token = ApiClient.Token;
            joinRoomOptions.playerId = ApiClient.ClientId;
            Room = await _colyseusClient.JoinById<UfbRoomState>(
                roomId,
                new Dictionary<string, object>() {
                    { "joinOptions", joinRoomOptions.ToDictionary() }
                }
            );
            RegisterHandlers(Room, onFirstStateChange);
            EventBus.Publish(new RoomJoinedEvent(Room));
        }



        public async Task CreateRoom(
            UfbRoomCreateOptions createOptions,
            UfbRoomJoinOptions joinOptions,
            Action<ColyseusRoom<UfbRoomState>> onFirstStateChange)
        {
            if (Room != null)
            {
                throw new Exception("Tried to join a room while already in a room!");
            }

            joinOptions.token = ApiClient.Token;
            joinOptions.playerId = ApiClient.ClientId;

            Room = await _colyseusClient.Create<UfbRoomState>(
                _roomType,
                new Dictionary<string, object>() {
                    { "createOptions", createOptions.ToDictionary() },
                    { "joinOptions", joinOptions.ToDictionary() }
                }
            );
            RegisterHandlers(Room, onFirstStateChange);
            EventBus.Publish(new RoomJoinedEvent(Room));
        }


        public void LeaveRoom()
        {
            if (Room != null)
            {
                Room.Leave();
                Room = null;
            }
            else
            {
                Debug.LogWarning("[NetworkManager] Tried to leave room while not in a room!");
            }
        }

        // idea - have a public method that allows others to subscribe to events on a certain message type

        public void AddMessageHandler<T>(string messageType, Action<T> handler)
        {
            Room.OnMessage<T>(messageType, handler);
        }


        private void RegisterHandlers(
            ColyseusRoom<UfbRoomState> room,
            Action<ColyseusRoom<UfbRoomState>> onFirstStateChange)
        {
            ColyseusRoom<UfbRoomState>.RoomOnStateChangeEventHandler onFirstStateChangeWrapper = null;
            onFirstStateChangeWrapper = (UfbRoomState state, bool isFirstState) =>
            {
                if (isFirstState)
                {
                    // Invoke the user-provided callback then remove the handler
                    onFirstStateChange.Invoke(room);
                    room.OnStateChange -= onFirstStateChangeWrapper;
                }
            };
            room.OnStateChange += onFirstStateChangeWrapper;
            room.OnLeave += OnRoomLeave;
        }


        private void OnRoomLeave(int code)
        {
            Debug.Log($"[NetworkManager] Room left! Code: {code}");
            // switch the codes here to determine whether to delete the Room or not
            // in some cases, if the user locks their phone or something, we don't want
            // to trigger things that ultimately would unload the scene and stuff 

            EventBus.Publish(new RoomLeftEvent(code));
        }

        private void OnRoomError(int code, string message)
        {
            Debug.Log($"[NetworkManager] Room error! Code: {code} | Message: {message}");
        }

    }
}