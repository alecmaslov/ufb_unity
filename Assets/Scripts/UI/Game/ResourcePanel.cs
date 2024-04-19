using System.Collections;
using System.Collections.Generic;
using UFB.Character;
using UFB.Events;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace UFB.UI
{
    public class ResourcePanel : MonoBehaviour
    {
        [SerializeField]
        private ItemCard item;

        [SerializeField]
        private Image _avatarImage;

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
            Addressables
                .LoadAssetAsync<UfbCharacter>("UfbCharacter/" + e.controller.State.characterClass)
                .Completed += (op) =>
                {
                    if (
                        op.Status
                        == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded
                    )
                        _avatarImage.sprite = op.Result.avatar;
                    else
                        Debug.LogError(
                            "Failed to load character avatar: " + op.OperationException.Message
                        );
                };
        }
    }
}

