using System.Collections;
using System.Collections.Generic;
using UFB.Events;
using UFB.Items;
using UFB.Network.RoomMessageTypes;
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

    public bool isBuyStatus = false;
    public string itemType;

    #region craft panel
    public Image item1;
    public Image item2;
    public Image item3;

    public Text itemDesText;
    [HideInInspector]
    public CraftItem selectedCraftItem;
    #endregion



    public void InitData(Item item, string type, bool isBuy = true)
    {
        itemType = type;
        isBuyStatus = isBuy;
        selectedItem = item;

        buySellPanel.SetActive(true);
        craftPanel.SetActive(false);
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

        costImage.gameObject.SetActive(false);
        costText.gameObject.SetActive(false);

        gameObject.SetActive(true);
    }

    public void InitCraft(CraftItem craftItem)
    {
        selectedCraftItem = craftItem;
        buySellPanel.SetActive(false);
        craftPanel.SetActive(true);
        selectedItem = null;
        gameObject.SetActive(true);
        if (craftItem.data.type == "item") 
        {
            item1.sprite = GlobalResources.instance.items[craftItem.data.idx1];
            item2.sprite = GlobalResources.instance.items[craftItem.data.idx2];
            item3.sprite = GlobalResources.instance.items[craftItem.data.idx3];
            coinText.text = craftItem.data.coin.ToString();

            itemDesText.text = UIGameManager.instance.merchantPanel.itemData[craftItem.data.idx3].name;
        }
        else
        {
            item1.sprite = GlobalResources.instance.powers[craftItem.data.idx1];
            item2.sprite = GlobalResources.instance.powers[craftItem.data.idx2];
            item3.sprite = GlobalResources.instance.powers[craftItem.data.idx3];
            coinText.text = craftItem.data.coin.ToString();

            foreach (var power in UIGameManager.instance.merchantPanel.powerData)
            {
                if(power.id == craftItem.data.idx3)
                {
                    itemDesText.text = $"LEVEL {power.level} {power.name}";
                }
            }
        }
    }

    public void OnApplyBtnClick()
    {
        // SEND COST STATUS DATA TO SERVER
        if (selectedItem != null)
        {
            if (isBuyStatus)
            {
                EventBus.Publish(
                    RoomSendMessageEvent.Create(
                        "buyItem",
                        new RequestBuyItem
                        {
                            id = selectedItem.id,
                            type = itemType
                        }
                    )
                );
            }
            else
            {
                EventBus.Publish(
                    RoomSendMessageEvent.Create(
                        "sellItem",
                        new RequestSellItem
                        {
                            id = selectedItem.id,
                            type = itemType
                        }
                    )
                );
            }
        }
        else if (selectedCraftItem != null)
        {
            // SEND CRAFT DTAT TO SERVER
            EventBus.Publish(
                RoomSendMessageEvent.Create(
                    "addCraftItem",
                    new RequestAddCraftItem
                    {
                        idx1 = selectedCraftItem.data.idx1,
                        idx2 = selectedCraftItem.data.idx2,
                        type = selectedCraftItem.data.type,
                        idx3 = selectedCraftItem.data.idx3,
                        coin = selectedCraftItem.data.coin,

                    }
                )
            );

        }
        gameObject.SetActive(false);

    }
}
