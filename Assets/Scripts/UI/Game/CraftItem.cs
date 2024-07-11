using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftItem : MonoBehaviour
{
    public string type;

    public Image item1;
    public Image item2;
    public Image item3;
    public Text coinText;

    public CraftData data;

    public struct CraftData
    {
        public int idx1;
        public int idx2;
        public int idx3;
        public int coin;
        public string type;
    }

    public void InitData(int img1, int img2, int img3, int coin, string _type)
    {
        data = new CraftData();
        data.idx1 = img1;
        data.idx2 = img2;
        data.idx3 = img3;
        data.coin = coin;
        data.type = _type;

        type = _type;

        if(type == "item")
        {
            item1.sprite = GlobalResources.instance.items[img1];
            item2.sprite = GlobalResources.instance.items[img2];
            item3.sprite = GlobalResources.instance.items[img3];
        } 
        else
        {
            item1.sprite = GlobalResources.instance.powers[img1];
            item2.sprite = GlobalResources.instance.powers[img2];
            item3.sprite = GlobalResources.instance.powers[img3];
        }

        coinText.text = coin.ToString();
        gameObject.SetActive(true);
    }
  
}
