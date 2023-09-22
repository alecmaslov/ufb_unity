using UnityEngine.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UFB.Events;


namespace UFB.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        public AssetReference toastPrefab;
        [SerializeField] private Canvas _rootCanvas;

        [SerializeField] private RectTransform _topSlot;
        [SerializeField] private RectTransform _middleSlot;
        [SerializeField] private RectTransform _bottomSlot;


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
            Instance = this;
            var currentScene = SceneManager.GetActiveScene();
            Debug.Log($"Current scene: {currentScene.name}");

            // subscribe to anything here
            EventBus.Subscribe<ToastMessageEvent>(ShowToast);
        }

        private void OnDisable()
        {
            // unsubscribe to anything here
            EventBus.Unsubscribe<ToastMessageEvent>(ShowToast);
        }

        private void InstantiatePanel(AssetReference asset, System.Action<GameObject> callback)
        {
            Addressables.InstantiateAsync(asset, _rootCanvas.transform).Completed += (obj) =>
            {
                if (obj.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
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
            InstantiatePanel(toastPrefab, (obj) =>
            {
                obj.GetComponent<UIToast>().Initialize(message);
            });
        }

        private void ShowToast(ToastMessageEvent messageEvent)
        {
            InstantiatePanel(toastPrefab, (obj) =>
            {
                obj.GetComponent<UIToast>().Initialize(messageEvent);
            });
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