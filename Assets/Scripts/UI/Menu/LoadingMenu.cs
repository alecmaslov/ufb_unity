using TMPro;

namespace UFB.UI
{
    public class LoadingMenu : Menu
    {
        public Menu mainMenu;
        public TextMeshProUGUI loadingStatusText;

        private void OnEnable()
        {
            loadingStatusText.text = "Loading...";
        }
    }
}
