using System.Collections;
using System.Collections.Generic;
using UFB.Items;
using UFB.StateSchema;
using UnityEngine;
using UnityEngine.UI;
using static GlobalResources;

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
            //if(preCoin == 0) return;
            if(gameObject.activeSelf && UIGameManager.instance.merchantPanel.gameObject.activeSelf)
                StartCoroutine(ChangeCoinAnimation(newCoin, preCoin));
            else
            {
                coinText.text = newCoin.ToString();
            }
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
            if(CheckCraftPower(power))
            {
                CraftItem card = Instantiate(craftItem, craftList);
                card.InitData((int)power.power1, (int)power.power2, (int)power.powerResult, power.coin, "power");
                card.GetComponent<Button>().onClick.AddListener(() =>
                {
                    OnClickCraftItem(card);
                });
            }
        }

        foreach (var item in GlobalResources.instance.itemCraftSystem)
        {
            if(CheckCraftItem(item))
            {
                CraftItem card = Instantiate(craftItem, craftList);
                card.InitData((int)item.item1, (int)item.item2, (int)item.itemResult, item.coin, "item");
                card.GetComponent<Button>().onClick.AddListener(() =>
                {
                    OnClickCraftItem(card);
                });
            }
        }

    }

    public bool CheckCraftPower( CraftPowerSystem powerCraft )
    {
        CharacterState state = UIGameManager.instance.controller.State;

        bool isStatus1 = false;
        bool isStatus2 = false;

        state.powers.ForEach(p => 
        { 
            if(p.id == (int) powerCraft.power1 && p.count > 0)
            {
                isStatus1 = true;
            }
            if (p.id == (int)powerCraft.power2 && p.count > 0)
            {
                isStatus2 = true;
            }

            if (powerCraft.power1 == powerCraft.power2)
            {
                isStatus1 = p.count > 1;
            }
        });

        return isStatus1 && isStatus2;
    }

    public bool CheckCraftItem(CraftItemSystem craftItemSystem) 
    {
        CharacterState state = UIGameManager.instance.controller.State;

        bool isStatus1 = false;
        bool isStatus2 = false;

        state.items.ForEach(p =>
        {
            if (p.id == (int)craftItemSystem.item1 && p.count > 0)
            {
                isStatus1 = true;
            }
            if (p.id == (int)craftItemSystem.item2 && p.count > 0)
            {
                isStatus2 = true;
            }

            if (craftItemSystem.item1 == craftItemSystem.item2)
            {
                isStatus1 = p.count > 1;
            }
        });

        return isStatus1 && isStatus2;
    }

    public void OnClickCraftItem(CraftItem item)
    {
        UIGameManager.instance.merchantPanel.merchantModalPanel.InitCraft(item);
    }

    IEnumerator ChangeCoinAnimation(int newCoin, int preCoin)
    {
        float count = Mathf.Abs(newCoin - preCoin);
        float duration = 1f;
        float delta = duration / count;
        for (int i = 1; i <= count; i++)
        {
            yield return new WaitForSeconds(delta);
            coinText.text = (preCoin - i).ToString();
            coinText.color = (newCoin - preCoin) < 0 ? Color.red : Color.green;
        }
        yield return new WaitForSeconds(1f);
        coinText.color = Color.white;
        coinText.text = newCoin.ToString();
    }
    
}
