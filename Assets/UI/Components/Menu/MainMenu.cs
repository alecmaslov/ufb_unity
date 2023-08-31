using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

    // <Style src="project://database/Assets/UI/MainMenuStylesheet.uss?fileID=7433441132597879392&amp;guid=2b62bae8e92f278439931192773afb9d&amp;type=3#MainMenuStylesheet" />


namespace UFB.UI
{

    [RequireComponent(typeof(UIDocument))]
    public class MainMenu : MonoBehaviour
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