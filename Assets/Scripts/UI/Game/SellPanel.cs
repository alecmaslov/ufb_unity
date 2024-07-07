using System.Collections;
using System.Collections.Generic;
using UFB.Items;
using UFB.StateSchema;
using UnityEngine;
using UnityEngine.UI;

public class SellPanel : MonoBehaviour
{
    public Transform itemList;
    public ItemCard itemCard;

    public Transform powerList;
    public ItemCard powerCard;

    public Transform stackList;
    public ItemCard stackCard;

    public Text coinText;

    public void InitData()
    {
        CharacterState state = UIGameManager.instance.controller.State;

        coinText.text = state.stats.coin.ToString();

        InitItem(state);
        InitPower(state);
        InitStack(state);
    }

    public void InitItem(CharacterState state)
    {
        for (int i = 1; i < itemList.childCount; i++)
        {
            Destroy(itemList.GetChild(i).gameObject);
        }

        state.items.ForEach(_item => 
        { 
            if(_item.sell > 0)
            {
                ItemCard it = Instantiate(itemCard, itemList);
                it.InitDate(_item.cost.ToString(), GlobalResources.instance.items[_item.id]);
                it.gameObject.SetActive(true);
                it.GetComponent<Button>().onClick.AddListener(() =>
                {
                    UIGameManager.instance.merchantPanel.merchantModalPanel.InitData(_item, "item", false);
                });
            }

        });
    }

    public void InitPower(CharacterState state)
    {
        for (int i = 1; i < powerList.childCount; i++)
        {
            Destroy(powerList.GetChild(i).gameObject);
        }

        state.powers.ForEach(power => 
        { 
            if(power.sell > 0)
            {
                ItemCard card = Instantiate(powerCard, powerList);
                card.InitDate(power.cost.ToString(), GlobalResources.instance.powers[power.id]);
                card.gameObject.SetActive(true);
                card.GetComponent<Button>().onClick.AddListener(() =>
                {
                    UIGameManager.instance.merchantPanel.merchantModalPanel.InitData(power, "power", false);
                });
            }
        });
    }

    public void InitStack(CharacterState state)
    {
        for (int i = 1; i < stackList.childCount; i++)
        {
            Destroy(stackList.GetChild(i).gameObject);
        }

        state.stacks.ForEach(stack => 
        {
            if(stack.sell > 0)
            {
                ItemCard card = Instantiate(stackCard, stackList);
                card.InitDate(stack.cost.ToString(), GlobalResources.instance.stacks[stack.id]);
                card.gameObject.SetActive(true);
                card.GetComponent<Button>().onClick.AddListener(() =>
                {
                    UIGameManager.instance.merchantPanel.merchantModalPanel.InitData(stack, "stack", false);
                });
            }
        });
    }
}
