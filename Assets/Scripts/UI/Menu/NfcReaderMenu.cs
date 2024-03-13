using UnityEngine.SceneManagement;

namespace UFB.UI
{
    public class NfcReaderMenu : Menu
    {
        // public Menu mainMenu;
     
        private void OnEnable()
        {
            // if (_menuManager.GetMenuData("joinOptions") == null)
            // {
            //     _menuManager.SetMenuData("joinOptions", new UfbRoomJoinOptions());
            // }

            // if (_menuManager.GetMenuData("createOptions") == null)
            // {
            //     _menuManager.SetMenuData("createOptions", new UfbRoomCreateOptions());
            // }

            // characterSelector.OnSelectionChanged += OnCharacterSelectionChanged;
            // mapSelector.OnSelectionChanged += OnMapSelectionChanged;
        }

        private void OnDisable()
        {
            // characterSelector.OnSelectionChanged -= OnCharacterSelectionChanged;
            // mapSelector.OnSelectionChanged -= OnMapSelectionChanged;
        }

        public void OnBackButton() => SceneManager.LoadScene("MainMenu");
    }
}
