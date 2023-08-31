using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UFB.UI
{

    public interface IUIScreen
    {
        VisualTreeAsset screenLayout { get; set; }

        void Show();
        void Hide();

    }


    public class UIScreenManager : MonoBehaviour
    {
        [Header("Screens")]
        public VisualTreeAsset mainMenuScreen;
        public VisualTreeAsset settingsScreen;
        public VisualTreeAsset gameOverScreen;
        // ... any other screens you have

        private VisualElement currentScreen;
        private VisualElement rootUI;

        private void Start()
        {
            rootUI = GetComponent<UIDocument>().rootVisualElement;
        }

        public void ShowScreen(VisualTreeAsset screenAsset)
        {
            if (currentScreen != null)
            {
                rootUI.Remove(currentScreen);
            }

            currentScreen = screenAsset.CloneTree().contentContainer;
            rootUI.Add(currentScreen);
        }
    }

}