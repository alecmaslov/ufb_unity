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
using System.Collections;
using System;

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

        private bool _isOrbiting = false;
        private Coroutine _resetOrbitFlagCoroutine;

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


        private void OnEnable() =>  EventBus.Subscribe<GameReadyEvent>(OnGameReadyEvent);

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameReadyEvent>(OnGameReadyEvent);
            EventBus.Unsubscribe<InteractionModeChangeEvent>(OnInteractionModeChangeEvent);
            // ServiceLocator.Current.Unregister<InteractionManager>();
            _gameInput.OrbitView.OrbitCamera.performed -= OnOrbitCamera;
            _gameInput.OrbitView.ScrollZoom.performed -= OnScrollZoom;
            _gameInput.OrbitView.SingleTapSelect.performed -= OnSingleTapSelect;
            _gameInput.OrbitView.DoubleTapSelect.performed -= OnDoubleTapSelect;
            _gameInput.OrbitView.TapHoldSelect.performed -= OnTapHoldSelect;
            _gameInput.Disable();
        }


        private void OnGameReadyEvent(GameReadyEvent e)
        {
            ServiceLocator.Current.Register(this);
            EventBus.Subscribe<InteractionModeChangeEvent>(OnInteractionModeChangeEvent);
            EventBus.Publish(new InteractionModeChangeEvent(InteractionMode.SelectItem));
            _gameInput.OrbitView.OrbitCamera.performed += OnOrbitCamera;
            _gameInput.OrbitView.ScrollZoom.performed += OnScrollZoom;
            _gameInput.OrbitView.SingleTapSelect.performed += OnSingleTapSelect;
            _gameInput.OrbitView.DoubleTapSelect.performed += OnDoubleTapSelect;
            _gameInput.OrbitView.TapHoldSelect.performed += OnTapHoldSelect;
            _gameInput.Enable();
        }

        private void OnSingleTapSelect(InputAction.CallbackContext ctx)
        {
            if (_isOrbiting)
                return;

            if (EventSystem.current.IsPointerOverGameObject(-1))
            {
                Debug.Log("Pointer is over UI");
                return;
            }
            // cancel any existing popup menus
            EventBus.Publish(new CancelPopupMenuEvent());

            Vector2 pointerPosition = _gameInput.OrbitView.PointerPosition.ReadValue<Vector2>();
            RaycastObjects(pointerPosition, ClickablePopupMenu);
        }

        private void OnDoubleTapSelect(InputAction.CallbackContext ctx)
        {
            if (_isOrbiting)
                return;

            if (EventSystem.current.IsPointerOverGameObject(-1))
            {
                Debug.Log("Pointer is over UI");
                return;
            }
            Vector2 pointerPosition = _gameInput.OrbitView.PointerPosition.ReadValue<Vector2>();
            // RaycastObjects(pointerPosition, ClickableFocusOrbit);
        }

        private void OnTapHoldSelect(InputAction.CallbackContext ctx)
        {
            if (_isOrbiting)
                return;

            if (EventSystem.current.IsPointerOverGameObject(-1))
            {
                Debug.Log("Pointer is over UI");
                return;
            }
            Vector2 pointerPosition = _gameInput.OrbitView.PointerPosition.ReadValue<Vector2>();
            RaycastObjects(pointerPosition, ClickableFocusOrbit);
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

            _isOrbiting = true;
            if (_resetOrbitFlagCoroutine != null) {
                StopCoroutine(_resetOrbitFlagCoroutine);
            }
            _resetOrbitFlagCoroutine = StartCoroutine(ResetOrbitFlag(0.5f));

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
                _cameraController.Orbit.ChangeRadiusTemporary(
                    ctx.ReadValue<Vector2>().y * 0.01f,
                    2f
                );
            }
            else
            {
                _cameraController.Orbit.ChangeRadius(ctx.ReadValue<Vector2>().y * 0.01f);
            }
        }

        private void RaycastObjects(
            Vector2 position,
            Action<Transform, IClickable> clickableCallback
        )
        {
            // Convert mouse position to ray
            Vector3 position3D = new Vector3(position.x, position.y, 0f);
            Ray ray = _mainCamera.ScreenPointToRay(position3D);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.TryGetComponent<IClickable>(out var clickable))
                {
                    // OnRaycastClicked(hit.transform, clickable);
                    clickableCallback(hit.transform, clickable);
                }
            }
        }

        private void ClickablePopupMenu(Transform t, IClickable clickable)
        {
            if (Mode != InteractionMode.SelectItem)
            {
                return;
            }

            EventBus.Publish(new CameraOrbitLookAtSecondaryTargetEvent(t));

            ServiceLocator.Current
                .Get<UIManager>()
                .OnPopupMenuEvent(
                    new PopupMenuEvent(
                        t.name,
                        t,
                        () => Debug.Log("Calling Cancel"),
                        new PopupMenuEvent.CreateButton[]
                        {
                            new(
                                "Move To",
                                () =>
                                {
                                    EventBus.Publish(
                                        new RoomSendMessageEvent(
                                            "move",
                                            new RequestMoveMessage
                                            {
                                                tileId = t.GetComponent<BaseTile>().Id,
                                                destination = t.GetComponent<BaseTile>().Coordinates
                                            }
                                        )
                                    );
                                    EventBus.Publish(new CancelPopupMenuEvent());
                                }
                            ),
                            // we should make CreateButton a bit more feature rich, so
                            // we can have certain types of buttons with certain icons
                            // new("Move To", () => UFB.Events.EventBus.Publish<)
                        }
                    )
                );
        }

        private void ClickableFocusOrbit(Transform t, IClickable clickable)
        {
            Debug.Log("Double clicked");
            EventBus.Publish(new CancelPopupMenuEvent());
            EventBus.Publish(new CameraOrbitAroundEvent(t, 0.3f));
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

        private IEnumerator ResetOrbitFlag(float delay)
        {
            yield return new WaitForSeconds(delay);
            _isOrbiting = false; // Reset the flag after the delay
        }
    }
}
