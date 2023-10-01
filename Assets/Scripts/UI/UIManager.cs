using UnityEngine.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UFB.Events;
using UFB.Network.RoomMessageTypes;

namespace UFB.UI
{
    public class UIManager : MonoBehaviour
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
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<ToastMessageEvent>(OnToastMessageEvent);
            EventBus.Unsubscribe<RoomReceieveMessageEvent<NotificationMessage>>(
                OnRoomNotificationMessage
            );
            EventBus.Unsubscribe<SelectedCharacterEvent>(OnSelectedCharacterEvent);
        }

        private void OnSelectedCharacterEvent(SelectedCharacterEvent e) =>
            _characterController = e.controller;

        private void InstantiatePanel(AssetReference asset, System.Action<GameObject> callback)
        {
            Addressables.InstantiateAsync(asset, RootCanvas.transform).Completed += (obj) =>
            {
                if (
                    obj.Status
                    == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded
                )
                {
                    callback(obj.Result);
                }
                else
                {
                    throw new System.Exception($"Failed to instantiate {asset}");
                }
            };
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
