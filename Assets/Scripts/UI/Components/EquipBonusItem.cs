using System.Collections;
using System.Collections.Generic;
using UFB.Network.RoomMessageTypes;
using UnityEngine;
using UnityEngine.UI;

public class EquipBonusItem : MonoBehaviour
{
    public Image bonusImage;
    public Transform bonusList;
    public ItemCard bonusItem;

    public void InitData(EquipBonus bonus)
    {
        bonusImage.sprite = GlobalResources.instance.powers[bonus.id];

        for (int i = 1; i < bonusList.childCount; i++) 
        { 
            Destroy(bonusList.GetChild(i).gameObject);
        }

        if (bonus.items != null) 
        {
            foreach (var item in bonus.items)
            {
                ItemCard it = Instantiate(bonusItem, bonusList);
                it.InitData(item.count.ToString(), GlobalResources.instance.items[item.id]);
                it.gameObject.SetActive(true);
            }
        }

        if (bonus.stacks != null) 
        {
            foreach (var item in bonus.stacks)
            {
                ItemCard it = Instantiate(bonusItem, bonusList);
                it.InitData(item.count.ToString(), GlobalResources.instance.stacks[item.id]);
                it.gameObject.SetActive(true);
            }
        }

        if(bonus.randomItems != null)
        {
            foreach (var item in bonus.randomItems)
            {
                ItemCard it = Instantiate(bonusItem, bonusList);
                it.InitData(item.count.ToString(), GlobalResources.instance.items[item.id]);
                it.gameObject.SetActive(true);
            }
        }

        gameObject.SetActive(true);

    }
}
