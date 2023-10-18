using UFB.Network.RoomMessageTypes;
using TMPro;
using UFB.Core;
using UFB.Character;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System;
using UnityEngine;
using UFB.Events;
using UnityEngine.UI;

namespace UFB.UI
{
    public class JoinGameMenu : Menu
    {
        public Menu loadingMenu;
        public TMP_InputField idInputField;
        public CharacterSelector characterSelector;

        [SerializeField]
        private Button _joinButton;

        [SerializeField]
        private GameObject _uiContainer;

        [SerializeField]
        private TextMeshProUGUI _roomIdText;

        [SerializeField]
        private GameObject _roomIdDisplay;

        private string _roomId;

        private void OnEnable()
        {
            if (_menuManager.GetMenuData("joinOptions") == null)
            {
                _menuManager.SetMenuData("joinOptions", new UfbRoomJoinOptions());
            }

            if (_menuManager.GetMenuData("createOptions") == null)
            {
                _menuManager.SetMenuData("createOptions", new UfbRoomCreateOptions());
            }
            idInputField.text = "";
            characterSelector.OnSelectionChanged += OnCharacterSelectionChanged;

            _joinButton.interactable = false;
            _uiContainer.SetActive(true);
            _roomIdDisplay.SetActive(false);
        }

        private void OnDisable()
        {
            characterSelector.OnSelectionChanged -= OnCharacterSelectionChanged;
        }

        public void OnJoinButton()
        {
            _menuManager.OpenMenu(loadingMenu);
            var joinOptions = new UfbRoomJoinOptions
            {
                displayName = "Player",
                characterId = "kirin"
            };
            ServiceLocator.Current
                .Get<GameService>()
                .JoinGame(
                    _roomId,
                    joinOptions,
                    () => // on error
                    {
                        _menuManager.CloseMenu();
                        EventBus.Publish(
                            new ToastMessageEvent(
                                $"Error joining room {_roomId}",
                                UIToast.ToastType.Error
                            )
                        );
                    }
                );
        }

        public void OnScanQRButton()
        {
            SceneManager.LoadSceneAsync("QRScanner", LoadSceneMode.Additive).completed += (op) =>
            {
                _uiContainer.SetActive(false);
                Debug.Log("QRScanner loaded");

                QRCodeDecodeController qrDecoder = FindObjectOfType<QRCodeDecodeController>();
                qrDecoder.onQRScanFinished += OnQRScanFinished;
            };
        }

        private void OnQRScanFinished(string dataText)
        {
            Debug.Log("QR code scanned: " + dataText);
            _roomId = dataText;

            SceneManager.UnloadSceneAsync("QRScanner").completed += (op) =>
            {
                _uiContainer.SetActive(true);

                if (_roomId != null)
                {
                    _roomIdDisplay.SetActive(true);
                    _roomIdText.text = _roomId;
                    _joinButton.interactable = true;
                }
            };
        }

        public void OnCharacterSelectionChanged(UfbCharacter character)
        {
            var joinOptions = _menuManager.GetMenuData("joinOptions") as UfbRoomJoinOptions;
            joinOptions.characterClass = character.characterClass;
            _menuManager.SetMenuData("joinOptions", joinOptions);
        }

        public void OnBackButton() => _menuManager.CloseMenu();
    }
}
