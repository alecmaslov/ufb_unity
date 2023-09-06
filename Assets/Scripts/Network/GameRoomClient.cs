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
    public class GameRoomState : Schema
    {
        [Colyseus.Schema.Type(0, "string")]
        public string mySynchronizedProperty;

        [Colyseus.Schema.Type(1, "number")]
        public float boardWidth;

        [Colyseus.Schema.Type(2, "number")]
        public float boardHeight;

        [Colyseus.Schema.Type(3, "number")]
        public float turn;

        [Colyseus.Schema.Type(4, "map", typeof(MapSchema<PlayerState>))]
        public MapSchema<PlayerState> players = new MapSchema<PlayerState>();
    }

    public class PlayerJoined : Schema
    {
        [Colyseus.Schema.Type(0, "string")]
        public string clientId;

        [Colyseus.Schema.Type(1, "boolean")]
        public bool isMe = false;

        [Colyseus.Schema.Type(2, "number")]
        public float x;

        [Colyseus.Schema.Type(3, "number")]
        public float y;

        public Coordinates Coordinates => new Coordinates((int)x, (int)y);

    }

    public class PlayerMoved : Schema
    {
        [Colyseus.Schema.Type(0, "string")]
        public string playerId;

        [Colyseus.Schema.Type(1, "number")]
        public float x;

        [Colyseus.Schema.Type(2, "number")]
        public float y;

        public Coordinates NewCoords => new Coordinates((int)x, (int)y);
    }


    public class GameRoomClient : MonoBehaviour
    {
        public GameController gameController;

        private ColyseusClient _coleseusClient;
        private ColyseusRoom<GameRoomState> _room;
        private UFBApiClient _apiClient;
        private string _currentPlayerId;

        void Start()
        {

            // here we create the apiClient
            InitializeClient();
        }

        int clientNum = 0;

        public async void JoinOrCreate()
        {

            Dictionary<string, object> roomOptions = new Dictionary<string, object>
            {
                ["YOUR_ROOM_OPTION_1"] = "option 1",
                ["YOUR_ROOM_OPTION_2"] = "option 2"
            };

            int currentClientNum = clientNum++;
            _room = await _coleseusClient.JoinOrCreate<GameRoomState>("my_room", roomOptions);
            _room.OnLeave += OnRoomLeave;
            _room.OnError += OnRoomError;

            var messageHandler = new Action<GameRoomState>((message) =>
            {
                Debug.Log($"{currentClientNum} | OnMessage: {message}");
            });

            _room.OnMessage("playerJoined", (PlayerJoined message) =>
            {
                Debug.Log($"{currentClientNum} | Who am I?: {message.clientId}");
                if (!message.isMe) return;
                _currentPlayerId = message.clientId;
                gameController.PlayerManager.SpawnRandomPlayer(message.clientId, message.Coordinates);
                var player = gameController.PlayerManager.GetPlayerById(message.clientId);
                player.FocusCamera();
            });

            _room.OnMessage("playerMoved", (PlayerMoved message) =>
            {
                Debug.Log($"{currentClientNum} | Player moved: {message.playerId} to {message.NewCoords}");
                var tile = gameController.GameBoard.GetTileByCoordinates(message.NewCoords);
                // var player = gameController.PlayerManager.GetPlayerById(message.playerId);


                var player = gameController.PlayerManager.GetPlayerById(message.playerId);
                
                gameController.PlayerManager.IteratePlayers((player) => {
                    player.ForceMoveToTile(tile);
                });
            });

            _room.OnStateChange += (state, isFirstState) =>
            {
                Debug.Log($"{currentClientNum} | New state: {state.mySynchronizedProperty}");
            };

        }

        public void InitializeClient()
        {
            _coleseusClient = new ColyseusClient("wss://api.thig.io:8080");
            _apiClient = new UFBApiClient("api.thig.io", 8080);
            Debug.Log("Client initialized");
        }


        public void PrintState()
        {
            Debug.Log("Current room state: " + _room.State.mySynchronizedProperty);
            Debug.Log($"Name {_room.Name} RoomId {_room.RoomId} Client {_room}");
        }

        public void SendHelloMessage()
        {
            if (_room != null)
            {
                _room.Send("hello", "yoooooo");
            }
        }


        public void PrintAllClients()
        {

            Debug.Log($"Current player id: {_currentPlayerId} | num players: {_room.State.players.Count}");
            // foreach(var client in _room.State.players)
            // {
            //     Debug.Log($"Client: {client.Key} {client.Value}");
            // }
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


        private void OnRoomError(int code, string message)
        {
            Debug.Log("Error: " + code + " - " + message);
        }

        private void OnRoomLeave(int code)
        {
            Debug.Log("Left room: " + code);
        }

    }

}