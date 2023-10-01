using UnityEngine;
using UFB.Network.RoomMessageTypes;
using UFB.Core;
using UFB.Character;

namespace UFB.UI
{
    public class CreateGameMenu : Menu
    {
        public Menu mainMenu;
        public Menu loadingMenu;
        public Menu selectCharacterMenu;
        public CharacterSelector characterSelector;

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

            characterSelector.OnSelectionChanged += OnCharacterSelectionChanged;
        }

        private void OnDisable()
        {
            characterSelector.OnSelectionChanged -= OnCharacterSelectionChanged;
        }

        private void OnCharacterSelectionChanged(UfbCharacter character)
        {
            var joinOptions = _menuManager.GetMenuData("joinOptions") as UfbRoomJoinOptions;
            joinOptions.characterClass = character.characterClass;
            _menuManager.SetMenuData("joinOptions", joinOptions);
        }

        public void OnBackButton() => _menuManager.CloseMenu();

        public void OnMapButton() => Debug.Log("Map button hit!");

        public void OnPlayerButton()
        {
            _menuManager.OpenMenu(selectCharacterMenu);
        }

        public void OnRulesButton() => Debug.Log("Rules button hit!");

        public void OnStartButton()
        {
            _menuManager.OpenMenu(loadingMenu);
            
            ServiceLocator.Current.Get<GameService>().CreateGame(
                _menuManager.GetMenuData("createOptions") as UfbRoomCreateOptions,
                _menuManager.GetMenuData("joinOptions") as UfbRoomJoinOptions
            );
        }
    }
}