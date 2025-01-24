using System.Collections;
using System.Collections.Generic;
using UFB.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace UFB.UI
{
    public class BottomDrawer : MonoBehaviour
    {
        public float swipeThreshold = 0.5f; // Swipe distance threshold to be considered a swipe.
        public float bottomTapThreshold = 0.2f;
        public float expandedPixelHeight = 500;
        public float closedPixelHeight = 500;

        [SerializeField]
        private List<EquipSlot> _equipSlots = new List<EquipSlot>();

        private GameInput _gameInput;

        private bool _isExpanded = false;
        private Coroutine _animationCoroutine;
        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void CloseBottomDrawer()
        {
            if (_animationCoroutine != null)
                StopCoroutine(_animationCoroutine);

            _animationCoroutine = this.LerpAction(
                (t) =>
                {
                    float y = _rectTransform.anchoredPosition.y;
                    y = Mathf.Lerp(y, closedPixelHeight, t);
                    _rectTransform.anchoredPosition = new Vector2(_rectTransform.anchoredPosition.x, y);

                },
                0.3f,
                () =>
                {
                    _isExpanded = false;
                    _rectTransform.anchoredPosition = new Vector2(_rectTransform.anchoredPosition.x, closedPixelHeight);
                }
            );
        }

        public void OpenBottomDrawer()
        {
            if (_animationCoroutine != null)
                StopCoroutine(_animationCoroutine);
            _animationCoroutine = this.LerpAction(
                (t) =>
                {
                    float y = _rectTransform.anchoredPosition.y;
                    y = Mathf.Lerp(y, expandedPixelHeight, t);
                    _rectTransform.anchoredPosition = new Vector2(_rectTransform.anchoredPosition.x, y);
                },
                0.3f,
                () =>
                {
                    _isExpanded = true;
                    _rectTransform.anchoredPosition = new Vector2(_rectTransform.anchoredPosition.x, expandedPixelHeight);
                }
            );
        }
    }
}
