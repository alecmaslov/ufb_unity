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
        public AssetReference toastPrefab;

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

            Debug.Log($"UIManager enabled");
            var currentScene = SceneManager.GetActiveScene();
            Debug.Log($"Current scene: {currentScene.name}");

            // subscribe to anything here
            EventBus.Subscribe<ToastMessageEvent>(ShowToast);
            EventBus.Subscribe<RoomReceieveMessageEvent<NotificationMessage>>(ShowToast);
        }

        private void OnDisable()
        {
            // unsubscribe to anything here
            EventBus.Unsubscribe<ToastMessageEvent>(ShowToast);
            EventBus.Subscribe<RoomReceieveMessageEvent<NotificationMessage>>(ShowToast);
        }

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
            InstantiatePanel(
                toastPrefab,
                (obj) =>
                {
                    obj.GetComponent<UIToast>().Initialize(message);
                }
            );
        }

        private void ShowToast(ToastMessageEvent messageEvent)
        {
            InstantiatePanel(
                toastPrefab,
                (obj) =>
                {
                    obj.GetComponent<UIToast>().Initialize(messageEvent);
                }
            );
        }

        private void ShowToast(RoomReceieveMessageEvent<NotificationMessage> messageEvent)
        {
            InstantiatePanel(
                toastPrefab,
                (obj) =>
                {
                    obj.GetComponent<UIToast>().Initialize(messageEvent.Message.message);
                }
            );
        }
    }
}


// private void Awake()
// {
//     Debug.Log($"UIManager awake");
//     if (Instance != null)
//     {
//         Destroy(gameObject);
//     }
//     Instance = this;
//     DontDestroyOnLoad(gameObject);
//     _rootCanvas = GetComponent<Canvas>();
//     var currentScene = SceneManager.GetActiveScene();
//     Debug.Log($"Current scene: {currentScene.name}");
// }

// [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
// private static void Initialize()
// {
//     if (Instance != null)
//     {
//         return;
//     }
//     var uiManagerGameObject = new GameObject("UIManager");
//     Instance = uiManagerGameObject.AddComponent<UIManager>();
//     DontDestroyOnLoad(uiManagerGameObject);
// }
