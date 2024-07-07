using System.Collections;
using System.Collections.Generic;
using UFB.StateSchema;
using UnityEngine;
using UnityEngine.UI;

public class BuyPanel : MonoBehaviour
{
    public Transform itemList;
    public ItemCard itemCard;

    public Transform itemList1;
    public ItemCard itemCard1;

    public Transform powerList;
    public ItemCard powerCard;

    public Transform stackList;
    public ItemCard stackCard;

    public Text coinText;

    public void InitData()
    {
        Debug.Log("=====>Buy model");
        if(UIGameManager.instance.merchantPanel.itemData != null)
        {
            Debug.Log(UIGameManager.instance.merchantPanel.itemData.Length);
        }

        CharacterState state = UIGameManager.instance.controller.State;

        coinText.text = state.stats.coin.ToString();

        InitItem(UIGameManager.instance.merchantPanel.itemData);
        InitItem1(UIGameManager.instance.merchantPanel.itemData);
        InitPower(UIGameManager.instance.merchantPanel.powerData);
        InitStack(UIGameManager.instance.merchantPanel.stackData);
    }

    public void InitItem(Item[] items)
    {
        if (items == null || items.Length == 0) return;

        for (int i = 1; i < itemList.childCount; i++)
        {
            Destroy(itemList.GetChild(i).gameObject);
        }

        for (int i = 0; i < items.Length; i++)
        {
            Item item = items[i];
            if (item.level == 1 && item.cost > 0) 
            {
                ItemCard it = Instantiate(itemCard, itemList);
                it.InitDate(item.cost.ToString(), GlobalResources.instance.items[item.id]);
                it.gameObject.SetActive(true);
                it.GetComponent<Button>().onClick.AddListener(() =>
                {
                    UIGameManager.instance.merchantPanel.merchantModalPanel.InitData(item, "item");
                });
            }
        }
    }

    public void InitItem1(Item[] items)
    {
        if (items == null || items.Length == 0) return;

        for (int i = 1; i < itemList1.childCount; i++)
        {
            Destroy(itemList1.GetChild(i).gameObject);
        }

        for (int i = 0; i < items.Length; i++)
        {
            Item item = items[i];
            if (item.level == 2 && item.cost > 0)
            {
                ItemCard it = Instantiate(itemCard1, itemList1);
                it.InitDate(item.cost.ToString(), GlobalResources.instance.items[item.id]);
                it.gameObject.SetActive(true);
                it.GetComponent<Button>().onClick.AddListener(() =>
                {
                    UIGameManager.instance.merchantPanel.merchantModalPanel.InitData(item, "item");
                });
            }
        }
    }

    public void InitPower(Item[] powers)
    {
        if (powers == null || powers.Length == 0) return;

        for (int i = 1; i < powerList.childCount; i++)
        {
            Destroy(powerList.GetChild(i).gameObject);
        }

        for (int i = 0; i < powers.Length; i++)
        {
            Item power = powers[i];
            if (power.level == 1 && power.cost > 0)
            {
                ItemCard it = Instantiate(powerCard, powerList);
                it.InitDate(power.cost.ToString(), GlobalResources.instance.powers[power.id]);
                it.gameObject.SetActive(true);
                it.GetComponent<Button>().onClick.AddListener(() =>
                {
                    UIGameManager.instance.merchantPanel.merchantModalPanel.InitData(power, "power");
                });
            }
        }

    }

    public void InitStack(Item[] stacks)
    {
        if (stacks == null || stacks.Length == 0) return;

        for (int i = 1; i < stackList.childCount; i++)
        {
            Destroy(stackList.GetChild(i).gameObject);
        }

        for(int i = 0; i < stacks.Length; i++)
        {
            Item stack = stacks[i];
            if (stack.cost > 0) 
            {
                ItemCard card = Instantiate(stackCard, stackList);
                card.InitDate(stack.cost.ToString(), GlobalResources.instance.stacks[stack.id]);
                card.gameObject.SetActive(true);
                card.GetComponent<Button>().onClick.AddListener(() =>
                {
                    UIGameManager.instance.merchantPanel.merchantModalPanel.InitData(stack, "stack");
                });
            }
        }

    }
}
