using System.Collections;
using System.Collections.Generic;
using UFB.Network.RoomMessageTypes;
using UFB.StateSchema;
using UnityEngine;
using UnityEngine.UI;

public class PowerMoveItem : MonoBehaviour
{

    [SerializeField]
    Text moveName;

    [SerializeField]
    Image image;

    [SerializeField]
    Transform costList;

    [SerializeField]
    ItemCard card;

    public void Init(PowerMove powerMove)
    {
        InitCostList();

        moveName.text = powerMove.name;
        image.sprite = GlobalResources.instance.powers[powerMove.powerImageId];

        foreach (var cost in powerMove.costList)
        {
            ItemCard itemCard = Instantiate(card, costList);
            itemCard.InitDate(cost.count.ToString(), GlobalResources.instance.items[cost.id]);
            itemCard.gameObject.SetActive(true);
        }

        if(powerMove.coin > 0)
        {
            ItemCard itemCard = Instantiate(card, costList);
            itemCard.InitDate(powerMove.coin.ToString(), GlobalResources.instance.coin);
            itemCard.gameObject.SetActive(true);
        }

        if(powerMove.light > 0)
        {
            ItemCard itemCard = Instantiate(card, costList);
            itemCard.InitDate(powerMove.light.ToString(), GlobalResources.instance.lightImage);
            itemCard.gameObject.SetActive(true);
        }
        
        if(powerMove.range > 0)
        {
            ItemCard itemCard = Instantiate(card, costList);
            itemCard.InitDate(powerMove.range.ToString(), GlobalResources.instance.range);
            itemCard.gameObject.SetActive(true);
        }
    }

    private void InitCostList()
    {
        for (int i = 1; i < costList.childCount; i++)
        {
            Destroy(costList.GetChild(i).gameObject);
        }
    }
}
