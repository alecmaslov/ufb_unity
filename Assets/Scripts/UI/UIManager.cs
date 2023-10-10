using UnityEngine.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UFB.Events;
using UFB.Network.RoomMessageTypes;
using SchemaTest.InheritedTypes;
using UFB.Core;

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
        private GameObject _entityPopupMenuPrefab;

        private GameObject _entityPopupMenu;

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

            ServiceLocator.Current.Register(this);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<ToastMessageEvent>(OnToastMessageEvent);
            EventBus.Unsubscribe<RoomReceieveMessageEvent<NotificationMessage>>(
                OnRoomNotificationMessage
            );
            EventBus.Unsubscribe<SelectedCharacterEvent>(OnSelectedCharacterEvent);

            ServiceLocator.Current.Unregister<UIManager>();
        }

        private void OnSelectedCharacterEvent(SelectedCharacterEvent e) =>
            _characterController = e.controller;

        public void ShowEntityPopupMenu(GameObject entity, Vector3 screenPosition)
        {
            if (_entityPopupMenu != null)
            {
                Destroy(_entityPopupMenu);
            }
            _entityPopupMenu = Instantiate(_entityPopupMenuPrefab, RootCanvas.transform);
            _entityPopupMenu.transform.position = screenPosition;
            var popupMenu = _entityPopupMenu.GetComponent<EntityPopupMenu>();
            popupMenu.Initialize(entity);
            // popupMenu.CreateButton(
            //     "Move",
            //     () => _characterController.MoveTo(entity.transform.position)
            // );
            popupMenu.CreateButton("Cancel", () => Destroy(_entityPopupMenu));
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
