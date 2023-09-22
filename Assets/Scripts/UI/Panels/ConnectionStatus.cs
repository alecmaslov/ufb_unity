using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UFB.Gameplay;
using UFB.Events;


namespace UFB.UI
{
    public class ConnectionStatus : MonoBehaviour
    {
        public TextMeshProUGUI text;
        public Image statusImage;


        private void Awake()
        {
            statusImage.color = Color.red;
            text.text = "Not Connected";
        }

        private void OnEnable()  // Subscribing on enabling instead of Start for better symmetry
        {
            EventBus.Subscribe<ApiClientRegisteredEvent>(OnApiClientRegistered);
            EventBus.Subscribe<NetworkManagerDisconnectedEvent>(OnNetworkManagerDisconnected);
        }

        private void OnDisable()  // Unsubscribing on disabling the object
        {
            EventBus.Unsubscribe<ApiClientRegisteredEvent>(OnApiClientRegistered);
            EventBus.Unsubscribe<NetworkManagerDisconnectedEvent>(OnNetworkManagerDisconnected);
        }

        private void OnNetworkManagerDisconnected(NetworkManagerDisconnectedEvent e)
        {
            statusImage.color = Color.red;
            text.text = "Not Connected";
        }

        private void OnApiClientRegistered(ApiClientRegisteredEvent e)
        {
            statusImage.color = Color.green;
            text.text = "Connected";
        }

        void Start()
        {
            statusImage.color = Color.red;
            text.text = "Not Connected";
        }
    }
}
