using UnityEngine;
using UFB.Events;
using UFB.Map;
using UFB.Core;
using UFB.UI;
using UFB.Network.RoomMessageTypes;
using Unity.VisualScripting;
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

        [SerializeField]
        UIGameManager uiGameManager;

        public bool isSpawn = true;

        private bool isMoveDirection = false;

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
            _gameInput.OrbitView.TouchPress.started += OnTapSelect;
            // _gameInput.OrbitView.MouseClick.started += OnMouseClickStarted;
            //_gameInput.OrbitView.TapSelect.performed += OnTapSelect;
            _gameInput.Enable();
        }

        private void OnDisable()
        {
            Events.EventBus.Unsubscribe<InteractionModeChangeEvent>(OnInteractionModeChangeEvent);
            ServiceLocator.Current.Unregister<InteractionManager>();
            _gameInput.OrbitView.TouchPress.started -= OnTapSelect;
            // _gameInput.OrbitView.MouseClick.started -= OnMouseClickStarted;
            //_gameInput.OrbitView.TapSelect.performed -= OnTapSelect;
            _gameInput.Disable();
        }

        private void OnTapSelect(InputAction.CallbackContext ctx)
        {
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
            if (isMoveDirection) 
            { 
                clickable.OnClick();
                //return;
            }

            string playerId = CharacterManager.Instance.SelectedCharacter.Id;

            string tileId = transform.GetComponent<Tile>().Id;
            Tile tile = ServiceLocator.Current.Get<GameBoard>().Tiles[tileId];

            if (tile != null)
            {
                if(isSpawn)
                {
                    Events.EventBus.Publish(
                        new CameraOrbitAroundEvent(transform, 0.3f)
                    );
                }


                for (int i = 0; i < tile.transform.childCount; i++)
                {
                    GameObject item = tile.transform.GetChild(i).gameObject;
                    
                    if (item.GetComponent<CharacterController>() != null && !isSpawn && uiGameManager.TopPanel.activeSelf)
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
                            HighlightRect.Instance.ClearHighLightRect();
                            Events.EventBus.Publish(
                                RoomSendMessageEvent.Create(
                                    "initSpawnMove",
                                    new RequestSpawnMessage
                                    {
                                        tileId = tile.Id,
                                        destination = tile.Coordinates,
                                        playerId = playerId,
                                    }
                                )
                            );
                            uiGameManager.controller.InitMovePos(tile);

                            ServiceLocator.Current.Get<CharacterManager>().PlayerCharacter.gameObject.SetActive(true);
                            Events.EventBus.Publish(
                                new CameraOrbitAroundEvent(
                                    ServiceLocator.Current.Get<CharacterManager>().PlayerCharacter.transform,
                                    0.3f
                                )
                            );
                            CameraManager.instance.SetTarget(uiGameManager.controller.transform);
                            CameraManager.instance.OnActiveInputAction();
                            isSpawn = false;
                        }

                    }


                }
                Debug.Log("xxxx: xx: xxx");
                if (!isSpawn && uiGameManager.isMoveTileStatus)
                {
                    UIGameManager.instance.movePanel.OnClickTile(tile);
                }
            }


            var turnOrder = ServiceLocator.Current.Get<GameService>().RoomState.turnOrder;
            Debug.Log(turnOrder.Serialize());

            ServiceLocator.Current
                .Get<UIManager>()
                .OnPopupMenuEvent(
                    new PopupMenuEvent(
                        transform.name,
                        transform,
                        () => Debug.Log("Calling Cancel"),
                        new PopupMenuEvent.CreateButton[]
                        {
                            new(
                                "Move To",
                                () => {
                                    Events.EventBus.Publish(
                                        new RoomSendMessageEvent(
                                            "move",
                                            new RequestMoveMessage
                                            {
                                                tileId = transform.GetComponent<Tile>().Id,
                                                destination = transform
                                                    .GetComponent<Tile>()
                                                    .Coordinates
                                            }
                                        )
                                    );
                                    Events.EventBus.Publish(new CancelPopupMenuEvent());
                                }
                            ),
                            new(
                                "Focus",
                                () =>
                                    Events.EventBus.Publish(
                                        new CameraOrbitAroundEvent(transform, 0.3f)
                                    )
                            )
                            // we should make CreateButton a bit more feature rich, so
                            // we can have certain types of buttons with certain icons
                            // new("Move To", () => UFB.Events.EventBus.Publish<)
                        }
                    )
                );
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
                    UFB.Events.EventBus.Publish(
                        new InteractionModeChangeEvent(InteractionMode.SelectItem)
                    );
                    break;
                case InteractionMode.SelectItem:
                    UFB.Events.EventBus.Publish(
                        new InteractionModeChangeEvent(InteractionMode.CameraControl)
                    );
                    break;
                case InteractionMode.CameraControl:
                    UFB.Events.EventBus.Publish(
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
            isMoveDirection = UIGameManager.instance.movePanel.globalDirection.gameObject.activeSelf;
        }
    }
}
