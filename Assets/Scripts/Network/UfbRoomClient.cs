using System;
using Colyseus;
using Colyseus.Schema;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UFB.Player;
using UFB.Map;
using UFB.Gameplay;
using UFB.Entities;
using System.Threading.Tasks;
using UFB.StateSchema;
using Unity.VisualScripting;
using UFB.Network.RoomMessageTypes;
using UFB.Core;

namespace UFB.Network
{
    // public class UfbRoomRules
    // {
    //     public int maxPlayers = 4;
    //     public int initHealth = 100;
    //     public int initEnergy = 100;
    //     public float turnTime = 60f*3f;
    // }


    // public class UfbRoomCreateOptions
    // {
    //     public string mapName = "kraken";
    //     public UfbRoomRules rules = new UfbRoomRules();

    //     public Dictionary<string, object> ToDictionary()
    //     {
    //         return new Dictionary<string, object> {
    //             { "mapName", mapName },
    //             { "rules", rules }
    //         };
    //     }
    // }

    // public class UfbRoomJoinOptions
    // {
    //     public string token;
    //     public string playerId;
    //     public string displayName;
    //     public string characterId;
    //     public string characterClass;

    //     public Dictionary<string, object> ToDictionary()
    //     {
    //         return new Dictionary<string, object> {
    //             { "token", token },
    //             { "playerId", playerId },
    //             { "displayName", displayName },
    //             { "characterId", characterId },
    //             { "characterClass", characterClass }
    //         };
    //     }
    // }

    // public class UfbRoomOptions
    // {
    //     public UfbRoomCreateOptions createOptions;
    //     public UfbRoomJoinOptions joinOptions;
    // }





    /// Look into using ColyseusManager
    public class UfbRoomClient
    {
        public GameManager gameController;

        public delegate void OnClientInitializedHander();
        public delegate void OnRoomJoinedHandler(UfbRoomState roomState);
        public delegate void OnRoomLeftHandler();
        public delegate void OnPlayerJoinedHandler(string playerId);
        public delegate void OnNotificationMessageHandler(NotificationMessage message);

        public event OnClientInitializedHander OnClientInitialized;
        public event OnRoomJoinedHandler OnRoomJoined;
        public event OnRoomLeftHandler OnRoomLeft;
        public event OnPlayerJoinedHandler OnPlayerJoined;
        public event OnNotificationMessageHandler OnNotificationMessage;

        public ColyseusRoom<UfbRoomState> Room { get { return _room; } }
        public UfbRoomState State { get { return _room.State; } }



        private ColyseusClient _coleseusClient;
        private ColyseusRoom<UfbRoomState> _room;
        private UfbApiClient _apiClient;
        private readonly string _roomType = "ufbRoom";


        public UfbRoomClient(UfbApiClient apiClient)
        {
            if (!_apiClient.IsRegistered) throw new Exception("Client is not registered!");
            _apiClient = apiClient;
            var wsURL = _apiClient.GetUrlWithProtocol("wss://");
            _coleseusClient = new ColyseusClient(wsURL);
        }

        public async Task JoinRoom(string roomId, UfbRoomJoinOptions joinRoomOptions)
        {
            joinRoomOptions.token = _apiClient.Token;
            joinRoomOptions.playerId = _apiClient.ClientId;
            var optionsDict = new Dictionary<string, object>() {
                { "joinOptions", joinRoomOptions.ConvertToDictionary() }
            };

            _room = await _coleseusClient.JoinById<UfbRoomState>(roomId, optionsDict);
            RegisterRoomHandlers(_room);
        }

        public async Task CreateRoom(UfbRoomCreateOptions createOptions, UfbRoomJoinOptions joinOptions)
        {
            if (_room != null)
            {
                Debug.LogWarning("Already in a room!");
                return;
            }


            joinOptions.token = _apiClient.Token;
            joinOptions.playerId = _apiClient.ClientId;

            var roomOptionsDict = createOptions.ConvertToDictionary();
            var joinOptionsDict = joinOptions.ConvertToDictionary();

            var optionsDict = new Dictionary<string, object>() {
                { "createOptions", roomOptionsDict },
                { "joinOptions", joinOptionsDict }
            };

            _room = await _coleseusClient.Create<UfbRoomState>(_roomType, optionsDict);
            RegisterRoomHandlers(_room);
        }

        public void LeaveRoom()
        {
            if (_room != null)
            {
                _room.Leave();
                OnRoomLeft?.Invoke();
                _room = null;
            }
        }



        private void RegisterRoomHandlers(ColyseusRoom<UfbRoomState> room)
        {
            room.State.map.OnNameChange((string currentValue, string previousValue) =>
            {
                Debug.Log($"Map name changed from {previousValue} to {currentValue}");
            });

            room.OnLeave += OnRoomLeave;
            room.OnError += OnRoomError;

            room.OnLeave += OnRoomLeave;
            room.OnError += OnRoomError;

            // room.OnMessage<PlayerJoined>("playerJoined", OnPlayerJoinedMessage);
            room.OnMessage<CharacterMovedMessage>("playerMoved", OnPlayerMovedMessage);
            room.OnMessage<NotificationMessage>("notification", OnNotification);

            room.OnStateChange += OnStateChangedMessage;
        }

        private void OnStateChangedMessage(UfbRoomState state, bool isFirstState)
        {
            Debug.Log($"STATE CHANGE | first? {isFirstState} | Received state change message: {state.Serialize()}");

            if (isFirstState)
            {
                OnRoomJoined?.Invoke(_room.State);
            }
        }


        private void OnNotification(NotificationMessage message)
        {
            Debug.Log($"Received notification message: {message.type} - {message.message}");
            OnNotificationMessage?.Invoke(message);
        }

        // private void OnPlayerJoinedMessage(PlayerJoined message)
        // {
        //     Debug.Log($"Received player joined message: {message.clientId}");
        // }

        private void OnPlayerMovedMessage(CharacterMovedMessage message)
        {
            // Debug.Log($"Received player moved message: {message.playerId} to {message.destination}");
            // var tile = gameController.GameBoard.GetTileByCoordinates(message.destination.ToCoordinates());
            // var player = gameController.PlayerManager.GetPlayerById(message.playerId);
            // player.ForceMoveToTile(tile);
            // gameController.PlayerManager.IteratePlayers((player) =>
            // {
            //     player.ForceMoveToTile(tile);
            // });
        }


        private void OnRoomError(int code, string message)
        {
            Debug.Log("Error: " + code + " - " + message);
        }

        private void OnRoomLeave(int code)
        {
            // pull up a re-join option or something
            // make sure to set the room to null, to clean up existing handlers!
            Debug.Log("Left room: " + code);
        }


        // public void MoveMyPlayer(Coordinates coords)
        // {
        //     _room.Send("move", new Dictionary<string, object>() {
        //         { "destination", coords.ToDictionary() }
        //     });
        // }

        // public void MoveMyPlayer(int x, int y)
        // {
        //     MoveMyPlayer(new Coordinates(x, y));
        // }

        // public void MovePlayerToRandomCoords()
        // {
        //     var random = new System.Random();
        //     var coords = new Coordinates(random.Next(0, 10), random.Next(0, 10));
        //     MoveMyPlayer(coords);
        // }
    }

}