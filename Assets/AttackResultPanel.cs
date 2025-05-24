using System.Collections;
using System.Collections.Generic;
using UFB.Items;
using UFB.Network.RoomMessageTypes;
using UnityEngine;

public class AttackResultPanel : MonoBehaviour
{
    public ItemCard enemyDice;

    public Transform playerDiceList;
    public ItemCard playerDice;

    public ItemCard stackItem;
    public Transform stackItemList;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void InitDiceData(DiceData[] diceData, Sprite pSprite)
    {
        foreach (var data in diceData)
        {
            ItemCard tempCard = Instantiate(playerDice, playerDiceList);
            tempCard.InitDate(data.diceCount.ToString(), pSprite);
            tempCard.gameObject.SetActive(true);
        }
        
        gameObject.SetActive(true);
    }

    public void InitEnemyDice(int diceCount, Sprite pSprite)
    {
        enemyDice.InitText(diceCount.ToString());
        enemyDice.InitImage(pSprite);
        enemyDice.gameObject.SetActive(true);
    }
    
    public void InitPowerMoveResult(PowerMove pm)
    {
        ClearStack();
        
        if (pm.result.items != null && pm.result.items.Length > 0)
        {
            foreach (var item in pm.result.items)
            {
                AddResultItem(item.count, GlobalResources.instance.items[item.id], false);
            }
        }

        if (pm.result.stacks != null && pm.result.stacks.Length > 0)
        {
            foreach (var item in pm.result.stacks)
            {
                AddResultItem(item.count, GlobalResources.instance.stacks[item.id], false);
            }
        }
        
        if (pm.result.perkId > -1)
        {
            AddResultItem(0, GlobalResources.instance.perks[pm.result.perkId], false);
        }
            
        if (pm.result.perkId1 > -1)
        {
            AddResultItem(0, GlobalResources.instance.perks[pm.result.perkId1], false);
        }
        
        if (pm.extraItemId >= 0)
        {
            ITEM _type = (ITEM) pm.extraItemId;

            if (_type == ITEM.Arrow)
            {
            }
            else if (_type == ITEM.BombArrow)
            {
                AddResultItem(0, GlobalResources.instance.perks[(int)PERK.PUSH], false);
            }
            else if (_type == ITEM.FireArrow)
            {
                AddResultItem(1, GlobalResources.instance.stacks[(int)STACK.Burn], false);
            }
            else if (_type == ITEM.IceArrow)
            {
                AddResultItem(1, GlobalResources.instance.stacks[(int)STACK.Freeze], false);
            }
            else if (_type == ITEM.VoidArrow)
            {
                AddResultItem(1, GlobalResources.instance.stacks[(int)STACK.Void], false);
            }
            else if (_type == ITEM.Bomb)
            {
            }
            else if (_type == ITEM.caltropBomb)
            {
            }
            else if (_type == ITEM.FireBomb)
            {
                AddResultItem(1, GlobalResources.instance.stacks[(int)STACK.Burn], false);
            }
            else if (_type == ITEM.IceBomb)
            {
                AddResultItem(1, GlobalResources.instance.stacks[(int)STACK.Freeze], false);
            }
            else if (_type == ITEM.VoidBomb)
            {
                AddResultItem(1, GlobalResources.instance.stacks[(int)STACK.Void], false); 
            }
        }
        
        gameObject.SetActive(true);
    }

    public void InitBombResult(ItemResult result)
    {
        gameObject.SetActive(true);
        StartCoroutine(CheckBombResult(result));
    }
    
    public void AddResultItem(int count, Sprite sprite, bool isText = true, Color? color = null)
    {
        ItemCard itemCard = Instantiate(stackItem, stackItemList);
        itemCard.InitText(count.ToString());
        itemCard.InitImage(sprite);
        if(itemCard.countText != null)
            itemCard.countText.gameObject.SetActive(isText);

        if (color != null)
        {
            itemCard.InitTextBG(color.Value);
        }
        
        itemCard.gameObject.SetActive(true);
    }
    
    public void CloseAttackResult()
    {
        ClearResult();
        gameObject.SetActive(false);
    }
    
    public void ClearResult()
    {
        enemyDice.gameObject.SetActive(false);
        ClearDiceList();
        ClearStack();
    }

    public void ClearDiceList()
    {
        for (int i = 1; i < playerDiceList.childCount; i++)
        {
            Destroy(playerDiceList.GetChild(i).gameObject);
        }
    }

    public void ClearStack()
    {
        for (int i = 1; i < stackItemList.childCount; i++)
        {
            Destroy(stackItemList.GetChild(i).gameObject);
        }
    }

    List<ToastBanStackMessage> banStackMessages = new List<ToastBanStackMessage>();
    public void InitBanStack(ToastBanStackMessage e)
    {
        Debug.LogError($"init ban stack message: sssss");
        banStackMessages.Add(e);
    }
    
    IEnumerator CheckBombResult(ItemResult result)
    {
        ClearStack();
        yield return new WaitForSeconds(1f);

        CheckBombStack(result);
        
        yield return new WaitForSeconds(1f);
        
        ClearStack();
        CheckBombStack(result, false);

        yield return new WaitForSeconds(2f);
        ClearStack();
        banStackMessages.Clear();
        gameObject.SetActive(false);
    }

    void CheckBombStack(ItemResult result, bool isOld = true)
    {
        bool isBanStack = false;
        banStackMessages.ForEach(msg =>
        {
            if (result.stackId == msg.stack1)
            {
                isBanStack = true;
                int banStackId = msg.stack2;
                int count = UIGameManager.instance.GetStackCount((STACK) banStackId);
                AddResultItem(isOld ? count + 1 : count, GlobalResources.instance.stacks[banStackId], true, isOld? Color.gray : new Color(0.735849f, 0, 0));
                
                UIGameManager.instance.movePanel.InitBomBList(result, true);
            }
        });
        
        if (!isBanStack)
        {
            if (result.stackId > -1)
            {
                int count = UIGameManager.instance.GetStackCount((STACK) result.stackId);
                AddResultItem(isOld ? Mathf.Max(0, count - 1) : count , GlobalResources.instance.stacks[result.stackId], true, isOld? Color.gray : Color.green);
            }
        }
    }
    
}
