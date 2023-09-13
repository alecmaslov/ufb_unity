using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using UFB.Gameplay;
using UFB.Network;

namespace UFB.UI
{
    public class JoinGameMenu : Menu
    {
        public Menu mainMenu;
        public Menu loadingMenu;
        public TMP_InputField idInputField;

        private void OnEnable()
        {
            idInputField.text = "";
        }

        public async void OnJoinButton() {
            _menuManager.OpenMenu(loadingMenu);
            var joinOptions = new UfbRoomJoinOptions {
                displayName = "Player",
                characterId = "kirin"
            };
            await GameManager.Instance.JoinGame(idInputField.text, joinOptions);
        }

        public void OnBackButton() => _menuManager.OpenMenu(mainMenu);
    }
}
