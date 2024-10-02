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

namespace UFB.UI
{
    public class KillItemCard : MonoBehaviour
    {
        [SerializeField]
        private Image _avatarImage;


        [SerializeField]
        private LinearIndicatorBar _healthBar;

        [SerializeField]
        private LinearIndicatorBar _energyBar;

        [SerializeField]
        private LinearIndicatorBar _ultimateBar;
        

        public void OnSelectedCharacterEvent(CharacterState e)
        {
            Debug.Log(e.stats.health.current);
            _healthBar.SetRangedValueState(e.stats.health, e);
            _energyBar.SetRangedValueState(e.stats.energy, e);
            _ultimateBar.SetRangedValueState(e.stats.ultimate, e);

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
                }
                else
                    Debug.LogError(
                        "Failed to load character avatar: " + op.OperationException.Message
                    );
            };
        }
    }
}
