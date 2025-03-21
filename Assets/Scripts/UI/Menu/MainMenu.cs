using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UFB.Events;
using UFB.Core;
using UFB.Network;
using UFB.Network.RoomMessageTypes;

namespace UFB.UI
{
    public class MainMenu : Menu
    {
        public Menu newGameMenu;
        public Menu joinGameMenu;
        public Menu selectCharacterMenu;
        public Menu settingMenu;

        public AccountMenuPanel accountMenuPanel;

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

        public void OnJoinRoomButton()
        {
            string roomId = PlayerPrefs.GetString("roomId");
            string token = PlayerPrefs.GetString("sessionId");
            
            Debug.Log(roomId);
            Debug.Log(token);
            
            if (string.IsNullOrEmpty(roomId))
            {
                MainScene.instance.ShowNotificationMessage("error", "Room id is empty");
            }

            if (string.IsNullOrEmpty(token))
            {
                MainScene.instance.ShowNotificationMessage("error", "Token is empty");
            }
            
            ServiceLocator.Current.Get<GameService>().ReConnectRoom(roomId, token);
        }
        
        public void OnAccountGameButton()
        {
            accountMenuPanel.InitPanel();
        }

        public void OnSettingsButton()
        {
            Debug.Log("Settings not implemented yet");
        }
    }
}
