using Colyseus.Schema;
using System;
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
    public Sprite[] perks;
    public Sprite[] quests;

    public Sprite coin;
    public Sprite range;
    public Sprite lightImage;
    public Sprite energy;
    public Sprite ultimate;
    public Sprite health;

    public Sprite[] divideTo4;
    public Sprite[] divideTo3;

    [Serializable]
    public class CraftPowerSystem
    {
        public POWER power1;
        public POWER power2;
        public POWER powerResult;
        public int coin;
    }

    [Serializable]
    public class CraftItemSystem
    {
        public ITEM item1;
        public ITEM item2;
        public ITEM itemResult;
        public int coin;
    }

    public CraftItemSystem[] itemCraftSystem;
    public CraftPowerSystem[] powerCraftSystem;

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
