using UnityEngine;
using UFB.Network.RoomMessageTypes;
using UFB.Core;
using UFB.Character;
using UFB.Map;

namespace UFB.UI
{
    public class CreateGameMenu : Menu
    {
        public Menu mainMenu;
        public Menu loadingMenu;

        // public Menu selectCharacterMenu;
        public CharacterSelector characterSelector;
        public MapSelector mapSelector;

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
            mapSelector.OnSelectionChanged += OnMapSelectionChanged;
        }

        private void OnDisable()
        {
            characterSelector.OnSelectionChanged -= OnCharacterSelectionChanged;
            mapSelector.OnSelectionChanged -= OnMapSelectionChanged;
        }

        private void OnMapSelectionChanged(UfbMap map)
        {
            var createOptions = _menuManager.GetMenuData("createOptions") as UfbRoomCreateOptions;
            createOptions.mapName = map.name;
            _menuManager.SetMenuData("createOptions", createOptions);
        }

        private void OnCharacterSelectionChanged(UfbCharacter character)
        {
            var joinOptions = _menuManager.GetMenuData("joinOptions") as UfbRoomJoinOptions;
            joinOptions.characterClass = character.characterClass;
            _menuManager.SetMenuData("joinOptions", joinOptions);
        }

        public void OnBackButton() => _menuManager.CloseMenu();

        public void OnMapButton() => Debug.Log("Map button hit!");

        public void OnRulesButton() => Debug.Log("Rules button hit!");

        public void OnStartButton()
        {
            _menuManager.OpenMenu(loadingMenu);

            ServiceLocator.Current
                .Get<GameService>()
                .CreateGame(
                    _menuManager.GetMenuData("createOptions") as UfbRoomCreateOptions,
                    _menuManager.GetMenuData("joinOptions") as UfbRoomJoinOptions
                );
        }
    }
}
