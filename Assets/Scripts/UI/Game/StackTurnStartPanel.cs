using System.Collections;
using System.Collections.Generic;
using UFB.Events;
using UFB.Items;
using UFB.Network.RoomMessageTypes;
using UnityEngine;
using UnityEngine.UI;

public class StackTurnStartPanel : MonoBehaviour
{
    public Image stackImage;
    public GameObject diceRect;

    public List<ResultItem> stackItems = new List<ResultItem>();

    public bool isStackTurn = false;

    private ResultItem selectedStack;
    private DiceData[] diceData;

    public void InitData(GetStackOnStartMessage m)
    {
        stackItems.Clear();
        selectedStack = null;
        diceData = null;
        if (m.stackList.Length > 0 )
        {
            UIGameManager.instance.bottomDrawer.gameObject.SetActive(false);
            foreach (var item in m.stackList)
            {
                stackItems.Add(item);
            }
            isStackTurn = true;
            gameObject.SetActive(true);
            InitDice();
        }
        else
        {
            gameObject.SetActive(false);
            isStackTurn = false;
        }

    }

    private DICE_TYPE selectedType = DICE_TYPE.DICE_4;

    public void InitDice()
    {
        ResultItem stack = stackItems[0];
        selectedStack = stack;
        stackImage.sprite = GlobalResources.instance.stacks[stack.id];
        stackImage.gameObject.SetActive(true);

        if((STACK) stack.id == STACK.Cure || (STACK)stack.id == STACK.Burn || (STACK)stack.id == STACK.Freeze || (STACK)stack.id == STACK.Charge)
        {
            DiceArea.instance.SetDiceType(DICE_TYPE.DICE_4);
            selectedType = DICE_TYPE.DICE_4;
        }
        else if((STACK)stack.id == STACK.Void || (STACK)stack.id == STACK.Slow)
        {
            DiceArea.instance.SetDiceType(DICE_TYPE.DICE_6_4);
            selectedType = DICE_TYPE.DICE_6_4;

        }

        stackItems.Remove(stack);

        StartCoroutine(StartDiceRoll());
    }

    public void OnSelectDice()
    {
        EventBus.Publish(
            RoomSendMessageEvent.Create(
                GlobalDefine.CLIENT_MESSAGE.SET_DICE_STACK_TURN_ROLL,
                new RequestSetDiceStackTurnRoll
                {
                    characterId = UIGameManager.instance.controller.Id,
                    diceType = (int) selectedType,
                }
            )
        );
        stackImage.gameObject.SetActive(false);
    }

    public void OnLanuchDiceRoll(SetDiceRollMessage message)
    {
        diceData = message.diceData;

        DiceArea.instance.LaunchDice(message.diceData);
    }

    public void OnFinishDice()
    {
        if(!gameObject.activeSelf)
        {
            return;
        }
        EventBus.Publish(
            RoomSendMessageEvent.Create(
                GlobalDefine.CLIENT_MESSAGE.SET_STACK_ON_START,
                new RequestStackOnStartMessage
                {
                    characterId = UIGameManager.instance.controller.Id,
                    stackId = selectedStack.id,
                    diceData = diceData
                }
            )
        );

        if (stackItems.Count > 0) { 
            InitDice();
        }
        else
        {
            isStackTurn = false;
            UIGameManager.instance.bottomDrawer.gameObject.SetActive(true);
            gameObject.SetActive (false);
        }
    }

    IEnumerator StartDiceRoll()
    {
        yield return new WaitForSeconds(1f);
        OnSelectDice();
    }
}
