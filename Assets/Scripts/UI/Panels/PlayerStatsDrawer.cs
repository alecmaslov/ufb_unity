using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;

namespace UFB.UI
{
    // we can have some sort of event that is like PlayerSelectedEvent that this listens to
    // it will then determine whether to update these stats based on the new state received
    // from the stateful list of players

    public class PlayerStatsDrawer : MonoBehaviour, IPointerClickHandler
    {
        public float animationDuration = 0.3f;

        public RadialIndicatorComponent healthIndicator;
        public RadialIndicatorComponent energyIndicator;

        [SerializeField] private RectTransform _container;
        [SerializeField] private RectTransform _headerContainer;
        [SerializeField] private float _headerOpenAnchoredY = 160;
        [SerializeField] private float _containerOpenBottomY = 500;
        [SerializeField] private float _headerOpenHeight = 200;
        [SerializeField] private AssetReference _drawerExpandedPrefab;

        private GameObject _drawerExpanded;

        private Vector2 _headerAnchoredPosition;
        private Vector2 _headerClosedSizeDelta;
        private Vector2 _containerOffsetMin;
        private bool _isExpanded = false;

        private Coroutine _animationCoroutine;

        void Awake()
        {
            _headerAnchoredPosition = _headerContainer.anchoredPosition;
            _containerOffsetMin = _container.offsetMin;
            _headerClosedSizeDelta = _headerContainer.sizeDelta;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_isExpanded) // CLOSE
            {
                var currentHeaderAnchoredPosition = _headerContainer.anchoredPosition;
                var currentContainerOffsetMin = _container.offsetMin;
                var currentSizeDelta = _headerContainer.sizeDelta;


                if (_drawerExpanded != null)
                    Addressables.ReleaseInstance(_drawerExpanded);
                

                if (_animationCoroutine != null)
                    StopCoroutine(_animationCoroutine);


                _animationCoroutine = CoroutineHelpers.LerpAction(
                    (t) =>
                    {
                        _headerContainer.anchoredPosition = Vector2.Lerp(currentHeaderAnchoredPosition, _headerAnchoredPosition, t);
                        _container.offsetMin = Vector2.Lerp(currentContainerOffsetMin, _containerOffsetMin, t);
                        _headerContainer.sizeDelta = Vector2.Lerp(currentSizeDelta, _headerClosedSizeDelta, t);
                    },
                    () =>
                    {
                        _headerContainer.anchoredPosition = _headerAnchoredPosition;
                        _container.offsetMin = _containerOffsetMin;
                        _headerContainer.sizeDelta = _headerClosedSizeDelta;
                    },
                    animationDuration,
                    this
                );

                _isExpanded = false;
            }
            else // OPEN
            {
                var currentHeaderAnchoredPosition = _headerContainer.anchoredPosition;
                var currentContainerOffsetMin = _container.offsetMin;
                var currentSizeDelta = _headerContainer.sizeDelta;

                _drawerExpandedPrefab.InstantiateAsync(transform).Completed += (op) =>
                {
                    _drawerExpanded = op.Result;
                };

                if (_animationCoroutine != null)
                    StopCoroutine(_animationCoroutine);

                _animationCoroutine = CoroutineHelpers.LerpAction(
                    (t) =>
                    {
                        _headerContainer.anchoredPosition = Vector2.Lerp(currentHeaderAnchoredPosition, new Vector2(_headerAnchoredPosition.x, _headerOpenAnchoredY), t);
                        _container.offsetMin = Vector2.Lerp(currentContainerOffsetMin, new Vector2(_containerOffsetMin.x, _containerOpenBottomY), t);
                        _headerContainer.sizeDelta = new Vector2(_headerClosedSizeDelta.x, Mathf.Lerp(currentSizeDelta.y, _headerOpenHeight, t));
                    },
                    () =>
                    {
                        _headerContainer.anchoredPosition = new Vector2(_headerAnchoredPosition.x, _headerOpenAnchoredY);
                        _container.offsetMin = new Vector2(_containerOffsetMin.x, _containerOpenBottomY);
                        _headerContainer.sizeDelta = new Vector2(_headerClosedSizeDelta.x, _headerOpenHeight);
                    },
                    animationDuration,
                    this
                );

                _isExpanded = true;
            }
        }
    }

}

// _headerContainer.anchoredPosition = Vector2.Lerp(_headerAnchoredPosition, new Vector2(_headerAnchoredPosition.x, _headerOpenAnchoredY), t);
// _container.offsetMin = Vector2.Lerp(_containerOffsetMin, new Vector2(_containerOffsetMin.x, _containerOpenBottomY), t);
// _headerContainer.sizeDelta = new Vector2(_headerClosedSizeDelta.x, Mathf.Lerp(_headerClosedSizeDelta.y, _headerOpenHeight, t));
// // _headerContainer.sizeDelta = Vector2.Lerp(_headerClosedSizeDelta, _headerOpenSizeDelta, t);

// _headerContainer.anchoredPosition = new Vector2(_headerAnchoredPosition.x, _headerOpenAnchoredY);
// _container.offsetMin = new Vector2(_containerOffsetMin.x, _containerOpenBottomY);
// _headerContainer.sizeDelta = new Vector2(_headerClosedSizeDelta.x, _headerOpenHeight);
// // _headerContainer.sizeDelta = _headerOpenSizeDelta;