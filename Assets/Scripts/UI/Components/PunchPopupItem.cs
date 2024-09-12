using System.Collections;
using System.Collections.Generic;
using UFB.Core;
using UFB.Items;
using UFB.Map;
using UFB.Network.RoomMessageTypes;
using UnityEngine;
using UnityEngine.UI;

public class PunchPopupItem : MonoBehaviour
{
    public Image perkImage;
    public Image[] others;
    public Text posText;

    public float delayTime = 3f;
    private float transValue = 1;

    public void InitData(ToastPerkMessage m)
    {
        Tile CurrentTile = ServiceLocator.Current.Get<GameBoard>().Tiles[m.tileId];

        perkImage.sprite = GlobalResources.instance.perks[m.perkId];
        posText.text = CurrentTile.TilePosText;
        transValue = delayTime;

        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        transValue -= Time.deltaTime;
        if (transValue < 0)
        {
            transValue = 0;
            Destroy(gameObject);
            return;
        }
        perkImage.color = new Color(perkImage.color.r, perkImage.color.g, perkImage.color.b, transValue);
        foreach (var item in others)
        {
            item.color = new Color(item.color.r, item.color.g, item.color.b, transValue);
        }
        posText.color = new Color(posText.color.r, posText.color.g, posText.color.b, transValue);

    }
}
