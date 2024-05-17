using System.Collections;
using System.Collections.Generic;
using UFB.StateSchema;
using UnityEngine;
using UnityEngine.UI;

public class EquipItem : MonoBehaviour
{
    [SerializeField]
    Image powerImage;

    [SerializeField]
    Text powerText;

    [SerializeField]
    Text levelText;

    [SerializeField]
    Text costText;

    public void Init(Sprite image, string name, string level, string cost)
    {
        powerImage.sprite = image;
        powerText.text = name;
        levelText.text = level;
        costText.text = cost;
    }
}
