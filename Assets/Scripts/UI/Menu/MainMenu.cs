using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UFB.Gameplay;
using UFB.Events;
using UFB.Core;
using UFB.Network;

namespace UFB.UI
{
    public class MainMenu : Menu
    {
        public Menu newGameMenu;
        public Menu joinGameMenu;

        // public override async void Start()
        // {
        //     base.Start();
        //     await GameController.Instance.Initialize();
        // }

        private void OnEnable()
        {
            if (!ServiceLocator.Current.Get<NetworkService>().ApiClient.IsRegistered)
            {
                ToggleButtonInteractability(false);
            }
            EventBus.Subscribe<NetworkManagerReadyEvent>(OnNetworkManagerReady);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<NetworkManagerReadyEvent>(OnNetworkManagerReady);
        }

        private void OnNetworkManagerReady(NetworkManagerReadyEvent e)
        {
            ToggleButtonInteractability(true);
            // EventBus.Subscribe<RoomJoinedEvent>(OnRoomJoined);
            // EventBus.Subscribe<RoomLeftEvent>(OnRoomLeft);
            // EventBus.Subscribe<RoomErrorEvent>(OnRoomError);
        }

        private void OnConnect() => ToggleButtonInteractability(true);

        private void ToggleButtonInteractability(bool interactable)
        {
            var buttons = GetComponentsInChildren<Button>();
            foreach (var button in buttons)
            {
                button.interactable = interactable;
            }
        }

        public void OnNewGameButton() => _menuManager.OpenMenu(newGameMenu);

        public void OnJoinGameButton() => _menuManager.OpenMenu(joinGameMenu);

        public void OnSettingsButton()
        {
            Debug.Log("Settings not implemented yet");
        }
    }
}
