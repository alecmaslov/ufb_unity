using System.Collections;
using System.Collections.Generic;
using UFB.Gameplay;
using UFB.Network;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace UFB.UI
{
    public class CreateGameMenu : Menu
    {
        public Menu mainMenu;
        public Menu loadingMenu;

        public void OnBackButton() => _menuManager.OpenMenu(mainMenu);

        public void OnMapButton() => Debug.Log("Map button hit!");

        public void OnPlayerButton() => Debug.Log("Player button hit!");

        public void OnRulesButton() => Debug.Log("Rules button hit!");

        public async void OnStartButton()
        {
            var roomOptions = new UfbRoomOptions {
                mapName = "kraken",
                rules = new UfbRoomRules {
                    maxPlayers = 2,
                    initHealth = 100,
                    initEnergy = 100,
                    turnTime = 60f,
                }
            };
        
            _menuManager.OpenMenu(loadingMenu);
            await GameController.Instance.CreateNewGame(roomOptions);
        }
    }
}
