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
        string header = "";
        if (type == 0)
        {
            header = "BOMBS";
        }
        else if (type == 1)
        {
            header = "ARROWS";
        }
        else if(type == 2) 
        {
            header = "ITEMS";
        }

        itemGroupText.text = header + "(0/4)";

        foreach (var item in itemCountTexts)
        {
            item.text = "0";
        }

        if(type == 0)
        {
            foreach (ITEM_DETAIL item in itemDetails)
            {
                item.count.text = UIGameManager.instance.GetItemCount(item.type).ToString();
            }
        }

        gameObject.SetActive(true);
    }
}
