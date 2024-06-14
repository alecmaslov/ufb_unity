using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetailPanel : MonoBehaviour
{
    public Text[] itemCountTexts;

    public Text itemGroupText;


    public void Init()
    {
        foreach (var item in itemCountTexts)
        {
            item.text = "0";
            itemGroupText.text = "0 / 4";
        }

        gameObject.SetActive(true);
    }
}
