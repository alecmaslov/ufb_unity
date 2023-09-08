using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UFB.UI
{
    public class MainMenu : Menu
    {
        public Menu newGameMenu;
        public Menu joinGameMenu;

        public void OnNewGameButton() => _menuManager.OpenMenu(newGameMenu);
        public void OnJoinGameButton() => _menuManager.OpenMenu(joinGameMenu);
        public void OnSettingsButton() => Debug.Log("Settings not implemented yet");
    }
}






// public class MainMenuManager : MonoBehaviour
// {
//     public RectTransform basePanel;
//     public RectTransform createGamePanel;
//     public RectTransform joinGamePanel;
//     public RectTransform gameSettingsPanel;

//     public void Start()
//     {
//         basePanel.gameObject.SetActive(true);
//         createGamePanel.gameObject.SetActive(false);
//         joinGamePanel.gameObject.SetActive(false);
//         gameSettingsPanel.gameObject.SetActive(false);
//     }

//     public void OnButtonClick(string buttonId)
//     {
//         switch (buttonId)
//         {
//             case "newGame":
//                 basePanel.gameObject.SetActive(false);
//                 createGamePanel.gameObject.SetActive(true);
//                 break;
//             case "joinGame":
//                 basePanel.gameObject.SetActive(false);
//                 joinGamePanel.gameObject.SetActive(true);
//                 break;
//             case "settings":
//                 basePanel.gameObject.SetActive(false);
//                 gameSettingsPanel.gameObject.SetActive(true);
//                 break;
//             case "back":
//                 basePanel.gameObject.SetActive(true);
//                 createGamePanel.gameObject.SetActive(false);
//                 joinGamePanel.gameObject.SetActive(false);
//                 gameSettingsPanel.gameObject.SetActive(false);
//                 break;
//         }
//     }
// }

