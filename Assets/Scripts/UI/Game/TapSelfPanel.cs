using System.Collections;
using System.Collections.Generic;
using UFB.Character;
using UFB.Events;
using UFB.Network.RoomMessageTypes;
using UFB.StateSchema;
using UFB.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class TapSelfPanel : MonoBehaviour
{
    public static TapSelfPanel Instance;

    public GameObject SelfItemPanel;
    public PowerMoveItem PowerMoveItem;

    public int idx = 0;

    public PowerMove[] moves;

    public PowerMove selectedPowermove;
    
    public Image avatarImage;

    private void Awake()
    {
        Instance = this;
    }

    public void InitPanel()
    {
        PowerMoveItem.gameObject.SetActive(false);
        SelfItemPanel.SetActive(true);
        gameObject.SetActive(true);
        
        Addressables
            .LoadAssetAsync<UfbCharacter>("UfbCharacter/" + UIGameManager.instance.controller.State.characterClass)
            .Completed += (op) =>
        {
            if (
                op.Status
                == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded
            )
            {
                avatarImage.sprite = op.Result.avatar;
            }
            else
                Debug.LogError(
                    "Failed to load character avatar: " + op.OperationException.Message
                );
        };
    }

    public void InitPowermove(Item item, EquipSlot slt, PowerMove[] _moves)
    {
        SelfItemPanel.SetActive(false);
        PowerMoveItem.gameObject.SetActive(true);

        moves = _moves;
        ResetPowermove();
    }

    public void ResetPowermove()
    {
        selectedPowermove = moves[idx];

        PowerMoveItem.Init(selectedPowermove);
        PowerMoveItem.InitResultList();
        PowerMoveItem.gameObject.SetActive(true);
    }

    public void OnNextPowermove()
    {
        idx++;
        if (idx >= moves.Length)
        {
            idx = 0;
        }
        ResetPowermove();
    }

    public void OnConfirmBuff()
    {
        if (selectedPowermove != null)
        {
            if (selectedPowermove.range == 0)
            {
                EventBus.Publish(
                    RoomSendMessageEvent.Create(
                        GlobalDefine.CLIENT_MESSAGE.SET_POWER_MOVE_ITEM,
                        new RequestSetPowerMoveItem
                        {
                            enemyId = "",
                            characterId = UIGameManager.instance.controller.Id,
                            powerMoveId = selectedPowermove.id,
                            diceCount = 1
                        }
                    )
                );
            }
            else
            {
                UIGameManager.instance.errorPanel.InitData("This power move is not buff.");
            }

        }
    }
    public void OnCancelBuff() 
    {
        SelfItemPanel.SetActive(true);
        PowerMoveItem.gameObject.SetActive(false);
    }
}
