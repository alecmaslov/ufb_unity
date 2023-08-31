using System;
using UnityEngine;

namespace UFB.Network
{
    public class NetworkTester : MonoBehaviour
    {
        public UFBApiClient Client
        {
            get
            {
                _client ??= new UFBApiClient("api.thig.io", 8080);
                return _client;
            }
        }

        private UFBApiClient _client;

        public async void RegisterClient()
        {
            Debug.Log("Registering client...");
            try
            {
                await Client.RegisterClient();
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