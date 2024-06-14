using Colyseus.Schema;
using System.Collections;
using System.Collections.Generic;
using UFB.Items;
using UFB.StateSchema;
using UnityEngine;

public class GlobalResources : MonoBehaviour
{
    public static GlobalResources instance;

    public Sprite[] items;
    public Sprite[] powers;
    public Sprite[] stacks;

    public Sprite coin;
    public Sprite range;
    public Sprite lightImage;

    public Sprite[] divideTo4;
    public Sprite[] divideTo3;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public int GetItemTotalCount(ArraySchema<Item> items, ITEM mainType, List<ITEM> subTypes, int divideNum = 1)
    {
        int totalCount = 0;
        int subCount = 0;
        int mainCount = 0;
        items.ForEach((item) =>
        {
            ITEM type = (ITEM)item.id;
            if (type == mainType)
            {
                mainCount += item.count;
            }
            if (subTypes.FindIndex(tp => tp == type) != -1)
            {
                subCount += item.count;
            }
        });

        totalCount = mainCount + Mathf.FloorToInt((float)subCount / (float)divideNum);

        return totalCount;
    }
}
