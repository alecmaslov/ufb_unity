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
        state.stats.OnCoinChange((int newCoin, int preCoin) =>
        {
            if(gameObject.activeSelf && UIGameManager.instance.merchantPanel.gameObject.activeSelf)
                StartCoroutine(ChangeCoinAnimation(newCoin, preCoin));
            else
            {
                coinText.color = Color.white;
                coinText.text = newCoin.ToString();
            }
            //coinText.text = newCoin.ToString();
        });

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
            if(_item.sell > 0 && 
                !(_item.id == (int)ITEM.HeartPiece || 
                _item.id == (int)ITEM.HeartCrystal || 
                _item.id == (int)ITEM.EnergyShard || 
                _item.id == (int)ITEM.EnergyCrystal || 
                _item.id == (int)ITEM.Elixir ||
                _item.id == (int)ITEM.Melee ||
                _item.id == (int)ITEM.Mana ||
                _item.id == (int)ITEM.Quiver)
                )
            {
                ItemCard it = Instantiate(itemCard, itemList);
                it.InitData3(_item.sell.ToString(), GlobalResources.instance.items[_item.id], _item.count.ToString(), _item);

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
                card.InitData3(power.sell.ToString(), GlobalResources.instance.powers[power.id], power.count.ToString(), power);
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
            if(stack.sell > 0 && !(
                stack.id == (int)STACK.Freeze ||
                stack.id == (int)STACK.Burn ||
                stack.id == (int)STACK.Slow ||
                stack.id == (int)STACK.Void 
                ))
            {
                ItemCard card = Instantiate(stackCard, stackList);
                card.InitData3(stack.sell.ToString(), GlobalResources.instance.stacks[stack.id], stack.count.ToString(), stack);

                card.gameObject.SetActive(true);
                card.GetComponent<Button>().onClick.AddListener(() =>
                {
                    UIGameManager.instance.merchantPanel.merchantModalPanel.InitData(stack, "stack", false);
                });
            }
        });
    }

    IEnumerator ChangeCoinAnimation(int newCoin, int preCoin)
    {
        float count = Mathf.Abs(newCoin - preCoin);
        float duration = 1f;
        float delta = duration / count;
        for (int i = 1; i <= count; i++)
        {
            yield return new WaitForSeconds(delta);
            coinText.text = (preCoin + (newCoin - preCoin) / count * i).ToString();
            coinText.color = (newCoin - preCoin) < 0 ? Color.red : Color.green;
        }
        yield return new WaitForSeconds(1f);
        coinText.color = Color.white;
        coinText.text = newCoin.ToString();
    }
}
