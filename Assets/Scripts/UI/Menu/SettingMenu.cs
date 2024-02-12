using TMPro;

namespace UFB.UI
{
    public class SettingMenu : Menu
    {
        

        private void OnEnable()
        {
            
        }

        public void OnBackButton()
        {
            _menuManager.CloseMenu();
        }
    }
}
