using UnityEngine;
using UFB.Events;
using UFB.Map;
using UFB.Core;
using UFB.UI;
using UFB.Network.RoomMessageTypes;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UFB.Input;
using UFB.Camera;
using UFB.Character;
using CharacterController = UFB.Character.CharacterController;
using UFB.Entities;
using UnityEngine.TextCore.Text;

namespace UFB.Events
{
    public class InteractionModeChangeEvent
    {
        public Interactions.InteractionMode Mode { get; }

        public InteractionModeChangeEvent(Interactions.InteractionMode mode)
        {
            Mode = mode;
        }
    }
}

namespace UFB.Interactions
{
    public enum InteractionMode
    {
        FocusEntity,
        SelectItem,
        CameraControl
    }

    public interface IInteractionController
    {
        void OnClick();
        void OnFocus();
        void OnUnfocus();
    }

    // maybe an InteractionManager coordinates this stuff, and an interactionController
    // determines how that interaction occurs. When a certain mode is activated, the interactionController
    // is called to handle the interaction

    // Controls state of game interactivity, allowing users to be in different states/modes of interaction
    // such as selecting a tile, focusing an entity, or controlling the camera

    public class InteractionManager : MonoBehaviour, IService
    {
        public static InteractionManager Instance;
        public InteractionMode Mode { get; private set; }
        private GameInput _gameInput;

        private bool _isOrbitLocked = false;
        
        [SerializeField]
        private GameObject resourcePanel;

        public bool isSpawn = true;


        private void Awake()
        {
#if UNITY_EDITOR
            if (!Mouse.current.enabled)
            {
                InputSystem.EnableDevice(Mouse.current);
            }
#endif

            _gameInput = new GameInput();
            Instance = this;
        }

        private void OnEnable()
        {
            Events.EventBus.Subscribe<InteractionModeChangeEvent>(OnInteractionModeChangeEvent);
            Events.EventBus.Publish(new InteractionModeChangeEvent(InteractionMode.SelectItem));
            ServiceLocator.Current.Register(this);
            _gameInput.OrbitView.TouchPress.canceled += OnTapSelect;
            // _gameInput.OrbitView.MouseClick.started += OnMouseClickStarted;
            //_gameInput.OrbitView.TapSelect.performed += OnTapSelect;
            //_gameInput.OrbitView.MultitouchHold.started += MultitouchHold_started;
            _gameInput.Enable();
        }

        private float doubleTouchTime = 0.3f; // Time window for double touch
        private float lastTouchTime = 0f;      // Time of the last touch

