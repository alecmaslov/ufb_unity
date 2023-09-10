using System.Diagnostics;
using NativeWebSocket;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;

namespace UFB.Network
{

    public class WebSocketMessage {
        public string type;
        public string data;
    }

    public class ServerWebsocket
    {
        public bool IsConnected { get { return _websocket != null && _websocket.State == WebSocketState.Open; } }
        public event Action OnOpenEvent;
        public event Action<string> OnErrorEvent;
        public event Action<WebSocketCloseCode> OnCloseEvent;
        public event Action<byte[]> OnMessageEvent;

        private ApiClient _client;
        private WebSocket _websocket;
        private string _protocol;

        public ServerWebsocket(ApiClient client)
        {
            _client = client;
            _protocol = client.IsSecure ? "wss://" : "ws://";
        }

        public async Task Connect(string connectionEndpoint)
        {
            if (IsConnected)
            {
                UnityEngine.Debug.Log("Already conncted!");
                return;
            }
            _websocket = new WebSocket(_client.GetUrlWithProtocol(_protocol) + connectionEndpoint);
            _websocket.OnOpen += OnOpen;
            _websocket.OnError += OnError;
            // _websocket.OnClose += OnClose;
            _websocket.OnMessage += OnMessage;
            // _websocket.OnMessage += OnStringMessage;
            await _websocket.Connect();
        }

        public async Task SendBytes(byte[] bytes)
        {
            if (!IsConnected)
                throw new System.Exception("Not connected to server!");
            await _websocket.Send(bytes);
        }

        public async Task SendText(string text)
        {
            if (!IsConnected)
                throw new System.Exception("Not connected to server!");
            await _websocket.SendText(text);
        }

        public async Task SendObject<T>(T obj)
        {
            if (!IsConnected)
                throw new System.Exception("Not connected to server!");
            await _websocket.SendText(JsonConvert.SerializeObject(obj));
        }

        public async void Close()
        {
            await _websocket.Close();
        }

        private void OnOpen()
        {
            // UnityEngine.Debug.Log("Connection Open!");
            OnOpenEvent?.Invoke();
        }

        private void OnError(string e)
        {
            // UnityEngine.Debug.Log("Error!" + e);
            OnErrorEvent?.Invoke(e);
        }

        private void OnClose(WebSocketCloseCode e)
        {
            // UnityEngine.Debug.Log("Connection closed!");
            OnCloseEvent?.Invoke(e);
        }

        private void OnMessage(byte[] bytes)
        {
            UnityEngine.Debug.Log("OnMessage");
            UnityEngine.Debug.Log(bytes);

            var message = System.Text.Encoding.UTF8.GetString(bytes);
            UnityEngine.Debug.Log(message);

            OnMessageEvent?.Invoke(bytes);
        }


        private void OnStringMessage(string message) {
            UnityEngine.Debug.Log("OnStringMessage");
            UnityEngine.Debug.Log(message);
            WebSocketMessage msg = JsonConvert.DeserializeObject<WebSocketMessage>(message);
            UnityEngine.Debug.Log(msg.type);
            UnityEngine.Debug.Log(msg.data);
        }

    }

}
