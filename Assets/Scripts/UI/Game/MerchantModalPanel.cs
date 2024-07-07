using System.Collections;
using System.Collections.Generic;
using UFB.StateSchema;
using UnityEngine;
using UnityEngine.UI;

public class MerchantModalPanel : MonoBehaviour
{
    public Image itemImage;
    public Text itemText;
    public Image costImage;
    public Text costText;

    public Text coinText;

    public Item selectedItem;

    public GameObject craftPanel;
    public GameObject buySellPanel;

    public void InitData(Item item, string type, bool isBuy = true)
    {
        buySellPanel.SetActive(true);
        craftPanel.SetActive(false);
        selectedItem = item;
        if (type == "item")
        {
            itemImage.sprite = GlobalResources.instance.items[item.id];
        }
        else if(type == "power")
        {
            itemImage.sprite = GlobalResources.instance.powers[item.id];
        }
        else if(type == "stack")
        {
            itemImage.sprite = GlobalResources.instance.stacks[item.id];
        }

        itemText.text = item.name;

        if (isBuy)
        {
            coinText.text = item.cost.ToString();
        }
        else
        {
            coinText.text = item.sell.ToString();
        }

        gameObject.SetActive(true);
    }

    public void InitCraft()
    {
        buySellPanel.SetActive(false);
        craftPanel.SetActive(true);
    }

    public void OnApplyBtnClick()
    {
        // SEND COST STATUS DATA TO SERVER

        gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
