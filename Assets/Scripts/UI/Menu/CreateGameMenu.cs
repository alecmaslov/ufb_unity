using UFB.Gameplay;
using UFB.Network;
using UnityEngine;

namespace UFB.UI
{
    public class CreateGameMenu : Menu
    {
        public Menu mainMenu;
        public Menu loadingMenu;
        public Menu selectCharacterMenu;

        private void OnEnable()
        {
            if (_menuManager.GetMenuData("joinOptions") == null)
            {
                _menuManager.SetMenuData("joinOptions", new UfbRoomJoinOptions());
            }

            if (_menuManager.GetMenuData("createOptions") == null)
            {
                _menuManager.SetMenuData("createOptions", new UfbRoomCreateOptions());
            }
        }

        public void OnBackButton() => _menuManager.CloseMenu();

        public void OnMapButton() => Debug.Log("Map button hit!");

        public void OnPlayerButton()
        {
            _menuManager.OpenMenu(selectCharacterMenu);
        }

        public void OnRulesButton() => Debug.Log("Rules button hit!");

        public async void OnStartButton()
        {
            _menuManager.OpenMenu(loadingMenu);
            await GameManager.Instance.CreateNewGame(
                _menuManager.GetMenuData("createOptions") as UfbRoomCreateOptions,
                _menuManager.GetMenuData("joinOptions") as UfbRoomJoinOptions
            );
        }
    }
}

// var createOptions = new UfbRoomCreateOptions
// {
//     mapName = "kraken",
//     rules = new UfbRoomRules
//     {
//         maxPlayers = 2,
//         initHealth = 100,
//         initEnergy = 100,
//         turnTime = 60f,
//     },
// };

// var joinOptions = new UfbRoomJoinOptions
// {
//     displayName = "Player",
//     characterId = "kirin"
// };
