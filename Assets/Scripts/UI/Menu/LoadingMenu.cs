using TMPro;

namespace UFB.UI
{
    public class LoadingMenu : Menu
    {
        public TextMeshProUGUI loadingStatusText;

        private void OnEnable()
        {
            loadingStatusText.text = "Loading...";
        }
    }
}
