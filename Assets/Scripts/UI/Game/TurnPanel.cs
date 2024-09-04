using System.Collections;
using System.Collections.Generic;
using UFB.Character;
using UFB.Events;
using UFB.Network.RoomMessageTypes;
using UnityEngine;
using UnityEngine.UI;

public class TurnPanel : MonoBehaviour
{
    public Text turnTimeText;
    public Text topTurnTimeText;

    public Text desText;

    public void InitData(float turnTime)
    {
        turnTimeText.text = Utility.GetTurnTimeString(turnTime);
        topTurnTimeText.text = Utility.GetTurnTimeString(turnTime);
                
        desText.text = UIGameManager.instance.isPlayerTurn? $"Your Turn!" : $"Monster Turn!";
        
        gameObject.SetActive(true);
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
}
