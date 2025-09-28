using System.Collections;
using System.Collections.Generic;
using UFB.Core;
using UFB.Events;
using UFB.Items;
using UFB.Map;
using UFB.Network.RoomMessageTypes;
using UnityEngine;
using UnityEngine.UI;

public class WndPortalPanel : MonoBehaviour
{
    public RectTransform mapRect;
    public float col = 26;
    public float row = 26;
    public float radius = 100f;

    public GameObject SelectPanel;
    public GameObject ConfirmPanel;
    public Text tilePosText;
    public Text energyText;

    public RectTransform greenRectItem;
    public RectTransform blueRectItem;

    public Image holeImage;

    public Sprite[] sprites;

    private Tile selectedTile;

    private List<GameObject> portalObjects = new List<GameObject>();

    private bool isLoading = false;

    public void InitData()
    {
        SelectPanel.SetActive(true);
        ConfirmPanel.SetActive(false);

        for (int i = 0; i < portalObjects.Count; i++) 
        { 
            Destroy(portalObjects[i]);
        }
        portalObjects.Clear();

        UIGameManager.instance.portals.ForEach(p => 
        {
            RectTransform rect = new RectTransform();
            if(p._portalParameters.portalIndex == 0)
            {
                rect = Instantiate(blueRectItem, mapRect);
            }
            else if(p._portalParameters.portalIndex == 1)
            {
                rect = Instantiate(greenRectItem, mapRect);
            }

            Tile tile = ServiceLocator.Current.Get<GameBoard>().Tiles[p.SpawnEntity.tileId];

            rect.GetComponent<Button>().onClick.AddListener(() =>
            {
                selectedTile = tile;
                tilePosText.text = tile.TilePosText;
                energyText.text = "-1";
                holeImage.sprite = sprites[p._portalParameters.portalIndex];
            });

            float w = 1000;
            float h = 1000;

            float rangeW = w / col;
            float rangeH = h / row;

            float x = tile.Coordinates.X * rangeW + rangeW / 2;
            float y = tile.Coordinates.Y * rangeH + rangeH / 2;

            rect.anchoredPosition = new Vector2(x, -y);
            rect.gameObject.SetActive(true);

        });


        gameObject.SetActive(true);

    }

    public void OnConfirm()
    {
        // send portal message...
        if (selectedTile == null) return;
        EventBus.Publish(
            RoomSendMessageEvent.Create(
                GlobalDefine.CLIENT_MESSAGE.SET_MOVE_ITEM,
                new RequestMoveItem
                {
                    characterId = UIGameManager.instance.controller.Id,
                    tileId = selectedTile.Id,
                    itemId = (int)ITEM.WarpCrystal,
                }
            )
        );
        gameObject.SetActive(false);
        isLoading = true;
    }

    public void OnMovePortal()
    {
        if(selectedTile == null) return;

    }

    public void OnCloseModal()
    {
        gameObject.SetActive(false);
        selectedTile = null;
    }
}
