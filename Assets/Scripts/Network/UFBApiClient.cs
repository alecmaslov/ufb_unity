using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UFB.Network
{
    public enum PlatformType
    {
        WEB,
        ANDROID,
        IOS,
        STEAM,
        UNITY_EDITOR
    }



    public class UFBApiClient : APIClient
    {
        public bool IsRegistered { get { return _clientId != null; } }
        public string ClientId { get { return _clientId; } }
        public ServerWebsocket Websocket { get { return _websocket; } }

        private ServerWebsocket _websocket;
        private string _clientId;
        private string _token;

        public UFBApiClient(string apiBase, int port) : base(apiBase, port)
        {
        }

        public async Task RegisterClient()
        {
            if (IsRegistered)
            {
                UnityEngine.Debug.Log("Already registered!");
                return;
            }
            try
            {
                var platformType = GetPlatformType();
                var jsonData = JsonConvert.SerializeObject(new { platform = platformType.ToString() });
                var clientResponse = await Post<RegisterClientResponse>("/register-client", jsonData);
                _clientId = clientResponse.clientId;
                await GenerateToken(_clientId);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log("Exception: " + e);
            }
        }

        public async Task GenerateToken(string clientId)
        {
            try
            {
                var jsonData = JsonConvert.SerializeObject(new { clientId });
                var response = await Post<TokenResponse>("/token", jsonData);
                _token = response.token;
                UnityEngine.Debug.Log("token: " + _token);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log("Exception: " + e);
            }
        }

        public async Task GenerateToken() => await GenerateToken(_clientId);

        public PlatformType GetPlatformType()
        {
            PlatformType type = PlatformType.WEB; // Default value

#if UNITY_WEBGL && !UNITY_EDITOR
    type = PlatformType.WEB;
#elif UNITY_ANDROID && !UNITY_EDITOR
    type = PlatformType.ANDROID;
#elif UNITY_IOS && !UNITY_EDITOR
    type = PlatformType.IOS;
#elif UNITY_EDITOR
            type = PlatformType.UNITY_EDITOR;
            UnityEngine.Debug.Log("Running in Unity Editor");
#endif

            UnityEngine.Debug.Log("Platform type: " + type);
            return type;
        }


        public async Task CreateWebsocketConnection()
        {
            if (!IsRegistered)
            {
                throw new Exception("Client not registered!");
            }

            UnityEngine.Debug.Log("Current websocket" + this._websocket);
            if (this._websocket != null)
            {
                UnityEngine.Debug.Log("Websocket already connected!");
                return;
            }

            var tcs = new TaskCompletionSource<ServerWebsocket>();
            _websocket = new ServerWebsocket(this);
            tcs.SetResult(_websocket);

            _websocket.OnOpenEvent += () =>
            {
                UnityEngine.Debug.Log("[API] Websocket opened!");
            };

            _websocket.OnMessageEvent += (message) =>
            {
                UnityEngine.Debug.Log("[API] Websocket message: " + message);   
            };

            _websocket.OnCloseEvent += (code) =>
            {
                UnityEngine.Debug.Log("[API] Websocket closed: " + code);
                _websocket = null;
            };

            _websocket.OnErrorEvent += (error) =>
            {
                UnityEngine.Debug.Log("[API] Websocket error: " + error);
            };


            try
            {
                await _websocket.Connect("?token=" + _token);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log("Exception: " + e);
                tcs.SetException(e);
            }
        }


        public struct TokenResponse
        {
            public string token;
        }
        public struct RegisterClientResponse
        {
            public string clientId;
        }

    }

}