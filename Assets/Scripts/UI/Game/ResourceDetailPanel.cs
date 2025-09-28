using System;
using System.Collections;
using System.Collections.Generic;
using UFB.Events;
using UFB.Items;
using UFB.Network.RoomMessageTypes;
using UFB.StateSchema;
using UFB.UI;
using UnityEngine;
using UnityEngine.UI;

public class ResourceDetailPanel : MonoBehaviour
{
    public enum DetailType
    {
        COIN,
        GIFT,
        KILL,
        STACK,
        ITEMTYPE,
        ITEMGROUP,

    }

    [Serializable]
    public struct DetailItem
    {
        public DetailType type;
        public int id;
    }

    public DetailItem[] detailTypes;

    [SerializeField]
    Text nameTxt;

    [SerializeField]
    Text itemCountText;

    [SerializeField]
    Image itemImage;

    [SerializeField]
    Transform itemDetailPanel;

    [SerializeField]
    public GameObject[] itemDetails;

    public Sprite[] detailImages;

    public string[] itemNames;

    public int[] itemIds;

    public string type;

    public int selectedPanelIdx = -1;

    public void OnItemDetailClicked(int idx)
    {
        selectedPanelIdx = idx;
        gameObject.SetActive(true);

        itemImage.sprite = detailImages[idx];

        nameTxt.text = itemNames[idx];

        itemCountText.text = "0";

        DetailItem detailItem = detailTypes[idx];
        if (detailItem.type == DetailType.ITEMTYPE)
        {
            UIGameManager.instance.controller.State.items.ForEach((Item item) =>
            {
                if (item.id == detailItem.id)
                {
                    itemCountText.text = item.count.ToString();
                    item.OnCountChange(((value, previousValue) =>
                    {
                        itemCountText.text = value.ToString();
                    }));
                }
            });
        }
        else if (detailItem.type == DetailType.ITEMGROUP)
        {
            if(detailItem.id == (int) ITEM.Bomb)
            {
                List<ITEM> bombs = new List<ITEM>
                {
                    ITEM.Bomb,
                    ITEM.IceBomb,
                    ITEM.VoidBomb,
                    ITEM.FireBomb,
                    ITEM.caltropBomb,
                };

                itemCountText.text = GlobalResources.instance.GetItemTotalCount(UIGameManager.instance.controller.State.items, ITEM.BombBag, bombs, 1).ToString();
            } 
            else if(detailItem.id == (int)(ITEM.Arrow))
            {
                List<ITEM> arrows = new List<ITEM>
                {
                    ITEM.IceArrow,
                    ITEM.BombArrow,
                    ITEM.FireArrow,
                    ITEM.VoidArrow,
                    ITEM.Arrow
                };

                itemCountText.text = GlobalResources.instance.GetItemTotalCount(UIGameManager.instance.controller.State.items, ITEM.Quiver, arrows, 1).ToString();
            }
        }
        else if (detailItem.type == DetailType.COIN)
        {
            itemCountText.text = UIGameManager.instance.controller.State.stats.coin.ToString();
        }
        else if(detailItem.type == DetailType.STACK)
        {
            UIGameManager.instance.controller.State.stacks.ForEach((Item item) =>
            {
                if(item.id == detailItem.id) 
                { 
                    itemCountText.text = item.count.ToString();
                }
            });
        }
        else if ( detailItem.type == DetailType.KILL)
        {

        }
        else if(detailItem.type == DetailType.GIFT)
        {
            itemCountText.text = UIGameManager.instance.controller.State.stats.bags.ToString();
        }

        foreach (var item in itemDetails)
        {
            item.gameObject.SetActive(false);
        }

        itemDetails[idx].gameObject.SetActive(true);
    }

    public void OnSetItemClicked(int type)
    {
        if (UIGameManager.instance.GetItemCount((ITEM)type) <= 0)
        {
            UIGameManager.instance.OnNotificationMessage("error", "you do not have enough resources to set the item.");
            return;
        }

        UIGameManager.instance.OnNotificationMessage("success", "You have set the item.");

        EventBus.Publish(
            RoomSendMessageEvent.Create(
                GlobalDefine.CLIENT_MESSAGE.SET_MOVE_ITEM,
                new RequestMoveItem
                {
                    characterId = UIGameManager.instance.controller.Id,
                    tileId = UIGameManager.instance.controller.CurrentTile.Id,
                    itemId = type,
                }
            )
        );
    }
}
