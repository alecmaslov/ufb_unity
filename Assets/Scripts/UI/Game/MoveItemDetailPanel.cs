using System.Collections;
using System.Collections.Generic;
using UFB.Items;
using UFB.Map;
using UnityEngine;
using UnityEngine.UI;

public class MoveItemDetailPanel : MonoBehaviour
{
    public ItemCard[] itemPrefab;

    public Text posText;

    public Tile tile;

    public void Init() {

        posText.text = tile.TilePosText;

        List<ITEM> bombs = new List<ITEM>
        {
            ITEM.Bomb,
            ITEM.IceBomb,
            ITEM.VoidBomb,
            ITEM.FireBomb,
            ITEM.caltropBomb,
        };

        UIGameManager.instance.controller.State.items.ForEach(item =>
        {
            ITEM type = (ITEM)item.id;
            if (type == ITEM.Bomb) 
            {
                itemPrefab[0].InitDate(item.count.ToString(), GlobalResources.instance.items[item.id]);
                itemPrefab[0].item = item;
            }
            else if (type == ITEM.caltropBomb)
            {
                itemPrefab[1].InitDate(item.count.ToString(), GlobalResources.instance.items[item.id]);
                itemPrefab[1].item = item;
            }
            else if (type == ITEM.FireBomb)
            {
                itemPrefab[2].InitDate(item.count.ToString(), GlobalResources.instance.items[item.id]);
                itemPrefab[2].item = item;
            }
            else if (type == ITEM.IceBomb)
            {
                itemPrefab[3].InitDate(item.count.ToString(), GlobalResources.instance.items[item.id]);
                itemPrefab[3].item = item;
            }
            else if (type == ITEM.VoidBomb)
            {
                itemPrefab[4].InitDate(item.count.ToString(), GlobalResources.instance.items[item.id]);
                itemPrefab[4].item = item;
            }

        });

        gameObject.SetActive(true);
    }

    public void OnBombClick(int idx)
    {
        if (itemPrefab[idx] != null && itemPrefab[idx].item != null) 
        {
            ITEM type = (ITEM) itemPrefab[idx].item.id;
            UIGameManager.instance.movePanel.OnSetMoveItem(tile, itemPrefab[idx].item.id);
        } 
        else
        {
            Debug.Log("Item count is zero.");
        }
    }

    public void OnFeatherClick()
    {
        //Init(ITEM.Feather);
    }

    public void OnCrystalClick()
    {
        //Init(ITEM.WarpCrystal);
    }
}
