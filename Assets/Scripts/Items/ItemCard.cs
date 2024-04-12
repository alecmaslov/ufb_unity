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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitDate(string count, Sprite sprite) 
    {
        countText.text = count;
        itemImage.sprite = sprite;
    }
}
