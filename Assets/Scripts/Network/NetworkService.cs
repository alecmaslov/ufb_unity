using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Colyseus;
using UFB.StateSchema;
using System;
using UFB.Events;
using UFB.Network.RoomMessageTypes;
using UFB.Core;

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
        public UFB.Network.NetworkService NetworkManager { get; private set; }

        public NetworkManagerReadyEvent(UFB.Network.NetworkService networkManager)
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
    public class NetworkService : IService
    {

        public UfbApiClient ApiClient { get; private set; }
        public string ClientId { get { return ApiClient.ClientId; } }

        private ColyseusClient _colyseusClient;

        private readonly string _roomType = "ufbRoom";


        public NetworkService(UfbApiClient apiClient)
        {
            ApiClient = apiClient;
            if (!ApiClient.IsRegistered)
            {
                throw new Exception("ApiClient is not registered!");
            }
            var wsURL = ApiClient.GetUrlWithProtocol("wss://");
            _colyseusClient = new ColyseusClient(wsURL);
            EventBus.Publish(new NetworkManagerReadyEvent(this));
        }

        public static async Task<NetworkService> CreateWithConnection()
        {
            var ApiClient = new UfbApiClient("api.thig.io", 8080);
            try
            {
                await ApiClient.RegisterClient();
                return new NetworkService(ApiClient);
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
            joinRoomOptions.token = ApiClient.Token;
            joinRoomOptions.playerId = ApiClient.ClientId;
            var room = await _colyseusClient.JoinById<UfbRoomState>(
                roomId,
                new Dictionary<string, object>() {
                    { "joinOptions", joinRoomOptions.ToDictionary() }
                }
            );
            RegisterHandlers(room, onFirstStateChange);
            EventBus.Publish(new RoomJoinedEvent(room));
        }



        public async Task CreateRoom(
            UfbRoomCreateOptions createOptions,
            UfbRoomJoinOptions joinOptions,
            Action<ColyseusRoom<UfbRoomState>> onFirstStateChange)
        {
            joinOptions.token = ApiClient.Token;
            joinOptions.playerId = ApiClient.ClientId;

            var room = await _colyseusClient.Create<UfbRoomState>(
                _roomType,
                new Dictionary<string, object>() {
                    { "createOptions", createOptions.ToDictionary() },
                    { "joinOptions", joinOptions.ToDictionary() }
                }
            );

            Debug.Log($"[NetworkManager] Created room {room.RoomId} with {room.State.characters.Count} characters");
            RegisterHandlers(room, onFirstStateChange);
            EventBus.Publish(new RoomJoinedEvent(room));
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
        }
    }
}