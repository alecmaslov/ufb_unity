using System.Collections;
using System.Collections.Generic;
using UFB.Character;
using UFB.Items;
using UFB.Network.RoomMessageTypes;
using UFB.StateSchema;
using UnityEngine;
using UnityEngine.UI;

public class TargetSelectPanel : MonoBehaviour
{
    public ItemCard[] items;

    public ITEM[] bombs;
    public ITEM[] arrows;

    public bool isBomb = false;

    public Image bg;

    public Sprite[] bgs;

    public Text header;

    private PowerMove pm;

    public void InitData(bool isBombed, PowerMove _pm)
    {
        pm = new PowerMove();
        pm.id = _pm.id;
        pm.name = _pm.name;
        pm.powerImageId = _pm.powerImageId;
        pm.result = new PowerMoveResult();
        pm.result.stacks = _pm.result.stacks;
        pm.extraItemId = _pm.extraItemId;
        isBomb = isBombed;

        int total = 0;
        int limit = 4;

        foreach (var item in items)
        {
            item.GetComponent<Button>().onClick.RemoveAllListeners();
            item.InitText("0");
        }

        if (isBomb)
        {
            limit = UIGameManager.instance.controller.State.stats.bombLimit;
            bg.sprite = bgs[0];
            int k = 0;
            foreach (var bomb in bombs)
            {

                int count = 0;
                UIGameManager.instance.controller.State.items.ForEach(item =>
                {
                    if (item.id == (int)bomb)
                    {
                        count = item.count;
                    }
                });
                total += count;
                items[k].InitDate(count.ToString(), GlobalResources.instance.items[(int)bomb]);

                if(count > 0)
                {
                    items[k].GetComponent<Button>().onClick.AddListener(() =>
                    {
                        OnClickItem(bomb);
                    });
                }

                k++;
            }

            header.text = $"BOMBS ({total}/{limit})";

        }
        else
        {
            limit = UIGameManager.instance.controller.State.stats.arrowLimit;

            bg.sprite = bgs[1];

            int k = 0;
            foreach (var arrow in arrows)
            {

                int count = 0;
                UIGameManager.instance.controller.State.items.ForEach(item =>
                {
                    if (item.id == (int)arrow)
                    {
                        count = item.count;
                    }
                });

                total += count;

                items[k].InitDate(count.ToString(), GlobalResources.instance.items[(int)arrow]);

                if(count > 0)
                {
                    items[k].GetComponent<Button>().onClick.AddListener(() =>
                    {
                        OnClickItem(arrow);
                    });
                }

                k++;
            }

            header.text = $"ARROWS ({total}/{limit})";
        }



        gameObject.SetActive(true);
    }
 
    private void OnClickItem(ITEM itemType)
    {
        pm.extraItemId = (int)itemType;

        // Create a new array with one additional slot
        ResultItem[] newStacks = new ResultItem[pm.result.stacks == null? 1 : (pm.result.stacks.Length + 1)];

        // Copy existing items to the new array
        if(pm.result.stacks != null)
        {
            for (int i = 0; i < pm.result.stacks.Length; i++)
            {
                newStacks[i] = pm.result.stacks[i];
            }
        }

        ResultItem resultItem = new ResultItem();

        if (itemType == ITEM.FireArrow)
        {
            resultItem.id = (int)STACK.Burn;
            resultItem.count = 1;
            newStacks[newStacks.Length - 1] = resultItem;
            pm.result.stacks = newStacks;
        }
        else if (itemType == ITEM.BombArrow)
        {
            pm.result.perkId = (int)PERK.PULL;
        }
        else if (itemType == ITEM.IceArrow)
        {
            resultItem.id = (int)STACK.Freeze;
            resultItem.count = 1;
            newStacks[newStacks.Length - 1] = resultItem;
            pm.result.stacks = newStacks;
        }
        else if (itemType == ITEM.VoidArrow)
        {
            resultItem.id = (int)STACK.Void;
            resultItem.count = 1;
            newStacks[newStacks.Length - 1] = resultItem;
            pm.result.stacks = newStacks;
        }
        else if (itemType == ITEM.FireBomb)
        {
            resultItem.id = (int)STACK.Burn;
            resultItem.count = 1;
            newStacks[newStacks.Length - 1] = resultItem;
            pm.result.stacks = newStacks;
        }
        else if (itemType == ITEM.IceBomb) 
        {
            resultItem.id = (int)STACK.Freeze;
            resultItem.count = 1;
            newStacks[newStacks.Length - 1] = resultItem;
            pm.result.stacks = newStacks;
        }
        else if (itemType == ITEM.VoidBomb) 
        {
            resultItem.id = (int)STACK.Void;
            resultItem.count = 1;
            newStacks[newStacks.Length - 1] = resultItem;
            pm.result.stacks = newStacks;
        }


        UIGameManager.instance.attackPanel.Init(pm);
        gameObject.SetActive(false);
    }

}
