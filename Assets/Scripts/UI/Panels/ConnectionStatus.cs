using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UFB.Events;
using UFB.Network;
using UFB.Core;

namespace UFB.UI
{
    public class ConnectionStatus : MonoBehaviour
    {
        public TextMeshProUGUI text;
        public Image statusImage;

        private void OnEnable()
        {
            SetStatus(ServiceLocator.Current.Get<NetworkService>().Status);
            EventBus.Subscribe<NetworkServiceStatusEvent>(OnNetworkServiceStatusChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<NetworkServiceStatusEvent>(OnNetworkServiceStatusChanged);
        }

        private void OnNetworkServiceStatusChanged(NetworkServiceStatusEvent e) =>
            SetStatus(e.Status);

        private void SetStatus(NetworkService.NetworkServiceStatus status)
        {
            switch (status)
            {
                case NetworkService.NetworkServiceStatus.Ready:
                    statusImage.color = Color.green;
                    text.text = "Connected";
                    break;
                case NetworkService.NetworkServiceStatus.Failed:
                    statusImage.color = Color.red;
                    text.text = "Not Connected";
                    break;
                case NetworkService.NetworkServiceStatus.Initializing:
                    statusImage.color = Color.yellow;
                    text.text = "Connecting...";
                    break;
                default:
                    statusImage.color = Color.red;
                    text.text = "Not Connected";
                    break;
            }
        }
    }
}
