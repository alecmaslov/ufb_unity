using System.Collections;
using System.Collections.Generic;
using UFB.Items;
using UFB.Network.RoomMessageTypes;
using UnityEngine;

public class RewardBonusPanel : MonoBehaviour
{
    public Transform bonusList;
    public ItemCard bonusItem;

    private RewardBonusMessage bonus;
    
    public void InitData(RewardBonusMessage _bonus)
    {
        bonus = _bonus;
        InitList();
        gameObject.SetActive(true);
        StartCoroutine(CheckBonusPanel());
    }

    void InitData(bool isOld = false)
    {
        InitList();
        if (bonus.items != null)
        {
            foreach (var item in bonus.items)
            {
                var count = UIGameManager.instance.GetItemCount((ITEM) item.id);
                if (isOld)
                {
                    count -= item.count;
                    count = Mathf.Max(0, count);
                }
                
                AddResultItem(count.ToString(), GlobalResources.instance.items[item.id], isOld? Color.white : item.count > 0? Color.green : Color.red );
            }
        }

        if (bonus.stacks != null)
        {
            foreach (var item in bonus.stacks)
            {
                var count = UIGameManager.instance.GetStackCount((STACK) item.id);
                if (isOld)
                {
                    count -= item.count;
                    count = Mathf.Max(0, count);
                }
                
                AddResultItem(count.ToString(), GlobalResources.instance.stacks[item.id], isOld? Color.white : item.count > 0? Color.green : Color.red);
            }
        }

        if (bonus.powers != null)
        {
            foreach (var item in bonus.powers)
            {
                var count = UIGameManager.instance.GetPowerCount((POWER) item.id);
                if (isOld)
                {
                    count -= item.count;
                    count = Mathf.Max(0, count);
                }
                AddResultItem(count.ToString(), GlobalResources.instance.powers[item.id], isOld? Color.white : item.count > 0? Color.green : Color.red );
            }
        }

        if (bonus.coin > 0)
        {
            var count = UIGameManager.instance.controller.State.stats.coin;
            if (isOld)
            {
                count -= bonus.coin;
                count = Mathf.Max(0, count);
            }
            
            AddResultItem(count.ToString(), GlobalResources.instance.coin, isOld? Color.white : bonus.coin > 0? Color.green : Color.red );
        }
    }

    void AddResultItem(string count, Sprite icon, Color? color)
    {
        ItemCard it = Instantiate(bonusItem, bonusList);
        it.InitDate(count, icon, false, true);
        it.InitTextBG(color.Value);
        it.gameObject.SetActive(true);
    }
    
    void InitList()
    {
        for (int i = 1; i < bonusList.childCount; i++) 
        {
            Destroy(bonusList.GetChild(i).gameObject );
        }
    }

    IEnumerator CheckBonusPanel()
    {
        yield return new WaitForSeconds(0.5f);
        InitData(true);
        yield return new WaitForSeconds(1f);
        InitData();
    }
}
