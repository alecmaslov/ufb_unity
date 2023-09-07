using System;
using UnityEngine;
using Colyseus;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using Colyseus.Schema; 

namespace UFB.Network
{


    public class FooMessage : Schema {
        [Colyseus.Schema.Type(0, "hello")]
        public string message;
    }



    public class NetworkTester : MonoBehaviour
    {
        public UfbApiClient Client
        {
            get
            {
                _client ??= new UfbApiClient("api.thig.io", 8080);
                return _client;
            }
        }

        private UfbApiClient _client;

        private ColyseusClient _colyseusClient;
        private ColyseusRoom<FooMessage> _room;
        // private Room _room;

        public async void RegisterClient()
        {
            Debug.Log("Registering client...");
            try
            {
                // await Client.RegisterClient();
                // Debug.Log("Creating colyseus client...");
                _colyseusClient = new ColyseusClient("wss://api.thig.io:8080");
            }
            catch (Exception e)
            {
                Debug.Log("Exception: " + e);
            }
        }

        public async void CreateWebsocketConnection()
        {
            Debug.Log("Creating websocket connection...");
            if (!Client.IsRegistered)
            {
                Debug.Log("Client is not registered!");
                return;
            }
            try
            {
                await Client.CreateWebsocketConnection();
                // await websocket.SendText("hello");
            }
            catch (Exception e)
            {
                Debug.Log("Exception: " + e);
            }
        }

        public async void CreateNewRoom()
        {
            try
            {
                // _room = await _colyseusClient.JoinOrCreate<Dictionary<string, string>>("my_room", new());
                _room = await _colyseusClient.JoinOrCreate<FooMessage>("my_room", new());
                var task = _room.Send("hello", new { hello = "I will make you hurt" });

                var hander = new Action<FooMessage>((message) =>
                {
                    Debug.Log(message.message);
                });


                _room.OnMessage("hello", hander);
            }
            catch (Exception e)
            {
                Debug.Log("Exception: " + e);
            }
        }


        public async void SendWebsocketHello()
        {
            Debug.Log("Sending websocket hello...");
            if (!Client.IsRegistered)
            {
                Debug.Log("Client is not registered!");
                return;
            }
            if (Client.Websocket == null)
            {
                Debug.Log("Websocket is not connected!");
                return;
            }
            try
            {
                await Client.Websocket.SendText("{\"type\": \"hello\"}");
            }
            catch (Exception e)
            {
                Debug.Log("Exception: " + e);
            }
        }
    }

}