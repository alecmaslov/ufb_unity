using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UFB.Events;
using UnityEngine;

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
        public bool IsRegistered
        {
            get { return _clientId != null; }
        }
        public string ClientId
        {
            get { return _clientId; }
        }
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
        private string _clientId;
        private string _token;

        public UfbApiClient(string apiBase, int port)
            : base(apiBase, port) { }

        private async Task<bool> ValidateToken()
        {
            var token = PlayerPrefs.GetString("token");
            if (token == null || token == "")
            {
                Debug.Log("No token found!");
                return false;
            }

            // if we do have a token, see if the server validates it
            // if it is valid, the server will reply with the clientId
            try
            {
                var response = await Post<ValidTokenResponse>(
                    "/auth/validate-token",
                    JsonConvert.SerializeObject(new { token })
                );
                _clientId = response.clientId;
                Token = token;
                return true;
            }
            catch (Exception e)
            {
                Debug.Log("Exception: " + e);
                return false;
            }
        }

        public async Task RegisterClient()
        {
            if (IsRegistered)
            {
                Debug.Log("Already registered!");
                return;
            }
            bool isValid = await ValidateToken();
            if (isValid)
            {
                Debug.Log("Token is valid, client registered!");
                return;
            }

            var platformType = GetPlatformType();
            var jsonData = JsonConvert.SerializeObject(new { platform = platformType.ToString() });
            var clientResponse = await Post<RegisterClientResponse>(
                "/auth/register-client",
                jsonData
            );
            _clientId = clientResponse.clientId;
            await GenerateToken(_clientId);
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
                Debug.Log("Exception: " + e);
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
            Debug.Log("Running in Unity Editor");
#endif
            Debug.Log("Platform type: " + type);
            return type;
        }

        private async Task<ApiTypes.MapTile[]> GetMapTiles(string mapId)
        {
            var mapTiles = await Get<ApiTypes.MapTile[]>(
                $"/maps/tiles?mapId={mapId}"
            );
            return mapTiles;
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
