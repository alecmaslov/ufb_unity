using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

namespace UFB.UI
{
    public class JoinGameMenu : Menu
    {
        public Menu mainMenu;
        public TMP_InputField idInputField;

        private void OnEnable()
        {
            idInputField.text = "";
        }

        public void OnJoinButton() => Debug.Log("Join button hit! " + idInputField.text);
        public void OnBackButton() => _menuManager.OpenMenu(mainMenu);
    }
}
