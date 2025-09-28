using System.Collections;
using System.Collections.Generic;
using UFB.Character;
using UFB.Events;
using UFB.Network.RoomMessageTypes;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class TurnPanel : MonoBehaviour
{
    public Text turnTimeText;
    public Text topTurnTimeText;

    public Text desText;
    public Image turnCharacterImage;

    public void InitData(float turnTime)
    {
        turnTimeText.text = Utility.GetTurnTimeString(turnTime);
        topTurnTimeText.text = Utility.GetTurnTimeString(turnTime);

        string characterName = UIGameManager.instance.controller.State.displayName;

        desText.text = UIGameManager.instance.isPlayerTurn ? "YOUR TURN" : characterName;
        Addressables
        .LoadAssetAsync<UfbCharacter>("UfbCharacter/" + UIGameManager.instance.controller.State.characterClass)
        .Completed += (op) =>
        {
            if (op.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {

                turnCharacterImage.sprite = op.Result.avatar;
            }
            else
            {
                Debug.LogError("Failed to load character avatar: " + op.OperationException.Message);
            }
        };
        gameObject.SetActive(true);

        StartCoroutine(ClosePanel());

    }

    public void SetTurnTime(float turnTime)
    {
        turnTimeText.text = Utility.GetTurnTimeString(turnTime);
        topTurnTimeText.text = Utility.GetTurnTimeString(turnTime);
    }

    public void CloseTurnPanel()
    {
        if (CharacterManager.Instance.PlayerCharacter.Id == CharacterManager.Instance.SelectedCharacter.Id) {
            EventBus.Publish(
                RoomSendMessageEvent.Create(
                    GlobalDefine.CLIENT_MESSAGE.GET_STACK_ON_TURN_START,
                    new RequestCharacterId
                    {
                        characterId = UIGameManager.instance.controller.Id,
                    }
                )
            );
        }

        gameObject.SetActive(false);
    }

    IEnumerator ClosePanel()
    {
        yield return new WaitForSeconds(3f);
        if (gameObject.activeSelf)
        {
            CloseTurnPanel();
        }
    }
}
