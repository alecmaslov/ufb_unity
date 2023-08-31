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
        private string _clientId;
        private string _token;

        public UFBApiClient(string apiBase, int port) : base(apiBase, port)
        {
        }

        public async Task RegisterClient(Action callback = null)
        {
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