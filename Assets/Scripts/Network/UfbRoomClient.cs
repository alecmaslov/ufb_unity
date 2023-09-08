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

namespace UFB.Network
{
    public class UfbRoomClient : MonoBehaviour
    {
        public GameController gameController;

        public delegate void OnClientInitializedHander();
        public delegate void OnRoomJoinedHandler(string roomId);
        public delegate void OnRoomLeftHandler();
        public delegate void OnPlayerJoinedHandler(string playerId);


        public event OnClientInitializedHander OnClientInitialized;
        public event OnRoomJoinedHandler OnRoomJoined;
        public event OnRoomLeftHandler OnRoomLeft;
        public event OnPlayerJoinedHandler OnPlayerJoined;


        private ColyseusClient _coleseusClient;
        private ColyseusRoom<UfbRoomState> _room;
        private UfbApiClient _apiClient;
        private string _currentPlayerId;
        private readonly string _roomType = "ufbRoom";


        void Start() => InitializeClient();

        public async void InitializeClient()
        {
            _apiClient = new UfbApiClient("api.thig.io", 8080);
            var wsURL = _apiClient.GetUrlWithProtocol("wss://");
            _coleseusClient = new ColyseusClient(wsURL);
            await _apiClient.RegisterClient();
            if (_apiClient.IsRegistered)
            {
                OnClientInitialized?.Invoke();
                Debug.Log("Client initialized");
            }
        }

        public async void JoinRoom(string roomId)
        {
            if (!_apiClient.IsRegistered) throw new Exception("Client is not registered!");

            Dictionary<string, object> roomOptions = new Dictionary<string, object> {
                { "token", _apiClient.Token },
            };

            try
            {
                _room = await _coleseusClient.JoinById<UfbRoomState>(roomId, roomOptions);
                RegisterRoomHandlers(_room);
                OnRoomJoined?.Invoke(roomId);
            }
            catch (Exception e)
            {
                Debug.Log("Error joining room: " + e);
                return;
            }

        }

        public async void CreateRoom()
        {
            if (!_apiClient.IsRegistered) throw new Exception("Client is not registered!");

            if (_room != null)
            {
                Debug.LogWarning("Already in a room!");
                return;
            }

            Dictionary<string, object> roomOptions = new Dictionary<string, object>
            {
                { "token", _apiClient.Token },
            };

            try
            {
                _room = await _coleseusClient.Create<UfbRoomState>(_roomType, roomOptions);
                RegisterRoomHandlers(_room);
                OnRoomJoined?.Invoke(_room.RoomId);
            }
            catch (Exception e)
            {
                Debug.Log("Error creating room: " + e);
                return;
            }
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
            room.OnLeave += OnRoomLeave;
            room.OnError += OnRoomError;

            _room.OnLeave += OnRoomLeave;
            _room.OnError += OnRoomError;

            _room.OnMessage<PlayerJoined>("playerJoined", OnPlayerJoinedMessage);
            _room.OnMessage<PlayerMoved>("playerMoved", OnPlayerMovedMessage);

            _room.OnStateChange += OnStateChangedMessage;
        }

        private void OnStateChangedMessage(UfbRoomState state, bool isFirstState)
        {
            Debug.Log($"Received state changed message: {state.mySynchronizedProperty}");
        }

        private void OnPlayerJoinedMessage(PlayerJoined message)
        {
            Debug.Log($"Received player joined message: {message.clientId}");
            if (!message.isMe) return;
            _currentPlayerId = message.clientId;
            gameController.PlayerManager.SpawnRandomPlayer(message.clientId, message.Coordinates);
            var player = gameController.PlayerManager.GetPlayerById(message.clientId);
            player.FocusCamera();

            OnPlayerJoined?.Invoke(message.clientId);
        }

        private void OnPlayerMovedMessage(PlayerMoved message)
        {
            Debug.Log($"Received player moved message: {message.playerId} to {message.NewCoords}");
            var tile = gameController.GameBoard.GetTileByCoordinates(message.NewCoords);
            var player = gameController.PlayerManager.GetPlayerById(message.playerId);
            player.ForceMoveToTile(tile);
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

        public void MoveCurrentPlayer(Coordinates coords)
        {
            _room.Send("move", new
            {
                x = coords.X,
                y = coords.Y
            });
        }

        public void MoveCurrentPlayer(int x, int y)
        {
            MoveCurrentPlayer(new Coordinates(x, y));
        }

        public void MovePlayerToRandomCoords()
        {
            var random = new System.Random();
            var coords = new Coordinates(random.Next(0, 10), random.Next(0, 10));
            MoveCurrentPlayer(coords);
        }
    }

}