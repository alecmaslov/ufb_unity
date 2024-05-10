using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine;
using UFB.StateSchema;
using UFB.Core;
using UFB.Events;
using TMPro;
using UFB.Character;
using UnityEngine.UI;
using UFB.Map;

namespace UFB.UI
{
    public class TopHeader : MonoBehaviour
    {
        [SerializeField]
        private Image _avatarImage;

        [SerializeField]
        private Image _resourceAvatarImage;

        [SerializeField]
        private Text _screenNameText;

        [SerializeField]
        private LinearIndicatorBar _healthBar;

        [SerializeField]
        private LinearIndicatorBar _energyBar;

        [SerializeField]
        private Text _stepEnergeText;

        [SerializeField]
        private Text _tilePosText;

        private void OnEnable()
        {
            EventBus.Subscribe<SelectedCharacterEvent>(OnSelectedCharacterEvent);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<SelectedCharacterEvent>(OnSelectedCharacterEvent);
        }

        public void OnSelectedCharacterEvent(SelectedCharacterEvent e)
        {
            _healthBar.SetRangedValueState(e.controller.State.stats.health);
            _energyBar.SetRangedValueState(e.controller.State.stats.energy);

            _screenNameText.text = e.controller.State.displayName;
            _stepEnergeText.text = e.controller.State.stats.energy.current.ToString();

            string newTileId = e.controller.State.currentTileId;
            Tile CurrentTile = ServiceLocator.Current.Get<GameBoard>().Tiles[newTileId];
            _tilePosText.text = CurrentTile.TilePosText;

            e.controller.State.OnChange(() => 
            {
                _screenNameText.text = e.controller.State.displayName;
                _stepEnergeText.text = e.controller.State.stats.energy.current.ToString();

                string newTileId = e.controller.State.currentTileId;
                Tile CurrentTile = ServiceLocator.Current.Get<GameBoard>().Tiles[newTileId];
                _tilePosText.text = CurrentTile.TilePosText;
            });



            Addressables
                .LoadAssetAsync<UfbCharacter>("UfbCharacter/" + e.controller.State.characterClass)
                .Completed += (op) =>
            {
                if (
                    op.Status
                    == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded
                )
                {
                    _avatarImage.sprite = op.Result.avatar;
                    _resourceAvatarImage.sprite = op.Result.avatar;
                }
                else
                    Debug.LogError(
                        "Failed to load character avatar: " + op.OperationException.Message
                    );
            };
        }
    }
}
