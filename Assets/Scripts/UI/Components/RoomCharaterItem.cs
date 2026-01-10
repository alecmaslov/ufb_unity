using System.Collections;
using System.Collections.Generic;
using UFB.Character;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class RoomCharaterItem : MonoBehaviour
{
    public Text characterName;
    public Text characterClass;
    public Image characterImage;

    public Text owerText;

    public void InitData(RoomUserData data, bool isMe = false)
    {
        characterName.text = data.name;
        characterClass.text = $"Mount {data.characterClass}";
        owerText.text = isMe ? "Me" : "";
        gameObject.SetActive(true);
        
        Addressables
            .LoadAssetAsync<UfbCharacter>("UfbCharacter/" + data.characterClass)
            .Completed += (op) =>
        {
            if (
                op.Status
                == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded
            )
            {
                characterImage.sprite = op.Result.avatar;
            }
            else
                Debug.LogError(
                    "Failed to load character avatar: " + op.OperationException.Message
                );
        };
    }
}
