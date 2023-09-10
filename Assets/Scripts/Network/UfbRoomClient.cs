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

namespace UFB.Network
{

    public struct UfbRoomRules
    {
        public int maxPlayers;
        public int initHealth;
        public int initEnergy;
        public float turnTime;
    }

    public struct UfbRoomOptions
    {
        public string mapName;
        public UfbRoomRules rules;


        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object> {
                { "mapName", mapName },
                { "rules", rules }
            };
        }
    }


    /// Look into using ColyseusManager
    public class UfbRoomClient
    {
        public GameController gameController;

        public delegate void OnClientInitializedHander();
        public delegate void OnRoomJoinedHandler(UfbRoomState roomState);
        public delegate void OnRoomLeftHandler();
        public delegate void OnPlayerJoinedHandler(string playerId);

        public ColyseusRoom<UfbRoomState> Room { get { return _room; } }

        public event OnClientInitializedHander OnClientInitialized;
        public event OnRoomJoinedHandler OnRoomJoined;
        public event OnRoomLeftHandler OnRoomLeft;
        public event OnPlayerJoinedHandler OnPlayerJoined;


        private ColyseusClient _coleseusClient;
        private ColyseusRoom<UfbRoomState> _room;
        private UfbApiClient _apiClient;
        private readonly string _roomType = "ufbRoom";


        public UfbRoomClient(UfbApiClient apiClient)
        {
            if (!apiClient.IsRegistered)
            {
                throw new Exception("Client is not registered. Client must be authenticated before creating or joining a game room.");
            }
            _apiClient = apiClient;
            var wsURL = _apiClient.GetUrlWithProtocol("wss://");
            _coleseusClient = new ColyseusClient(wsURL);
        }

        public async Task JoinRoom(string roomId)
        {
            if (!_apiClient.IsRegistered) throw new Exception("Client is not registered!");

            Dictionary<string, object> roomOptions = new Dictionary<string, object> {
                { "token", _apiClient.Token },
            };

            try
            {
                _room = await _coleseusClient.JoinById<UfbRoomState>(roomId, roomOptions);
                RegisterRoomHandlers(_room);
                OnRoomJoined?.Invoke(_room.State);
            }
            catch (Exception e)
            {
                Debug.Log("Error joining room: " + e);
                return;
            }

        }

        public async Task CreateRoom(UfbRoomOptions ufbRoomOptions)
        {
            if (!_apiClient.IsRegistered) throw new Exception("Client is not registered!");

            if (_room != null)
            {
                Debug.LogWarning("Already in a room!");
                return;
            }

            var roomOptionsDict = ufbRoomOptions.ToDictionary();
            roomOptionsDict.Add("token", _apiClient.Token);

            try
            {
                _room = await _coleseusClient.Create<UfbRoomState>(_roomType, roomOptionsDict);
                RegisterRoomHandlers(_room);
                // Debug.Log("Created room! " + _room.State.map.name);
                OnRoomJoined?.Invoke(_room.State);
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
            Debug.Log(room.State.AsDictionary().Keys);

            room.State.map.OnNameChange((string currentValue, string previousValue) =>
            {
                Debug.Log($"Map name changed from {previousValue} to {currentValue}");
            });



            room.OnLeave += OnRoomLeave;
            room.OnError += OnRoomError;

            room.OnLeave += OnRoomLeave;
            room.OnError += OnRoomError;

            room.OnMessage<PlayerJoined>("playerJoined", OnPlayerJoinedMessage);
            room.OnMessage<PlayerMoved>("playerMoved", OnPlayerMovedMessage);
            room.OnMessage<object>("mapChanged", (object value) => {
                Debug.Log("Map changed: " + value);
                Debug.Log("MAP NAME!!! " + room.State.map.name);
            });

            
            

            room.OnStateChange += OnStateChangedMessage;

            room.State.players.OnAdd((player, key) =>
            {
                Debug.Log("Player added: " + player);
            });

            // _room.State.
                // OnPlayerJoined?.Invoke(player.clientId);
        }

        private void OnStateChangedMessage(UfbRoomState state, bool isFirstState)
        {
            Debug.Log("Received state change message: " + state);
        }


        private void OnNotification()
        {

        }

        private void OnPlayerJoinedMessage(PlayerJoined message)
        {
            Debug.Log($"Received player joined message: {message.clientId}");
            if (!message.isMe) return;
            // gameController.PlayerManager.SpawnRandomPlayer(message.clientId, message.Coordinates);
            // var player = gameController.PlayerManager.GetPlayerById(message.clientId);
            // player.FocusCamera();

            // OnPlayerJoined?.Invoke(message.clientId);
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