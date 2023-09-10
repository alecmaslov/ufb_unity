using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using UFB.Gameplay;

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
            await GameController.Instance.JoinGame(idInputField.text);
        }

        public void OnBackButton() => _menuManager.OpenMenu(mainMenu);
    }
}