        private void MultitouchHold_started(InputAction.CallbackContext obj)
        {

            Vector2 pointerPosition = _gameInput.OrbitView.PointerPosition.ReadValue<Vector2>();
            Vector3 position3D = new Vector3(pointerPosition.x, pointerPosition.y, -4f);
            Ray ray = CameraManager.instance.cam.ScreenPointToRay(position3D);
            RaycastHit hit;

            Debug.Log("Double clicked");

            // Check if the click is on a UI element
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.TryGetComponent<IClickable>(out var clickable))
                    {
                        OnRaycastDBClicked(hit.transform, clickable);
                    }
                }
            }

        }

        private void OnDisable()
        {
            Events.EventBus.Unsubscribe<InteractionModeChangeEvent>(OnInteractionModeChangeEvent);
            ServiceLocator.Current.Unregister<InteractionManager>();
            _gameInput.OrbitView.TouchPress.started -= OnTapSelect;
            // _gameInput.OrbitView.MouseClick.started -= OnMouseClickStarted;
            //_gameInput.OrbitView.TapSelect.performed -= OnTapSelect;
            //_gameInput.OrbitView.MultitouchHold.started -= OnTapSelect;
            _gameInput.Disable();
        }

        bool isTileClicked = false;


        private void OnTapSelect(InputAction.CallbackContext ctx)
        {

            CameraManager.instance.OnUIClicked();

            if (CameraManager.instance.IsUIClicked || CameraManager.instance.isMovingCamera) return;

            // Check if the time since the last touch is within the double touch time window
            if (Time.time - lastTouchTime < doubleTouchTime)
            {
                // Double touch detected
                MultitouchHold_started(ctx);
                lastTouchTime = Time.time;
                return;
            }

            // Update the time of the last touch
            lastTouchTime = Time.time;

            isTileClicked = false;

            Debug.Log(
                $"Tap select performed | {ctx.ReadValue<float>()} | {_gameInput.OrbitView.PointerPosition.ReadValue<Vector2>()}"
            );
            // int pointerId = Touchscreen.current.primaryTouch.touchId.ReadValue();
            if (EventSystem.current.IsPointerOverGameObject(-1))
            {
                Debug.Log("Pointer is over UI");
                return;
            }
            Vector2 pointerPosition = _gameInput.OrbitView.PointerPosition.ReadValue<Vector2>();
            Debug.Log($"Touch position: {pointerPosition.x} {pointerPosition.y}");
            RaycastObjects(pointerPosition);

            if (isTileClicked)
            {
                UIGameManager.instance.bottomDrawer.OpenBottomDrawer();
                if (pointerPosition.y < Screen.height / 2 - 80)
                {
                    CameraManager.instance.cameraTarget.position = hitTilePos;
                }
            }
        }


        private void RaycastObjects(Vector2 position)
        {
            // Convert mouse position to ray
            Vector3 position3D = new Vector3(position.x, position.y, -4f);
            Ray ray = CameraManager.instance.cam.ScreenPointToRay(position3D);
            RaycastHit hit;

            // Check if the click is on a UI element
            if(!EventSystem.current.IsPointerOverGameObject())
            {
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.TryGetComponent<IClickable>(out var clickable))
                    {
                        OnRaycastClicked(hit.transform, clickable);
                    }
                }
            }
        }


        private void OnRaycastClicked(Transform transform, IClickable clickable)
        {
            if (Mode != InteractionMode.SelectItem)
            {
                return;
            }

            string playerId = CharacterManager.Instance.SelectedCharacter.Id;

            string tileId = transform.GetComponent<Tile>().Id;
            Tile tile = ServiceLocator.Current.Get<GameBoard>().Tiles[tileId];


            if (tile != null)
            {
                for (int i = 0; i < tile.transform.childCount; i++)
                {
                    GameObject item = tile.transform.GetChild(i).gameObject;
                    
                    if (item.GetComponent<CharacterController>() != null && !isSpawn && UIGameManager.instance.TopPanel.activeSelf)
                    {
                        if (resourcePanel != null && item.GetComponent<CharacterController>().Id == playerId)
                        {
                            //resourcePanel.gameObject.SetActive(true);
                        }
                    }


                    if (item.GetComponent<Chest>() != null && isSpawn)
                    {
                        if(!item.GetComponent<Chest>().isItemBag)
                        {
                            item.GetComponent<Chest>().OnClick();

                            UIGameManager.instance.selectSpawnPanel.InitSpawnData(tile, playerId);
                            
                        }
                    }

                }
                Debug.Log("xxxx: xx: xxx");
                if (!isSpawn/* && uiGameManager.isMoveTileStatus*/ && UIGameManager.instance.isPlayerTurn)
                {
                    if (UIGameManager.instance.bottomDrawer.IsExpanded) 
                    {
                        UIGameManager.instance.StepPanel.SetHighLightBtn(true);
                        UIGameManager.instance.equipPanel.ClearHighLightItems();
                        
                        UIGameManager.instance.bottomDrawer.CloseBottomDrawer();
                        HighlightRect.Instance.ClearHighLightRect();
                    }
                    else
                    {
                        UIGameManager.instance.movePanel.OnClickTile(tile);
                        hitTilePos = tile.Position;
                        isTileClicked = true;
                    }

                }
            }
        }

        Vector3 hitTilePos = Vector3.zero;
        
        private void OnRaycastDBClicked(Transform transform, IClickable clickable)
        {
            if (Mode != InteractionMode.SelectItem)
            {
                return;
            }

            string playerId = CharacterManager.Instance.SelectedCharacter.Id;

            string tileId = transform.GetComponent<Tile>().Id;
            Tile tile = ServiceLocator.Current.Get<GameBoard>().Tiles[tileId];

            if (tile != null)
            {
                Debug.Log("xxxx: xx: xxx");
                if (!isSpawn)
                {
                    for (int i = 0; i < tile.transform.childCount; i++)
                    {
                        GameObject item = tile.transform.GetChild(i).gameObject;

                        if (item.GetComponent<CharacterController>() != null)
                        {
                            CharacterController controller = item.GetComponent<CharacterController>();
                            UIGameManager.instance.ResourcePanel.OnCharacterValueEvent(controller.State);
                            UIGameManager.instance.ResourcePanel.InitButtonStatus();
                            UIGameManager.instance.ResourcePanel.gameObject.SetActive(true);
                        }
                    }
                }
            }

        }

        public void ToggleLockOrbit()
        {
            Debug.Log($"Toggling lock orbit {_isOrbitLocked}");
            _isOrbitLocked = !_isOrbitLocked;
        }

        public void CycleInteractionMode()
        {
            switch (Mode)
            {
                case InteractionMode.FocusEntity:
                    Events.EventBus.Publish(
                        new InteractionModeChangeEvent(InteractionMode.SelectItem)
                    );
                    break;
                case InteractionMode.SelectItem:
                    Events.EventBus.Publish(
                        new InteractionModeChangeEvent(InteractionMode.CameraControl)
                    );
                    break;
                case InteractionMode.CameraControl:
                    Events.EventBus.Publish(
                        new InteractionModeChangeEvent(InteractionMode.FocusEntity)
                    );
                    break;
            }
        }

        private void OnInteractionModeChangeEvent(InteractionModeChangeEvent e)
        {
            Mode = e.Mode;
        }

        private void Update()
        {
            //isMoveDirection = UIGameManager.instance.movePanel.globalDirection.gameObject.activeSelf;
        }
    }
}
