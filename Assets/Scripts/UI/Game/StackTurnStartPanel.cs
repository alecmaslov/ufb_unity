using System.Collections;
using System.Collections.Generic;
using UFB.Character;
using UFB.Events;
using UFB.Items;
using UFB.Network.RoomMessageTypes;
using UnityEngine;
using UnityEngine.UI;

public class StackTurnStartPanel : MonoBehaviour
{
    public GameObject[] diceRects;

    public Transform stackList;
    public ItemCard stackCard;

    public GameObject stackResultPanel;
    public ItemCard banStackCard;
    public ItemCard goodStackCard;
    public Transform banStackList;
    public Transform goodStackList;
    
    private List<ResultItem> stackItems = new List<ResultItem>();

    public bool isStackTurn = false;

    private SetDiceRollMessage[] diceResult;

    public Image stackPanel;
    
    public Color mineColor;
    public Color enemyColor;
    
    public void InitData(GetStackOnStartMessage m)
    {
        stackPanel.color = UIGameManager.instance.isPlayerTurn ? mineColor : mineColor;
        
        stackResultPanel.SetActive(false);
        stackItems.Clear();
        foreach (var dr in diceRects)
        {
            dr.gameObject.SetActive(false);
        }
        
        diceResult = null;
        if (m.stackList.Length > 0 )
        {
            diceResult = m.diceResult;
            
            UIGameManager.instance.bottomDrawer.gameObject.SetActive(true);
            UIGameManager.instance.bottomDrawer.OpenBottomDrawer();
            
            foreach (var item in m.stackList)
            {
                stackItems.Add(item);
            }
            isStackTurn = true;
            gameObject.SetActive(true);
            InitDice();
            InitStackList(false);
        }
        else
        {
            gameObject.SetActive(false);
            isStackTurn = false;
        }
    }

    private void InitStackList(bool isOld = true)
    {
        for (int i = 1; i < stackList.childCount; i++)
        {
            Destroy(stackList.GetChild(i).gameObject);
        }

        foreach (var result in stackItems)
        {
            int count = UIGameManager.instance.GetStackCount((STACK)result.id, CharacterManager.Instance.SelectedCharacter.State);
            
            count = isOld ? count - 1 : count;
            
            var stack = Instantiate(stackCard, stackList);
            stack.InitImage(GlobalResources.instance.stacks[result.id]);
            stack.InitText(count.ToString());
            stack.gameObject.SetActive(true);
        }
    }
    
    private DICE_TYPE selectedType = DICE_TYPE.DICE_4;

    public void InitDice()
    {
        if (stackItems.Count == 2)
        {
            SetDiceType(1, GetDiceType((STACK) stackItems[0].id), GetIsBanStack((STACK) stackItems[0].id));
            SetDiceType(2, GetDiceType((STACK) stackItems[1].id), GetIsBanStack((STACK) stackItems[1].id));
            
            diceRects[1].gameObject.SetActive(true);
            diceRects[2].gameObject.SetActive(true);
        }
        else
        {
            int i = 0;
            foreach (var stack in stackItems)
            {
                SetDiceType(i, GetDiceType((STACK) stack.id), GetIsBanStack((STACK) stack.id));
                diceRects[i].gameObject.SetActive(true);
                i++;
            }
        }

        StartCoroutine(StartDiceRoll());
    }

    private DICE_TYPE GetDiceType(STACK stackId)
    {
        if(stackId == STACK.Cure || stackId == STACK.Burn || stackId == STACK.Freeze || stackId == STACK.Charge)
        {
            DiceArea.instance.SetDiceType(DICE_TYPE.DICE_4);
            return DICE_TYPE.DICE_4;
        }
        else if(stackId == STACK.Void || stackId == STACK.Slow)
        {
            DiceArea.instance.SetDiceType(DICE_TYPE.DICE_6_4);
            return DICE_TYPE.DICE_6_4;

        } 
        else if (stackId == STACK.PUMP)
        {
            DiceArea.instance.SetDiceType(DICE_TYPE.DICE_6);
            return DICE_TYPE.DICE_6;
        }
        else
        {
            return DICE_TYPE.DICE_4;
        }
    }

