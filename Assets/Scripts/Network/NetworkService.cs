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
using System.Net;

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

    public class NetworkServiceStatusEvent
    {
        public Network.NetworkService.NetworkServiceStatus Status { get; private set; }

        public NetworkServiceStatusEvent(Network.NetworkService.NetworkServiceStatus status)
        {
            Status = status;
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
}

namespace UFB.Network
{
    public class NetworkService : IService
    {
        public enum NetworkServiceStatus
        {
            Initializing,
            Ready,
            Failed
        }

        public NetworkServiceStatus Status { get; private set; } =
            NetworkServiceStatus.Initializing;

        public UfbApiClient ApiClient { get; private set; }
        public string ClientId
        {
            get { return ApiClient.ClientId; }
        }

        private ColyseusClient _colyseusClient;

        private readonly string _roomType = "ufbRoom";

        public NetworkService(UfbApiClient apiClient)
        {
            ApiClient = apiClient;
            _colyseusClient = new ColyseusClient(ApiClient.GetUrlWithProtocol(GlobalDefine.isHttps? GlobalDefine.WebSocket_https_Header : GlobalDefine.WebSocket_http_Header));
        }

        public async Task Connect(string userId)
        {
            try
            {
                await ApiClient.RegisterClient(userId);
                Status = NetworkServiceStatus.Ready;
                EventBus.Publish(new NetworkServiceStatusEvent(Status));
            }
            catch (Exception ex)
            {
                if(ex is WebException)
                {
                    Debug.Log($"{ex.Message}");
                }

                Status = NetworkServiceStatus.Failed;
                EventBus.Publish(new NetworkServiceStatusEvent(Status));
            }
        }

        public async Task ReConnect(ReconnectionToken reconnectionToken, Action<ColyseusRoom<UfbRoomState>> onFirstStateChange)
        {
            var room = await _colyseusClient.Reconnect<UfbRoomState>(reconnectionToken);
            RegisterHandlers(room, onFirstStateChange);
            EventBus.Publish(new RoomJoinedEvent(room));
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
                new Dictionary<string, object>()
                {
                    { "joinOptions", joinRoomOptions.ConvertToDictionary() }
                }
            );
            RegisterHandlers(room, onFirstStateChange);
            EventBus.Publish(new RoomJoinedEvent(room));
        }

        public async Task CreateRoom(
            UfbRoomCreateOptions createOptions,
            UfbRoomJoinOptions joinOptions,
            Action<ColyseusRoom<UfbRoomState>> onFirstStateChange
        )
        {
            joinOptions.token = ApiClient.Token;
            joinOptions.playerId = ApiClient.ClientId;

            var room = await _colyseusClient.Create<UfbRoomState>(
                _roomType,
                new Dictionary<string, object>()
                {
                    { "createOptions", createOptions.ConvertToDictionary() },
                    { "joinOptions", joinOptions.ConvertToDictionary() }
                }
            );

            Debug.Log(
                $"[NetworkManager] Created room {room.RoomId} with {room.State.characters.Count} characters"
            );
            RegisterHandlers(room, onFirstStateChange);
            EventBus.Publish(new RoomJoinedEvent(room));
        }

        private void RegisterHandlers(
            ColyseusRoom<UfbRoomState> room,
            Action<ColyseusRoom<UfbRoomState>> onFirstStateChange
        )
        {
            ColyseusRoom<UfbRoomState>.RoomOnStateChangeEventHandler onFirstStateChangeWrapper =
                null;
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

        public async Task<UfbApiClient.RegisterUserResponse> SignUpHandler(string email, string password)
        {
            UfbApiClient.RegisterUserResponse result = await ApiClient.SignUpHandler(email, password);
            return result;
        }
        
        public async Task<UfbApiClient.RegisterUserResponse> LoginHandler(string email, string password)
        {
            UfbApiClient.RegisterUserResponse result = await ApiClient.LoginHandler(email, password);
            return result;
        }
    }
}