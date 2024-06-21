using System.Collections;
using System.Collections.Generic;
using UFB.Events;
using UFB.Items;
using UFB.Network.RoomMessageTypes;
using UFB.StateSchema;
using UFB.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AttackPanel : MonoBehaviour
{
    [HideInInspector]
    public PowerMoveItem powerMoveItem;

    [HideInInspector]
    public PowerMove pm;

    [SerializeField]
    private LinearIndicatorBar _healthBar;

    [SerializeField]
    private LinearIndicatorBar _energyBar;

    [SerializeField]
    private LinearIndicatorBar _ultimateBar;

    [SerializeField]
    Text meleeText;

    [SerializeField]
    Text manaText;

    [SerializeField]
    Text powermoveText;

    [SerializeField]
    Image itemImage;

    [SerializeField]
    Image powermoveImage;

    [SerializeField]
    Transform powerCostList;

    [SerializeField]
    Transform resultList;

    [SerializeField]
    ItemCard costItem;

    [SerializeField]
    ItemCard resultItem;

    public void InitCharacterState(ChangeCharacterStateEvent e)
    {
        InitOthers();
    }

    public void Init(PowerMoveItem _power)
    {
        powerMoveItem = _power;
        pm = powerMoveItem.pm;

        powermoveImage.sprite = GlobalResources.instance.powers[pm.powerImageId];

        powermoveText.text = pm.name.ToString();

        InitCostList();
        InitResultList();
        InitOthers();

        gameObject.SetActive(true);
    }

    public void InitCostList()
    {
        for (int i = 1; i < powerCostList.childCount; i++)
        {
            Destroy(powerCostList.GetChild(i).gameObject);
        }

        foreach (var cost in pm.costList)
        {
            ItemCard itemCard = Instantiate(costItem, powerCostList);
            itemCard.InitDate(cost.count.ToString(), GlobalResources.instance.items[cost.id]);
            itemCard.gameObject.SetActive(true);
        }

        if (pm.coin > 0)
        {
            ItemCard itemCard = Instantiate(costItem, powerCostList);
            itemCard.InitDate(pm.coin.ToString(), GlobalResources.instance.coin);
            itemCard.gameObject.SetActive(true);
        }

        if (pm.light > 0)
        {
            ItemCard itemCard = Instantiate(costItem, powerCostList);
            itemCard.InitDate(pm.light.ToString(), GlobalResources.instance.lightImage);
            itemCard.gameObject.SetActive(true);
        }

        if (pm.range > 0)
        {
            ItemCard itemCard = Instantiate(costItem, powerCostList);
            itemCard.InitDate(pm.range.ToString(), GlobalResources.instance.range);
            itemCard.gameObject.SetActive(true);
        }

    }

    public void InitResultList()
    {
        for (int i = 1; i < resultList.childCount; i++)
        {
            Destroy(resultList.GetChild(i).gameObject);
        }

        PowerMoveResult result = pm.result;
        
        if (result == null) return;

        if (result.coin > 0) 
        {
            ItemCard itemCard = Instantiate(resultItem, resultList);
            itemCard.InitDate(result.coin.ToString(), GlobalResources.instance.coin);
            itemCard.gameObject.SetActive(true);
        }

        if(result.energy > 0)
        {
            ItemCard itemCard = Instantiate(resultItem, resultList);
            itemCard.InitDate(result.energy.ToString(), GlobalResources.instance.energy);
            itemCard.gameObject.SetActive(true);
        }

        if (result.ultimate > 0) 
        {
            ItemCard itemCard = Instantiate(resultItem, resultList);
            itemCard.InitDate(result.ultimate.ToString(), GlobalResources.instance.ultimate);
            itemCard.gameObject.SetActive(true);
        }

        if (result.stacks != null)
        {
            foreach (var stack in result.stacks)
            {
                ItemCard itemCard = Instantiate(costItem, resultList);
                itemCard.InitDate(stack.count.ToString(), GlobalResources.instance.stacks[stack.id]);
                itemCard.gameObject.SetActive(true);
            }
        }

        if (result.items != null)
        {
            foreach (var item in result.items)
            {
                ItemCard itemCard = Instantiate(costItem, resultList);
                itemCard.InitDate(item.count.ToString(), GlobalResources.instance.items[item.id]);
                itemCard.gameObject.SetActive(true);
            }
        }

    }

    public void InitOthers()
    {
        CharacterState state = UIGameManager.instance.controller.State;
        _healthBar.SetRangedValueState(state.stats.health);
        _energyBar.SetRangedValueState(state.stats.energy);
        _ultimateBar.SetRangedValueState(state.stats.ultimate);

        state.items.ForEach(item =>
        {
            ITEM type = (ITEM)item.id;
            if (type == ITEM.Melee)
            {
                meleeText.text = item.count.ToString();
            }
            else if (type == ITEM.Mana)
            {
                manaText.text = item.count.ToString();
            }
        });
    }

    public void OnConfirmBtnClicked()
    {
        UFB.Events.EventBus.Publish(
            RoomSendMessageEvent.Create(
                "setPowerMove",
                new RequestSetPowerMoveItem
                {
                    powerMoveId = pm.id,
                }
            )
        );

        gameObject.SetActive(false);
    }

    public void OnCancelBtnClicked()
    {
        gameObject.SetActive(false);
    }
}
