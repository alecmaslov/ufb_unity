using System.Collections;
using System.Collections.Generic;
using UFB.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace UFB.UI
{
    public class BottomDrawer : MonoBehaviour, IPointerClickHandler
    {
        public float swipeThreshold = 0.5f; // Swipe distance threshold to be considered a swipe.
        public float bottomTapThreshold = 0.2f;
        public float expandedPixelHeight = 500;

        [SerializeField]
        private List<EquipSlot> _equipSlots = new List<EquipSlot>();

        private GameInput _gameInput;

        private bool _isExpanded = false;
        private Coroutine _animationCoroutine;
        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _gameInput = new GameInput();
            // _gameInput.GameUI.Select.performed += OnSelectPerformed;
            _gameInput.GameUI.Swipe.performed += OnSwipePerformed;
        }

        private void OnEnable()
        {
            _gameInput.Enable();
        }

        private void OnDisable()
        {
            _gameInput.Disable();
        }

        private void OnSwipePerformed(InputAction.CallbackContext context)
        {
            Vector2 swipeDelta = context.ReadValue<Vector2>();
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            // make sure touch occurs in bottom portion of screen

            if (touchPosition.y > (Screen.height * bottomTapThreshold))
                return;

            // Vector2 position = 
            if (swipeDelta.y > swipeThreshold)
            {
                Debug.Log("Swiped Up!");
                OpenBottomDrawer();
            }
            else if (swipeDelta.y < -swipeThreshold)
            {
                Debug.Log("Swiped Down!");
                CloseBottomDrawer();
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Clicked bottom drawer!");
            // throw new System.NotImplementedException();

            if (_isExpanded)
                CloseBottomDrawer();
            else
                OpenBottomDrawer();
        }

        private void CloseBottomDrawer()
        {
            if (_animationCoroutine != null)
                StopCoroutine(_animationCoroutine);

            _animationCoroutine = this.LerpAction(
                (t) =>
                {
                    Debug.Log($"CLOSING | Lerping to {t}");
                    var currentPosition = transform.position;
                    currentPosition.y = Mathf.Lerp(currentPosition.y, 0, t);
                    transform.position = currentPosition;
                },
                0.3f,
                () =>
                {
                    _isExpanded = false;
                    var currentPosition = transform.position;
                    currentPosition.y = 0;
                    transform.position = currentPosition;
                }
            );
        }

        private void OpenBottomDrawer()
        {
            Debug.Log("Opening bottom drawer!"); 
            if (_animationCoroutine != null)
                StopCoroutine(_animationCoroutine);
            _animationCoroutine = this.LerpAction(
                (t) =>
                {
                    Debug.Log($"OPENING | Lerping to {t}");
                    var currentPosition = transform.position;
                    currentPosition.y = Mathf.Lerp(currentPosition.y, expandedPixelHeight, t);
                    transform.position = currentPosition;
                },
                0.3f,
                () =>
                {
                    _isExpanded = true;
                    var currentPosition = transform.position;
                    currentPosition.y = expandedPixelHeight;
                    transform.position = currentPosition;
                }
            );
        }
    }
}
