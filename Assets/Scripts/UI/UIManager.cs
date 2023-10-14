using UnityEngine.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UFB.Events;
using UFB.Network.RoomMessageTypes;
using UFB.Core;
using UFB.UI;

namespace UFB.Events
{
    public class PopupMenuEvent
    {
        public class CreateButton
        {
            public string text;
            public System.Action onClick;

            public CreateButton(string text, System.Action onClick)
            {
                this.text = text;
                this.onClick = onClick;
            }
        }

        public string title;
        public Vector3 positionOffset = new(0, 2, 0);
        public Transform target;
        public System.Action onCancel;
        public CreateButton[] buttons;

        public PopupMenuEvent(
            string title,
            Transform target,
            System.Action onCancel,
            CreateButton[] buttons
        )
        {
            this.title = title;
            this.target = target;
            this.onCancel = onCancel;
            this.buttons = buttons;
        }
    }
}

namespace UFB.UI
{
    public class UIManager : MonoBehaviour, IService
    {
        // public AssetReference toastPrefab;
        public GameObject toastPrefab;

        public Canvas RootCanvas
        {
            get
            {
                if (_rootCanvas == null)
                {
                    _rootCanvas = GetComponentInChildren<Canvas>();
                    if (_rootCanvas == null)
                    {
                        throw new System.Exception("UIManager requires a Canvas component");
                    }
                }
                return _rootCanvas;
            }
        }

        [SerializeField]
        private Canvas _rootCanvas;

        [SerializeField]
        private RectTransform _topSlot;

        [SerializeField]
        private RectTransform _middleSlot;

        [SerializeField]
        private RectTransform _bottomSlot;

        [SerializeField]
        private GameObject _popupMenuScreenPrefab;

        [SerializeField]
        private GameObject _popupMenuWorldPrefab;

        private PopupMenu _currentPopupMenu;

        private Character.CharacterController _characterController;

        private void OnEnable()
        {
            if (_rootCanvas == null)
            {
                _rootCanvas = GetComponentInChildren<Canvas>();
                if (_rootCanvas == null)
                {
                    throw new System.Exception("UIManager requires a Canvas component");
                }
            }

            EventBus.Subscribe<ToastMessageEvent>(OnToastMessageEvent);
            EventBus.Subscribe<RoomReceieveMessageEvent<NotificationMessage>>(
                OnRoomNotificationMessage
            );
            EventBus.Subscribe<SelectedCharacterEvent>(OnSelectedCharacterEvent);
            EventBus.Subscribe<PopupMenuEvent>(OnPopupMenuEvent);

            ServiceLocator.Current.Register(this);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<ToastMessageEvent>(OnToastMessageEvent);
            EventBus.Unsubscribe<RoomReceieveMessageEvent<NotificationMessage>>(
                OnRoomNotificationMessage
            );
            EventBus.Unsubscribe<SelectedCharacterEvent>(OnSelectedCharacterEvent);
            EventBus.Unsubscribe<PopupMenuEvent>(OnPopupMenuEvent);
            ServiceLocator.Current.Unregister<UIManager>();
        }

        private void OnSelectedCharacterEvent(SelectedCharacterEvent e) =>
            _characterController = e.controller;

        public void OnPopupMenuEvent(PopupMenuEvent e)
        {
            if (_currentPopupMenu != null)
                Destroy(_currentPopupMenu.gameObject);
            _currentPopupMenu = Instantiate(_popupMenuWorldPrefab, transform)
                .GetComponent<PopupMenu>();
            _currentPopupMenu.Initialize(e);
        }

        public void ShowToast(string message)
        {
            var toast = Instantiate(toastPrefab, RootCanvas.transform);
            toast.GetComponent<UIToast>().Initialize(message);
        }

        private void OnToastMessageEvent(ToastMessageEvent messageEvent) =>
            ShowToast(messageEvent.Message);

        private void OnRoomNotificationMessage(
            RoomReceieveMessageEvent<NotificationMessage> messageEvent
        ) => ShowToast(messageEvent.Message.message);
    }
}
