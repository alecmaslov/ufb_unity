using System.Collections;
using System.Collections.Generic;
using UFB.Network.RoomMessageTypes;
using UnityEngine;

public class RewardBonusPanel : MonoBehaviour
{
    public Transform bonusList;
    public ItemCard bonusItem;

    public void InitData(RewardBonusMessage bonus)
    {
        InitList();


        if (bonus.items != null)
        {
            foreach (var item in bonus.items)
            {
                ItemCard it = Instantiate(bonusItem, bonusList);
                it.InitDate(item.count.ToString(), GlobalResources.instance.items[item.id]);
                it.gameObject.SetActive(true);
            }
        }

        if (bonus.stacks != null)
        {
            foreach (var item in bonus.stacks)
            {
                ItemCard it = Instantiate(bonusItem, bonusList);
                it.InitDate(item.count.ToString(), GlobalResources.instance.stacks[item.id]);
                it.gameObject.SetActive(true);
            }
        }

        if (bonus.powers != null)
        {
            foreach (var item in bonus.powers)
            {
                ItemCard it = Instantiate(bonusItem, bonusList);
                it.InitDate(item.count.ToString(), GlobalResources.instance.powers[item.id]);
                it.gameObject.SetActive(true);
            }
        }

        if (bonus.coin > 0)
        {
            ItemCard it = Instantiate(bonusItem, bonusList);
            it.InitDate(bonus.coin.ToString(), GlobalResources.instance.coin);
            it.gameObject.SetActive(true);
        }

        gameObject.SetActive(true);
    }

    void InitList()
    {
        for (int i = 1; i < bonusList.childCount; i++) 
        {
            Destroy(bonusList.GetChild(i).gameObject );
        }
    }
}
