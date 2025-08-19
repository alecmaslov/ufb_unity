using Colyseus.Schema;
using System.Collections;
using System.Collections.Generic;
using UFB.Items;
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

    public bool isLoadData = false;

    private List<Item> items1 = new List<Item>();
    private List<Item> items2 = new List<Item>();
    private List<Item> powers = new List<Item>();
    private List<Item> stacks = new List<Item>();

    public List<int> boughtItemIds = new List<int>();
    public List<int> boughtPowerIds = new List<int>();
    public List<int> boughtStackIds = new List<int>();
    
    public void InitData()
    {
        Debug.Log("=====>Buy model");
        if(UIGameManager.instance.merchantPanel.itemData1 != null)
        {
            Debug.Log(UIGameManager.instance.merchantPanel.itemData1.Length);
        }
        else
        {
            return;
        }

        CharacterState state = UIGameManager.instance.controller.State;

        coinText.text = state.stats.coin.ToString();

        state.stats.OnCoinChange((int newCoin, int preCoin) => {
            // coinText.text = newCoin.ToString();
            if(preCoin == 0) return;
            StartCoroutine(ChangeCoinAnimation(newCoin, preCoin));
        }, true);


        InitItem(UIGameManager.instance.merchantPanel.itemData1);
        InitItem1(UIGameManager.instance.merchantPanel.itemData2);
        InitPower(UIGameManager.instance.merchantPanel.powerData);
        InitStack(UIGameManager.instance.merchantPanel.stackData);

        isLoadData = true;
    }

    public void ClearBoughtItems()
    {
        boughtItemIds.Clear();
        boughtPowerIds.Clear();
        boughtStackIds.Clear();
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
            if (!boughtItemIds.Exists(item => item == items[i].id))
            {
                Item item = items[i];
                if (item.level == 1 && item.cost > 0)
                {
                    ItemCard it = Instantiate(itemCard, itemList);
                    it.InitData(item.cost.ToString(), GlobalResources.instance.items[item.id]);
                    it.gameObject.SetActive(true);
                    it.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        UIGameManager.instance.merchantPanel.merchantModalPanel.InitData(item, "item");
                        it.gameObject.SetActive(false);
                    });
                }
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
            if (!boughtItemIds.Exists(item => item == items[i].id))
            {
                Item item = items[i];
                if (item.level == 2 && item.cost > 0)
                {
                    ItemCard it = Instantiate(itemCard1, itemList1);
                    it.InitData(item.cost.ToString(), GlobalResources.instance.items[item.id]);
                    it.gameObject.SetActive(true);
                    it.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        UIGameManager.instance.merchantPanel.merchantModalPanel.InitData(item, "item");
                    });
                }
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
            if (!boughtPowerIds.Exists(item => item == powers[i].id))
            {
                Item power = powers[i];
                if (power.level == 1 && power.cost > 0)
                {
                    ItemCard it = Instantiate(powerCard, powerList);
                    it.InitData(power.cost.ToString(), GlobalResources.instance.powers[power.id]);
                    it.gameObject.SetActive(true);
                    it.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        UIGameManager.instance.merchantPanel.merchantModalPanel.InitData(power, "power");
                    });
                }
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
            if (!boughtStackIds.Exists(item => item == stacks[i].id))
            {
                Item stack = stacks[i];
                if (stack.cost > 0 && !(stack.id == (int)STACK.Slow || stack.id == (int)STACK.Void))
                {
                    ItemCard card = Instantiate(stackCard, stackList);
                    card.InitData(stack.cost.ToString(), GlobalResources.instance.stacks[stack.id]);
                    card.gameObject.SetActive(true);
                    card.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        UIGameManager.instance.merchantPanel.merchantModalPanel.InitData(stack, "stack");
                    });
                }
            }
        }
    }

    public void AddBoughtItemId(string itemType, int selectedItemId)
    {
        if (itemType == "item")
        {
            UIGameManager.instance.merchantPanel.buyPanel.boughtItemIds.Add(selectedItemId);
        }
        else if(itemType == "power")
        {
            UIGameManager.instance.merchantPanel.buyPanel.boughtPowerIds.Add(selectedItemId);
        }
        else if(itemType == "stack")
        {
            UIGameManager.instance.merchantPanel.buyPanel.boughtStackIds.Add(selectedItemId);
        }
        
        InitData();
    }
    
    IEnumerator ChangeCoinAnimation(int newCoin, int preCoin)
    {
        Debug.Log($"-----------newCoin : {newCoin}, preCoin : {preCoin}");
        float count = Mathf.Abs(newCoin - preCoin);
        float duration = 1f;
        float delta = duration / count;
        
        Debug.Log($"-----------count : {count}, delta : {delta}");

        for (int i = 1; i <= count; i++)
        {
            yield return new WaitForSeconds(delta);
            
            Debug.Log($"-----------count : {(preCoin - i)}, ddd : {newCoin - preCoin}");

            coinText.text = (preCoin - i).ToString();
            coinText.color = (newCoin - preCoin) < 0 ? Color.red : Color.green;
        }
        yield return new WaitForSeconds(1f);
        coinText.color = Color.white;
        coinText.text = newCoin.ToString();
    }
}
