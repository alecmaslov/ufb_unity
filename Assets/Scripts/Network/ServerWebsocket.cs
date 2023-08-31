using System.Diagnostics;
using NativeWebSocket;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UFB.Network
{
    public class ServerWebsocket
    {
        private APIClient _client;
        private WebSocket _websocket;

        public ServerWebsocket(APIClient client)
        {
            
        }

        async void Start()
        {
            _websocket = new WebSocket("wss://");

            _websocket.OnOpen += () =>
            {
                UnityEngine.Debug.Log("Connection Open!");
            };

            _websocket.OnError += (e) =>
            {
                UnityEngine.Debug.Log("Error!" + e);
            };

            _websocket.OnClose += (e) =>
            {
                UnityEngine.Debug.Log("Connection closed!");
            };

            _websocket.OnMessage += (bytes) =>
            {
                UnityEngine.Debug.Log("OnMessage");
                UnityEngine.Debug.Log(bytes);
            };

            await _websocket.Connect();
        }

    }

}
