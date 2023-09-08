using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace UFB.UI
{
    public class CreateGameMenu : Menu
    {
        public Menu mainMenu;
        public ProgressBarPanel progressBarPanel;

        public void OnEnable()
        {
            progressBarPanel.gameObject.SetActive(false);
        }

        public void OnBackButton() => _menuManager.OpenMenu(mainMenu);

        public void OnMapButton() => Debug.Log("Map button hit!");

        public void OnPlayerButton() => Debug.Log("Player button hit!");

        public void OnRulesButton() => Debug.Log("Rules button hit!");

        public void OnStartButton()
        {
            Debug.Log("Start button hit!");

            AsyncOperation op = SceneManager.LoadSceneAsync("Main");

            progressBarPanel.gameObject.SetActive(true);
            progressBarPanel.LoadAsyncOperation(op);
            
        }
    }
}
