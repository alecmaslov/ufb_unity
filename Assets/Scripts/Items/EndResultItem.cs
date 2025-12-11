using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndResultItem : MonoBehaviour
{
    public Image powerImage;
    public Text goldText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void InitData(Sprite powerSprite, string gold)
    {
        powerImage.sprite = powerSprite;
        goldText.text = gold;
    }
}
