using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UFB.UI
{

    [RequireComponent(typeof(UIDocument))]
    public class MainMenuManager : MonoBehaviour
    {
        public void Start()
        {
            UIDocument document = GetComponent<UIDocument>();
            VisualTreeAsset mainMenuTemplate = Resources.Load<VisualTreeAsset>("MainMenu");

            var mainMenuContainer = mainMenuTemplate.Instantiate();

            // Initialize each button
            var startButton = mainMenuContainer.Q<MainMenuButton>("start-button");
            startButton.Init("Start Game", () =>
            {
                // Logic to start game
            });

            var settingsButton = mainMenuContainer.Q<MainMenuButton>("settings-button");
            settingsButton.Init("Settings", () =>
            {
                // Logic to go to settings
            });

            var quitButton = mainMenuContainer.Q<MainMenuButton>("quit-button");
            quitButton.Init("Quit", () =>
            {
                // Logic to quit game
            });

            document.rootVisualElement.Add(mainMenuContainer);
        }
    }
}