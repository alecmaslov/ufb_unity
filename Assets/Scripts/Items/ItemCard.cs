using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCard : MonoBehaviour
{
    [SerializeField]
    private Image itemImage;

    [SerializeField]
    private Text countText;


    public void InitDate(string count, Sprite sprite) 
    {
        countText.text = count;
        itemImage.sprite = sprite;
    }
}
