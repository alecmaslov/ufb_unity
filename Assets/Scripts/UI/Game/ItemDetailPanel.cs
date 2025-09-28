using System;
using System.Collections;
using System.Collections.Generic;
using UFB.Items;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetailPanel : MonoBehaviour
{
    public Text[] itemCountTexts;

    public Text itemGroupText;

    [Serializable]
    public struct ITEM_DETAIL
    {
        public ITEM type;
        public Text count;
    }

    public ITEM_DETAIL[] itemDetails;

    public void Init(int type = 0)
    {
        int total = 0;
        int limit = 4;

        string header = "";
        if (type == 0)
        {
            header = "BOMBS";
            limit = UIGameManager.instance.controller.State.stats.bombLimit;
        }
        else if (type == 1)
        {
            header = "ARROWS";
            limit = UIGameManager.instance.controller.State.stats.arrowLimit;
        }
        else if(type == 2) 
        {
            header = "ITEMS";
        }

        foreach (var item in itemCountTexts)
        {
            item.text = "0";
        }


        foreach (ITEM_DETAIL item in itemDetails)
        {
            UIGameManager.instance.controller.State.items.ForEach((item1) =>
            {
                if (item.type == (ITEM) item1.id)
                {
                    item.count.text = item1.count.ToString();
                    item1.OnCountChange(((value, previousValue) =>
                    {
                        item.count.text = value.ToString();
                        if (type is 0 or 1) 
                        { 
                            total += value;
                        }
                    }));
                    
                    if (type is 0 or 1) 
                    { 
                        total += item1.count;
                    }
                }
            });
            //int count = UIGameManager.instance.GetItemCount(item.type);
        }
        
        if(type == 2)
        {
            itemGroupText.text = header;
        }
        else
        {
            itemGroupText.text = header + $"({total})";
        }

        gameObject.SetActive(true);
    }

    public int GetTotalItemCount()
    {
        int total = 0;

        foreach (ITEM_DETAIL item in itemDetails)
        {
            total += UIGameManager.instance.GetItemCount(item.type);
        }
        
        return total;
    }
}
