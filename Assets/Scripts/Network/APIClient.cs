using UnityEngine.Networking;
using System;
using Newtonsoft.Json;
using System.Threading.Tasks; // Don't forget to import this at the top of your file

namespace UFB.Network
{
    public class ApiClient
    {
        public string APIUrl { get { return _protocol + _baseUrl + ":" + _port; } }
        public bool IsSecure { get { return _protocol == "https://"; } }

        private string _protocol;
        private string _baseUrl;
        private int _port;


        public ApiClient(string apiBase, int port, bool useHttps = true)
        {
            this._baseUrl = apiBase;
            this._port = port;
            if (useHttps)
                this._protocol = "https://";
            else
                this._protocol = "http://";
        }

        /// <summary>
        /// Gets the url but with a different protocol (useful for ws/wss)
        /// </summary>
        public string GetUrlWithProtocol(string protocol)
        {
            return protocol + _baseUrl + ":" + _port;
        }


        public async Task<T> Get<T>(string endpoint)
        {
            var tcs = new TaskCompletionSource<T>();

            using (UnityWebRequest webRequest = UnityWebRequest.Get(APIUrl + endpoint))
            {
                UnityWebRequestAsyncOperation asyncOperation = webRequest.SendWebRequest();

                asyncOperation.completed += (operation) =>
                {
                    if (webRequest.result != UnityWebRequest.Result.Success)
                    {
                        UnityEngine.Debug.Log("ERROR: " + webRequest.error);
                        tcs.SetException(new Exception(webRequest.error));
                    }
                    else
                    {
                        T result = JsonConvert.DeserializeObject<T>(webRequest.downloadHandler.text);
                        tcs.SetResult(result);
                    }
                };
            }
            return await tcs.Task;
        }

        public async Task<T> Post<T>(string endpoint, string body)
        {
            var tcs = new TaskCompletionSource<T>();

            UnityEngine.Debug.Log("=> APIClient [POST]: " + APIUrl + endpoint + " | " + body);

            UnityWebRequest webRequest = UnityWebRequest.Post(APIUrl + endpoint, body, "application/json");

            UnityWebRequestAsyncOperation asyncOperation = webRequest.SendWebRequest();

            asyncOperation.completed += (operation) =>
            {
                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    tcs.SetException(new Exception(webRequest.error));
                }
                else
                {
                    T result = JsonConvert.DeserializeObject<T>(webRequest.downloadHandler.text);
                    tcs.SetResult(result);
                }
            };

            return await tcs.Task;
        }
    }
}