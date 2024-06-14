using System.Collections;
using System.Collections.Generic;
using UFB.Items;
using UnityEngine;
using UnityEngine.UI;

public class ResourceDetailPanel : MonoBehaviour
{
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

    public void OnItemDetailClicked(int idx)
    {
        gameObject.SetActive(true);

        itemImage.sprite = detailImages[idx];

        nameTxt.text = itemNames[idx];

        itemCountText.text = "0";

        foreach (var item in itemDetails)
        {
            item.gameObject.SetActive(false);
        }

        itemDetails[idx].gameObject.SetActive(true);
    }
}
