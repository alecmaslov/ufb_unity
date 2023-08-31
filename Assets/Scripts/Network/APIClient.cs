using UnityEngine.Networking;
using System;
using Newtonsoft.Json;
using System.Threading.Tasks; // Don't forget to import this at the top of your file

namespace UFB.Network
{
    public class APIClient
    {
        public string APIUrl { get { return apiBase + ":" + port; } }

        private string apiBase;
        private int port;


        public APIClient(string apiBase, int port)
        {
            this.apiBase = apiBase;
            this.port = port;
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

            UnityEngine.Debug.Log("POSTING TO: " + APIUrl + endpoint + " BODY: " + body);

            UnityWebRequest webRequest = UnityWebRequest.Post(APIUrl + endpoint, body, "application/json");

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

            return await tcs.Task;
        }
    }
}