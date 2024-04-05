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
        public Menu selectCharacterMenu;
        public Menu settingMenu;

        // public override async void Start()
        // {
        //     base.Start();
        //     await GameController.Instance.Initialize();
        // }

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

        public void OnSettingMenu() => _menuManager.OpenMenu(settingMenu, false);

        public void OnNewGameButton()
        {
            _menuManager.OpenMenu(selectCharacterMenu);
            ((SelectCharacterMenu)selectCharacterMenu).menuType = 0;
        } 

        public void OnJoinGameButton()
        {
            _menuManager.OpenMenu(selectCharacterMenu);
            ((SelectCharacterMenu)selectCharacterMenu).menuType = 1;
        }

        public void OnSettingsButton()
        {
            Debug.Log("Settings not implemented yet");
        }
    }
}
