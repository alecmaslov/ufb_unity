using System.Collections;
using System.Collections.Generic;
using UFB.Items;
using UFB.StateSchema;
using UnityEngine;
using UnityEngine.UI;

public class CraftPanel : MonoBehaviour
{
    public Text coinText;

    public Transform craftList;
    public CraftItem craftItem;

    public void InitData()
    {
        CharacterState state = UIGameManager.instance.controller.State;

        coinText.text = state.stats.coin.ToString();
        state.stats.OnCoinChange((int newCoin, int preCoin) =>
        {
            coinText.text = newCoin.ToString();
        });

        InitList();

    }

    void InitList()
    {
        for (int i = 1; i < craftList.childCount; i++)
        {
            Destroy(craftList.GetChild(i).gameObject);
        }

        foreach (var power in GlobalResources.instance.powerCraftSystem)
        {
            CraftItem card = Instantiate(craftItem, craftList);
            card.InitData((int)power.power1, (int)power.power2, (int)power.powerResult, power.coin, "power");
            card.GetComponent<Button>().onClick.AddListener(() => 
            { 
                OnClickCraftItem(card);
            });
        }

        foreach (var item in GlobalResources.instance.itemCraftSystem)
        {
            CraftItem card = Instantiate(craftItem, craftList);
            card.InitData((int)item.item1, (int)item.item2, (int)item.itemResult, item.coin, "item");
            card.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnClickCraftItem(card);
            });
        }

    }

    public void OnClickCraftItem(CraftItem item)
    {
        UIGameManager.instance.merchantPanel.merchantModalPanel.InitCraft(item);
    }

}
