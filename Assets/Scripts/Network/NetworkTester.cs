using UnityEngine;

namespace UFB.Network
{
    public class NetworkTester : MonoBehaviour
    {
        private UFBApiClient _client;

        void Start()
        {
            _client = new UFBApiClient("https://api.thig.io", 8080);
        }

        public void RegisterClient()
        {
            if (_client == null)
                _client = new UFBApiClient("https://api.thig.io", 8080);

            Debug.Log("Registering client...");

            _client.RegisterClient();
        }
    }

}