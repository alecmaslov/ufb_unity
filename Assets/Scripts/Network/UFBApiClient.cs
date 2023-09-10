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



    public class UfbApiClient : ApiClient
    {
        // public delegate void OnClientConnectedHander();
        // public event OnClientConnectedHander OnClientConnected;


        public bool IsRegistered { get { return _clientId != null; } }
        public string ClientId { get { return _clientId; } }
        public ServerWebsocket Websocket { get { return _websocket; } }
        public string Token
        {
            get { return _token; }
            set
            {
                // store in the local storage
                UnityEngine.PlayerPrefs.SetString("token", value);
                _token = value;
            }
        }


        private ServerWebsocket _websocket;
        private string _clientId;
        private string _token;

        public UfbApiClient(string apiBase, int port) : base(apiBase, port)
        {
        }

        private async Task<bool> ValidateToken()
        {
            var token = UnityEngine.PlayerPrefs.GetString("token");
            if (token == null || token == "")
            {
                UnityEngine.Debug.Log("No token found!");
                return false;
            }

            // if we do have a token, see if the server validates it
            // if it is valid, the server will reply with the clientId
            try
            {
                var response = await Post<ValidTokenResponse>("/auth/validate-token", JsonConvert.SerializeObject(new { token }));
                _clientId = response.clientId;
                Token = token;
                return true;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log("Exception: " + e);
                return false;
            }
        }

        public async Task<bool> RegisterClient()
        {       
            if (IsRegistered)
            {
                UnityEngine.Debug.Log("Already registered!");
                return true;
            }
            bool isValid = await ValidateToken();
            if (isValid)
            {
                UnityEngine.Debug.Log("Token is valid, client registered!");
                return true;
            }

            try
            {
                var platformType = GetPlatformType();
                var jsonData = JsonConvert.SerializeObject(new { platform = platformType.ToString() });
                var clientResponse = await Post<RegisterClientResponse>("/auth/register-client", jsonData);
                _clientId = clientResponse.clientId;
                await GenerateToken(_clientId);
                return true;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log("Exception: " + e);
                return false;
            }
        }

        public async Task GenerateToken(string clientId)
        {
            try
            {
                var jsonData = JsonConvert.SerializeObject(new { clientId });
                var response = await Post<TokenResponse>("/auth/token", jsonData);
                Token = response.token;
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

        public struct TokenResponse
        {
            public string token;
        }
        public struct RegisterClientResponse
        {
            public string clientId;
        }

        public struct ValidTokenResponse
        {
            public string clientId;
        }

    }

}