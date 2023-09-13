using UnityEngine.UI;
using UnityEngine;

namespace UFB.UI
{   

    public interface IUIPanel
    {
        void Show();
        void Hide();
    }

    public class UIManager
    {
        public static UIToast Toast { get; private set; }

        private Canvas _uiRoot;
        private MonoBehaviour _monobehaviour;


        public UIManager(MonoBehaviour monobehaviour)
        {
            // here we can get or create the uiRoot

            _monobehaviour = monobehaviour;
            Toast = new UIToast();
        }


        // public void 

    }
}