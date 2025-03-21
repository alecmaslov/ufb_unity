using System.Collections;
using System.Collections.Generic;
using UFB.Items;
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

    public Transform resultList;
    
    [SerializeField]
    ItemCard card;

    public PowerMove pm;

    public void Init(PowerMove powerMove)
    {
        pm = powerMove;
        InitCostList();
        CharacterState state = UIGameManager.instance.controller.State;

        moveName.text = powerMove.name;
        if (image != null) 
        { 
            image.sprite = GlobalResources.instance.powers[powerMove.powerImageId];
        }
        

        foreach (var cost in powerMove.costList)
        {
            ItemCard itemCard = Instantiate(card, costList);
            itemCard.InitDate(cost.count.ToString(), GlobalResources.instance.items[cost.id], UIGameManager.instance.GetItemCount((ITEM) cost.id) < cost.count);
            itemCard.gameObject.SetActive(true);
        }

        if(powerMove.coin > 0)
        {
            ItemCard itemCard = Instantiate(card, costList);
            itemCard.InitDate(powerMove.coin.ToString(), GlobalResources.instance.coin, state.stats.coin < powerMove.coin);
            itemCard.gameObject.SetActive(true);
        }

        if(powerMove.light > 0)
        {
            ItemCard itemCard = Instantiate(card, costList);
            itemCard.InitDate(powerMove.light.ToString(), GlobalResources.instance.lightImage, state.stats.energy.current < powerMove.light);
            itemCard.gameObject.SetActive(true);
        }
        
        if(powerMove.range > 0)
        {
            int currectRange = UIGameManager.instance.bottomAttackPanel.GetRange(state);
            Debug.Log("cost range: " + currectRange);
            ItemCard itemCard = Instantiate(card, costList);
            itemCard.InitDate(powerMove.range.ToString(), GlobalResources.instance.range, currectRange > powerMove.range);
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

    public void InitResultList()
    {
        for (int i = 0; i < resultList.childCount; i++)
        {
            Destroy(resultList.GetChild(i).gameObject);
        }

        if (pm.result != null)
        {
            if (pm.result.dice > 0)
            {
                ItemCard itemCard = Instantiate(card, resultList);
                itemCard.InitImage(GlobalResources.instance.health);
                itemCard.gameObject.SetActive(true);

                if ((DICE_TYPE) pm.result.dice == DICE_TYPE.DICE_4)
                {
                    var cd = Instantiate(card, resultList);
                    cd.InitImage(GlobalResources.instance.dice[1]);
                    cd.gameObject.SetActive(true);
                } 
                else if ((DICE_TYPE) pm.result.dice == DICE_TYPE.DICE_6)
                {
                    var cd = Instantiate(card, resultList);
                    cd.InitImage(GlobalResources.instance.dice[0]);
                    cd.gameObject.SetActive(true);
                }
                else if ((DICE_TYPE) pm.result.dice == DICE_TYPE.DICE_6_4)
                {
                    var cd = Instantiate(card, resultList);
                    cd.InitImage(GlobalResources.instance.dice[0]);
                    cd.gameObject.SetActive(true);
                    var cd1 = Instantiate(card, resultList);
                    cd1.InitImage(GlobalResources.instance.dice[1]);
                    cd1.gameObject.SetActive(true);
                }
                else if ((DICE_TYPE) pm.result.dice == DICE_TYPE.DICE_6_6)
                {
                    var cd = Instantiate(card, resultList);
                    cd.InitImage(GlobalResources.instance.dice[0]);
                    cd.gameObject.SetActive(true);
                    var cd1 = Instantiate(card, resultList);
                    cd1.InitImage(GlobalResources.instance.dice[0]);
                    cd1.gameObject.SetActive(true);
                }
            }
            
            if (pm.result.items != null && pm.result.items.Length > 0)
            {
                foreach (var item in pm.result.items)
                {
                    var cd = Instantiate(card, resultList);
                    cd.InitImage(GlobalResources.instance.items[item.id]);
                    cd.gameObject.SetActive(true);
                }
            }

            if (pm.result.stacks != null && pm.result.stacks.Length > 0)
            {
                foreach (var item in pm.result.stacks)
                {
                    var cd = Instantiate(card, resultList);
                    cd.InitImage(GlobalResources.instance.stacks[item.id]);
                    cd.gameObject.SetActive(true);
                }
            }

            if (pm.result.coin > 0)
            {
                ItemCard itemCard = Instantiate(card, resultList);
                itemCard.InitImage(GlobalResources.instance.coin);
                itemCard.gameObject.SetActive(true);
            }

            if (pm.result.energy > 0)
            {
                ItemCard itemCard = Instantiate(card, resultList);
                itemCard.InitImage(GlobalResources.instance.energy);
                itemCard.gameObject.SetActive(true);
            }
            
            if (pm.result.health > 0)
            {
                ItemCard itemCard = Instantiate(card, resultList);
                itemCard.InitImage(GlobalResources.instance.health);
                itemCard.gameObject.SetActive(true);
            }
            
            if (pm.result.ultimate > 0)
            {
                ItemCard itemCard = Instantiate(card, resultList);
                itemCard.InitImage(GlobalResources.instance.ultimate);
                itemCard.gameObject.SetActive(true);
            }
            
            if (pm.result.perkId > 0)
            {
                ItemCard itemCard = Instantiate(card, resultList);
                itemCard.InitImage(GlobalResources.instance.perks[pm.result.perkId]);
                itemCard.gameObject.SetActive(true);
            }
            
            if (pm.result.perkId1 > 0)
            {
                ItemCard itemCard = Instantiate(card, resultList);
                itemCard.InitImage(GlobalResources.instance.perks[pm.result.perkId1]);
                itemCard.gameObject.SetActive(true);
            }
        }
    }
    
    public void OnClickPowermove()
    {
        if (IsPowermoveEnabled())
        {
            UIGameManager.instance.bottomAttackPanel.OnClickPowermoveItem(pm);
        }
        else
        {
            UIGameManager.instance.OnNotificationMessage("error", "You cannot power move this item.");
        }
    }

    public bool IsPowermoveEnabled()
    {
        bool isPowermoveEnabled = true;
        CharacterState state = UIGameManager.instance.controller.State;
        
        foreach (var cost in pm.costList)
        {
            int count = UIGameManager.instance.GetItemCount((ITEM)cost.id);
            if (count < cost.cost)
            {
                isPowermoveEnabled = false;
            }
        }

        int currectRange = UIGameManager.instance.bottomAttackPanel.GetRange(state);

        if(state.stats.coin < pm.coin || state.stats.energy.current < pm.light || (currectRange > pm.range && pm.range > 0))
        {
            isPowermoveEnabled = false;
        }
        
        return isPowermoveEnabled;
    }
}