    private bool GetIsBanStack(STACK stackId)
    {
        return stackId == STACK.Burn || stackId == STACK.Slow || stackId == STACK.Freeze || stackId == STACK.Void;
    }
    
    private void SetDiceType(int idx, DICE_TYPE type, bool isRed = false)
    {
        Debug.LogError("IsRed : " + isRed);
        if (idx == 0)
        {
            DiceArea.instance.SetDiceType(type, false, isRed);
        }
        else if (idx == 1)
        {
            DiceArea1.instance.SetDiceType(type, false, isRed);
        }
        else
        {
            DiceArea2.instance.SetDiceType(type, false, isRed);
        }
    }

    private void OnLaunchDiceRoll(int idx, int diceIdx)
    {
        if (idx == 0)
        {
            DiceArea.instance.LaunchDice(diceResult[diceIdx].diceData);
        }
        else if (idx == 1)
        {
            DiceArea1.instance.LaunchDice(diceResult[diceIdx].diceData);
        }
        else
        {
            DiceArea2.instance.LaunchDice(diceResult[diceIdx].diceData);
        }
    }
    
    public void OnSelectDice()
    {
        if (stackItems.Count == 2)
        {
            OnLaunchDiceRoll(1, 0);
            OnLaunchDiceRoll(2, 1);
        }
        else
        {
            for (int i = 0; i < stackItems.Count; i++)
            {
                OnLaunchDiceRoll(i, i);
            }            
        }

        diceFinished = false;
    }

    private bool diceFinished = false;
    public void OnFinishDice()
    {
        Debug.Log("OnFinishDice");
        if(!gameObject.activeSelf || diceFinished)
        {
            return;
        }
        Debug.Log("OnFinishDice after finish");

        int i = 0;
        foreach (var result in stackItems)
        {
            EventBus.Publish(
                RoomSendMessageEvent.Create(
                    GlobalDefine.CLIENT_MESSAGE.SET_STACK_ON_START,
                    new RequestStackOnStartMessage
                    {
                        characterId = CharacterManager.Instance.SelectedCharacter.Id,
                        stackId = result.id,
                        diceData = diceResult[i].diceData
                    }
                )
            );
            i++;
        }

        InitStackResult();
        Debug.Log("stack finished");
        StartCoroutine(CheckStackCount());
        diceFinished = true;
    }

    private void InitStackResult()
    {
        for (int j = 1; j < banStackList.childCount; j++)
        {
            Destroy(banStackList.GetChild(j).gameObject);
        }

        for (int k = 0; k < goodStackList.childCount; k++)
        {
            Destroy(goodStackList.GetChild(k).gameObject);
        }
        
        int i = 0;
        foreach (var result in stackItems)
        {
            if (GetIsBanStack((STACK)result.id))
            {
                var item = Instantiate(banStackCard, banStackList);
                item.InitData(GetDiceResult(diceResult[i].diceData).ToString(), GlobalResources.instance.stacks[result.id], true);
                item.gameObject.SetActive(true);
            }
            else
            {
                var item = Instantiate(goodStackCard, goodStackList);
                item.InitData(GetDiceResult(diceResult[i].diceData).ToString(), GlobalResources.instance.stacks[result.id], false);
                item.gameObject.SetActive(true);
            }

            i++;
        }
        stackResultPanel.SetActive(true);
    }

    private int GetDiceResult(DiceData[] diceData)
    {
        int sum = 0;
        foreach (var data in diceData)
        {
            sum += data.diceCount;
        }
        return sum;
    }
    
    IEnumerator StartDiceRoll()
    {
        yield return new WaitForSeconds(0.5f);
        OnSelectDice();
    }

    IEnumerator CheckStackCount()
    {
        yield return new WaitForSeconds(0.5f);
        InitStackList(false);
        yield return new WaitForSeconds(0.5f);
        
        Debug.LogError("finished stack count");
        isStackTurn = false;
        UIGameManager.instance.bottomDrawer.gameObject.SetActive(true);
        gameObject.SetActive (false);
    }
}
