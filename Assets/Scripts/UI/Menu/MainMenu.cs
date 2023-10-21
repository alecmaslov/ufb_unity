using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UFB.Events;
using UFB.Core;
using UFB.Network;

namespace UFB.UI
{
    public class MainMenu : Menu
    {
        public Menu newGameMenu;
        public Menu joinGameMenu;


        private void OnEnable()
        {
            if (
                ServiceLocator.Current.Get<NetworkService>().Status
                != NetworkService.NetworkServiceStatus.Ready
            )
            {
                ToggleButtonInteractability(false);
            }
            EventBus.Subscribe<NetworkServiceStatusEvent>(OnNetworkManagerReady);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<NetworkServiceStatusEvent>(OnNetworkManagerReady);
        }

        private void OnNetworkManagerReady(NetworkServiceStatusEvent e)
        {
            ToggleButtonInteractability(true);
        }

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
