using System;
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

    public Image banImage;
    
    [HideInInspector]
    public Item item;

    private void OnEnable()
    {
        
    }

    public void InitImage(Sprite sprite)
    {
        if(itemImage != null)
            itemImage.sprite = sprite;
    }

    public void InitText(string text) 
    {
        if(countText != null)
            countText.text = text;
    }

    public void InitTextBG(Color bgColor)
    {
        if(countText != null)
            countText.transform.parent.GetComponent<Image>().color = bgColor;
    }

    public void InitDate(string count, Sprite sprite, bool isRed = false, bool isBlack = false) 
    {
        if (countText != null) 
        {
            countText.text = count;
            
            countText.color = isRed? Color.red : Color.white;
            if (isBlack)
            {
                countText.color = Color.black;
            }
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

    public void InitBanImage()
    {
        if (banImage != null)
        {
            banImage.gameObject.SetActive(true);
        }
    }
}
