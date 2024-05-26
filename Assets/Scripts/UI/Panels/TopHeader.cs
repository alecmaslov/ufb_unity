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
        private LinearIndicatorBar _ultimateBar;
        
        [SerializeField]
        private Text _stepEnergeText;

        [SerializeField]
        private Text _tilePosText;

        private void OnEnable()
        {
            //EventBus.Subscribe<SelectedCharacterEvent>(OnSelectedCharacterEvent);
        }

        private void OnDisable()
        {
            //EventBus.Unsubscribe<SelectedCharacterEvent>(OnSelectedCharacterEvent);
        }

        public void OnSelectedCharacterEvent(ChangeCharacterStateEvent e)
        {
            _healthBar.SetRangedValueState(e.state.stats.health);
            _energyBar.SetRangedValueState(e.state.stats.energy);
            _ultimateBar.SetRangedValueState(e.state.stats.ultimate);

            _screenNameText.text = e.state.displayName;
            _stepEnergeText.text = e.state.stats.energy.current.ToString();

            string newTileId = e.state.currentTileId;
            Tile CurrentTile = ServiceLocator.Current.Get<GameBoard>().Tiles[newTileId];
            _tilePosText.text = CurrentTile.TilePosText;

            e.state.OnChange(() => 
            {
                _screenNameText.text = e.state.displayName;
                _stepEnergeText.text = e.state.stats.energy.current.ToString();

                string newTileId = e.state.currentTileId;
                Tile CurrentTile = ServiceLocator.Current.Get<GameBoard>().Tiles[newTileId];
                _tilePosText.text = CurrentTile.TilePosText;
            });



            Addressables
                .LoadAssetAsync<UfbCharacter>("UfbCharacter/" + e.state.characterClass)
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
