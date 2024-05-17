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
        public InteractionMode Mode { get; private set; }
        private UnityEngine.Camera _mainCamera;
        private GameInput _gameInput;
        private CameraController _cameraController;

        private bool _isOrbitLocked = false;
        
        [SerializeField]
        private GameObject resourcePanel;

        [SerializeField]
        UIGameManager uiGameManager;

        private bool isSpawn = true;

        private void Awake()
        {
#if UNITY_EDITOR
            if (!Mouse.current.enabled)
            {
                InputSystem.EnableDevice(Mouse.current);
            }
#endif

            _gameInput = new GameInput();
            _mainCamera = UnityEngine.Camera.main;
            _cameraController = _mainCamera.GetComponent<CameraController>();
        }

        private void OnEnable()
        {
            Events.EventBus.Subscribe<InteractionModeChangeEvent>(OnInteractionModeChangeEvent);
            Events.EventBus.Publish(new InteractionModeChangeEvent(InteractionMode.SelectItem));
            ServiceLocator.Current.Register(this);
            // _gameInput.OrbitView.TouchPress.started += OnTouchPressStarted;
            // _gameInput.OrbitView.MouseClick.started += OnMouseClickStarted;
            _gameInput.OrbitView.OrbitCamera.performed += OnOrbitCamera;
            _gameInput.OrbitView.ScrollZoom.performed += OnScrollZoom;
            _gameInput.OrbitView.TapSelect.performed += OnTapSelect;
            _gameInput.Enable();
        }

        private void OnDisable()
        {
            Events.EventBus.Unsubscribe<InteractionModeChangeEvent>(OnInteractionModeChangeEvent);
            ServiceLocator.Current.Unregister<InteractionManager>();
            // _gameInput.OrbitView.TouchPress.started -= OnTouchPressStarted;
            // _gameInput.OrbitView.MouseClick.started -= OnMouseClickStarted;
            _gameInput.OrbitView.OrbitCamera.performed -= OnOrbitCamera;
            _gameInput.OrbitView.ScrollZoom.performed -= OnScrollZoom;
            _gameInput.OrbitView.TapSelect.performed -= OnTapSelect;
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

        // we're going to need to think of some way to toggle the orbit on and off
        // because in top down mode, we need a different control
        // we could consider having the camera controller add/remove the monobehavior
        // responsivle for control, then we send the controller a simple Control
        // or TemporaryControl message
        private void OnOrbitCamera(InputAction.CallbackContext ctx)
        {
            if (_gameInput.OrbitView.PrimaryPress.ReadValue<float>() < 0.5f)
            {
                return;
            }

            if (_isOrbitLocked)
            {
                _cameraController.Orbit.RotateTemporary(ctx.ReadValue<Vector2>() * 3f, 2f);
            }
            else
            {
                _cameraController.Orbit.Rotate(ctx.ReadValue<Vector2>() * 3f);
            }
        }

        private void OnScrollZoom(InputAction.CallbackContext ctx)
        {
            if (_isOrbitLocked)
            {
                _cameraController.Orbit.ChangeRadiusTemporary(ctx.ReadValue<Vector2>().y * 0.01f, 2f);
            }
            else
            {
                _cameraController.Orbit.ChangeRadius(ctx.ReadValue<Vector2>().y * 0.01f);
            }
        }

        private void RaycastObjects(Vector2 position)
        {
            // Convert mouse position to ray
            Vector3 position3D = new Vector3(position.x, position.y, -4f);
            Ray ray = _mainCamera.ScreenPointToRay(position3D);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.TryGetComponent<IClickable>(out var clickable))
                {
                    OnRaycastClicked(hit.transform, clickable);
                }
            }
        }

        private void RaycastObjects(Vector3 position)
        {
            Ray ray = _mainCamera.ScreenPointToRay(position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.TryGetComponent<IClickable>(out var clickable))
                {
                    OnRaycastClicked(hit.transform, clickable);
                }
            }
        }

        private void OnRaycastClicked(Transform transform, IClickable clickable)
        {
            if (Mode != InteractionMode.SelectItem)
            {
                return;
            }

            string playerId = ServiceLocator.Current.Get<CharacterManager>().PlayerCharacter.Id;

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
                            resourcePanel.gameObject.SetActive(true);
                        }
                    }


                    if (item.GetComponent<Chest>() != null && isSpawn)
                    {
                        item.GetComponent<Chest>().OnClick();

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
                        isSpawn = false;
                    }


                }
            }

            /*if (transform.childCount > 0)
            {
                if(transform.GetChild(0).TryGetComponent(out CharacterController character))
                {
                    if(stepPanel != null && character != null && character.Id == playerId)
                    {
                        stepPanel.gameObject.SetActive(true);
                    }
                }
                if (transform.GetChild(0).TryGetComponent(out Chest chest))
                {
                    if (stepPanel != null && chest != null)
                    {
                        Debug.Log("Click chest");
                        chest.OnClick();
                    }
                }
                if (transform.GetChild(0).TryGetComponent(out Merchant merchant))
                {
                    if (stepPanel != null && merchant != null)
                    {
                        Debug.Log("Click merchant");
                        merchant.OnClick();
                    }
                }
            }*/

            var turnOrder = ServiceLocator.Current.Get<GameService>().RoomState.turnOrder;
            Debug.Log(turnOrder.Serialize());

            /*            new RoomSendMessageEvent(
                            "move",
                            new RequestMoveMessage
                            {
                                tileId = transform.GetComponent<Tile>().Id,
                                destination = transform
                                    .GetComponent<Tile>()
                                    .Coordinates
                            }
                        );*/


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
    }
}
