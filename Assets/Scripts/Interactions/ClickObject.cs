using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UFB.Events;
using UFB.Input;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace UFB.Events
{
    public class RaycastClickableEvent
    {
        public RaycastHit Hit { get; }
        public IRaycastClickable Clickable { get; }

        public RaycastClickableEvent(RaycastHit hit, IRaycastClickable clickable)
        {
            Hit = hit;
            Clickable = clickable;
        }
    }
}

namespace UFB.Interactions
{
    public class ClickObject : MonoBehaviour
    {
        public enum ClickInteractionMode
        {
            None,
            Select,
            Interact
        }

        private UnityEngine.Camera _mainCamera;
        private GameInput _gameInput;

        private void Awake()
        {
            if (!Mouse.current.enabled)
            {
                InputSystem.EnableDevice(Mouse.current);
            }

            _gameInput = new GameInput();
            _gameInput.OrbitView.TouchPress.started += OnTouchPressStarted;
            _gameInput.OrbitView.MouseClick.started += OnMouseClickStarted;

            _gameInput.OrbitView.MouseClick.started += ctx =>
            {
                Debug.Log("Mouse click started");
            };
            // _gameInput.Touch.MouseClick.performed += OnMouseClickStarted;
        }

        private void OnEnable()
        {
            _gameInput.Enable();
        }

        private void OnDisable()
        {
            _gameInput.Disable();
        }

        void Start()
        {
            // Get reference to the main camera
            _mainCamera = UnityEngine.Camera.main;
        }

        private void OnTouchPressStarted(InputAction.CallbackContext ctx)
        {
            int pointerId = Touchscreen.current.primaryTouch.touchId.ReadValue();
            if (EventSystem.current.IsPointerOverGameObject(pointerId))
                return;
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            Debug.Log($"Touch position: {touchPosition}");
            RaycastObjects(touchPosition);
        }

        private void OnMouseClickStarted(InputAction.CallbackContext ctx)
        {
            Debug.Log("Mouse click started");
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Debug.Log($"Mouse position: {mousePosition}");

            // For mouse, pointerId is generally -1 for the first mouse
            if (EventSystem.current.IsPointerOverGameObject(-1))
                return;

            RaycastObjects(mousePosition);
        }

        private void RaycastObjects(Vector2 position)
        {
            // Convert mouse position to ray
            Vector3 position3D = new Vector3(position.x, position.y, 0f);
            Ray ray = _mainCamera.ScreenPointToRay(position3D);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.TryGetComponent<IRaycastClickable>(out var clickable))
                {
                    Debug.Log($"IRaycastClickable component found | {hit.transform.name}");
                    EventBus.Publish(new RaycastClickableEvent(hit, clickable));
                }
            }
        }

        // void Update()
        // {
        //     // If left mouse button is clicked
        //     if (UnityEngine.Input.GetMouseButtonDown(0)
        //     // && !EventSystem.current.IsPointerOverGameObject()
        //     )
        //     {
        //         RaycastObjects(UnityEngine.Input.mousePosition);
        //     }
        // }
    }
}
