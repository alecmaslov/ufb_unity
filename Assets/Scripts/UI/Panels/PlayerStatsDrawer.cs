using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine;
using UFB.StateSchema;
using UFB.Core;
using UFB.Events;
using TMPro;
using UFB.Character;
using UnityEngine.UI;

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

        [SerializeField]
        private RectTransform _container;

        [SerializeField]
        private RectTransform _headerContainer;

        [SerializeField]
        private float _headerOpenAnchoredY = 160;

        [SerializeField]
        private float _containerOpenBottomY = 500;

        [SerializeField]
        private float _headerOpenHeight = 200;

        // [SerializeField]
        // private AssetReference _drawerExpandedPrefab;

        [SerializeField]
        private GameObject _drawerExpandedPrefab;

        [SerializeField]
        private TextMeshProUGUI _screenNameText;

        [SerializeField]
        private Image _characterAvatar;

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

        private void OnEnable()
        {
            EventBus.Subscribe<SelectedCharacterEvent>(OnSelectedCharacterEvent);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<SelectedCharacterEvent>(OnSelectedCharacterEvent);
        }

        private void OnSelectedCharacterEvent(SelectedCharacterEvent e)
        {
            healthIndicator.SetRangedValueState(e.controller.State.stats.health);
            energyIndicator.SetRangedValueState(e.controller.State.stats.energy);
            _screenNameText.text = e.controller.State.displayName;
            Addressables
                .LoadAssetAsync<UfbCharacter>("UfbCharacter/" + e.controller.State.characterClass)
                .Completed += (op) =>
            {
                if (op.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                    _characterAvatar.sprite = op.Result.avatar;
                else
                    Debug.LogError("Failed to load character avatar: " + op.OperationException.Message);
            };
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_isExpanded) // CLOSE
            {
                var currentHeaderAnchoredPosition = _headerContainer.anchoredPosition;
                var currentContainerOffsetMin = _container.offsetMin;
                var currentSizeDelta = _headerContainer.sizeDelta;

                // if (_drawerExpanded != null)
                //     Addressables.ReleaseInstance(_drawerExpanded);

                Destroy(_drawerExpanded);

                if (_animationCoroutine != null)
                    StopCoroutine(_animationCoroutine);

                _animationCoroutine = this.LerpAction(
                    (t) =>
                    {
                        _headerContainer.anchoredPosition = Vector2.Lerp(
                            currentHeaderAnchoredPosition,
                            _headerAnchoredPosition,
                            t
                        );
                        _container.offsetMin = Vector2.Lerp(
                            currentContainerOffsetMin,
                            _containerOffsetMin,
                            t
                        );
                        _headerContainer.sizeDelta = Vector2.Lerp(
                            currentSizeDelta,
                            _headerClosedSizeDelta,
                            t
                        );
                    },
                    animationDuration,
                    () =>
                    {
                        _headerContainer.anchoredPosition = _headerAnchoredPosition;
                        _container.offsetMin = _containerOffsetMin;
                        _headerContainer.sizeDelta = _headerClosedSizeDelta;
                    }
                );

                _isExpanded = false;
            }
            else // OPEN
            {
                var currentHeaderAnchoredPosition = _headerContainer.anchoredPosition;
                var currentContainerOffsetMin = _container.offsetMin;
                var currentSizeDelta = _headerContainer.sizeDelta;

                // _drawerExpandedPrefab.InstantiateAsync(transform).Completed += (op) =>
                // {
                //     _drawerExpanded = op.Result;
                // };

                _drawerExpanded = Instantiate(_drawerExpandedPrefab, transform);

                if (_animationCoroutine != null)
                    StopCoroutine(_animationCoroutine);

                _animationCoroutine = this.LerpAction(
                    (t) =>
                    {
                        _headerContainer.anchoredPosition = Vector2.Lerp(
                            currentHeaderAnchoredPosition,
                            new Vector2(_headerAnchoredPosition.x, _headerOpenAnchoredY),
                            t
                        );
                        _container.offsetMin = Vector2.Lerp(
                            currentContainerOffsetMin,
                            new Vector2(_containerOffsetMin.x, _containerOpenBottomY),
                            t
                        );
                        _headerContainer.sizeDelta = new Vector2(
                            _headerClosedSizeDelta.x,
                            Mathf.Lerp(currentSizeDelta.y, _headerOpenHeight, t)
                        );
                    },
                    animationDuration,
                    () =>
                    {
                        _headerContainer.anchoredPosition = new Vector2(
                            _headerAnchoredPosition.x,
                            _headerOpenAnchoredY
                        );
                        _container.offsetMin = new Vector2(
                            _containerOffsetMin.x,
                            _containerOpenBottomY
                        );
                        _headerContainer.sizeDelta = new Vector2(
                            _headerClosedSizeDelta.x,
                            _headerOpenHeight
                        );
                    }
                );

                _isExpanded = true;
            }
        }
    }
}
