using UFB.Gameplay;
using UFB.Network.RoomMessageTypes;
using TMPro;
using UFB.Core;

namespace UFB.UI
{
    public class JoinGameMenu : Menu
    {
        public Menu loadingMenu;
        public TMP_InputField idInputField;

        private void OnEnable()
        {
            idInputField.text = "";
        }

        public void OnJoinButton() {
            _menuManager.OpenMenu(loadingMenu);
            var joinOptions = new UfbRoomJoinOptions {
                displayName = "Player",
                characterId = "kirin"
            };
            ServiceLocator.Current.Get<GameService>().JoinGame(idInputField.text, joinOptions);
        }

        public void OnBackButton() => _menuManager.CloseMenu();
    }
}
