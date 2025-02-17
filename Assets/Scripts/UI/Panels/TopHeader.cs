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
using UFB.Items;
using Unity.VisualScripting;

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

        public string selectedId = "";

        private void OnEnable()
        {
            //EventBus.Subscribe<SelectedCharacterEvent>(OnSelectedCharacterEvent);
        }

        private void OnDisable()
        {
            //EventBus.Unsubscribe<SelectedCharacterEvent>(OnSelectedCharacterEvent);
        }

        public void OnSelectedCharacterEvent(CharacterState e)
        {
            selectedId = e.characterId;
            _healthBar.SetRangedValueState(e.stats.health, e);
            _energyBar.SetRangedValueState(e.stats.energy, e);
            _ultimateBar.SetRangedValueState(e.stats.ultimate, e);

            if(_screenNameText != null)
            {
                _screenNameText.text = e.displayName;
            }
            if (_stepEnergeText != null) 
            { 
                _stepEnergeText.text = e.stats.energy.current.ToString();
            }

            string newTileId = e.currentTileId;
            Tile CurrentTile = ServiceLocator.Current.Get<GameBoard>().Tiles[newTileId];

            if(_tilePosText != null)
            {
                _tilePosText.text = CurrentTile.TilePosText;
            }

            e.OnChange(() => 
            {
                if(e.characterId == selectedId)
                {
                    if (_screenNameText != null)
                    {
                        _screenNameText.text = e.displayName;
                    }
                    if (_stepEnergeText != null)
                    {
                        _stepEnergeText.text = e.stats.energy.current.ToString();
                    }

                    string newTileId = e.currentTileId;
                    Tile CurrentTile = ServiceLocator.Current.Get<GameBoard>().Tiles[newTileId];
                    if (_tilePosText != null)
                    {
                        _tilePosText.text = CurrentTile.TilePosText;
                    }
                }
            });



            Addressables
                .LoadAssetAsync<UfbCharacter>("UfbCharacter/" + e.characterClass)
                .Completed += (op) =>
            {
                if (
                    op.Status
                    == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded
                )
                {
                    _avatarImage.sprite = op.Result.avatar;
                    if (_resourceAvatarImage != null) 
                    { 
                        _resourceAvatarImage.sprite = op.Result.avatar;
                    }
                }
                else
                    Debug.LogError(
                        "Failed to load character avatar: " + op.OperationException.Message
                    );
            };
        }
    }
}
