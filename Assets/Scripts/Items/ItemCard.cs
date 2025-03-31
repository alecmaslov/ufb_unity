using System.Collections;
using System.Collections.Generic;
using UFB.StateSchema;
using UnityEngine;
using UnityEngine.UI;

public class ItemCard : MonoBehaviour
{
    [SerializeField]
    private Image itemImage;

    public Text countText;

    public Text itemCountText;

    [HideInInspector]
    public Item item;

    public void InitImage(Sprite sprite)
    {
        itemImage.sprite = sprite;
    }

    public void InitText(string text) 
    {
        countText.text = text;
    }

    public void InitDate(string count, Sprite sprite, bool isRed = false) 
    {
        if (countText != null) 
        {
            countText.text = count;
            
            countText.color = isRed? Color.red : Color.white;
        }
        if (itemImage != null)
        {
            itemImage.sprite = sprite;
        }
    }

    public void InitData3(string count, Sprite sprite, string itemCount, Item _item) 
    {
        item = _item;
        countText.text = count;
        itemImage.sprite = sprite;
        itemCountText.text = itemCount;

        item.OnCountChange((short newCount, short preCount) =>
        {
            Debug.Log($"_---,,,, change count {newCount}", itemCountText);
            if (itemCountText != null) 
            { 
                itemCountText.text = newCount.ToString();
            }
        }, true);
    }
}
