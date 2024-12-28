using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UFB;
using UnityEngine.UI;
using UFB.Map;
using UFB.Core;
using UFB.Network.RoomMessageTypes;
using UFB.StateSchema;
using UFB.Entities;
using UFB.Events;
using UFB.Items;
using Colyseus.Schema;
using UFB.Character;
using UnityEditor;

public class MovePanel : MonoBehaviour
{
    [SerializeField]
    private StepPanel stepPanel;

    [SerializeField] 
    private GameObject resourcePanel;

    [SerializeField]
    private Image leftBtn;

    [SerializeField]
    private Image rightBtn;

    [SerializeField]
    private Image topBtn;

    [SerializeField]
    private Image bottomBtn;

    [SerializeField]
    public UFB.Character.CharacterController character;

    [SerializeField]
    private Sprite[] sprites;

    [SerializeField]
    private Tile currentTile;

    [SerializeField]
    public UIDirection globalDirection;

    [SerializeField]
    Text[] btnList;

    [SerializeField]
    MoveItemDetailPanel moveItemDetailPanel;

    public int selectedItemId = -1;

    [SerializeField]
    GameObject explosionBtn;

    [SerializeField]
    ItemCard damageItem;

    [SerializeField]
    Transform damageList;

    public Image damageMainItemImage;

    public Text posText;
    public Text energyText;
    public Text bombEngeryText;
    public Image posImage;

    public RectTransform movePart;
    public RectTransform bombPart;
    public Image moveImage;
    public Text tileTypeText;

    private bool isLeft = true;
    private bool isRight = true;
    private bool isTop = true;
    private bool isDown = true;

    private float originEnergy;
    private List<Item> originItems = new List<Item>();

    Vector2Int _destination;

    // Start is called before the first frame update
    private void Start()
    {

    }

    public void OnCharacterMoved(CharacterMovedMessage m)
    {
        InitMoveBtns();

        Sprite leftSprite = sprites[0];
        Sprite rightSprite = sprites[0];
        Sprite topprite = sprites[0];
        Sprite bottomSprite = sprites[0];

        if (m.left == 0)
        {
            leftBtn.sprite = sprites[5];
            isLeft = false;
            leftSprite = sprites[5];
        }
        if(m.right == 0)
        {
            rightBtn.sprite = sprites[5];
            isRight = false;
            rightSprite = sprites[5];
        }
        if (m.top == 0) 
        {
            topBtn.sprite = sprites[5];
            isTop = false;
            topprite = sprites[5];
        }
        if (m.down == 0)
        {
            bottomBtn.sprite = sprites[5];
            isDown = false;
            bottomSprite = sprites[5];
        }

        globalDirection.InitPosDirection(leftSprite, rightSprite, topprite, bottomSprite, isLeft, isRight, isTop, isDown);


        List<ITEM> bombs = new List<ITEM>
        {
            ITEM.IceBomb,
            ITEM.VoidBomb,
            ITEM.FireBomb,
            ITEM.caltropBomb,
        };

        btnList[0].text = GlobalResources.instance.GetItemTotalCount(character.State.items, ITEM.Bomb, bombs, 1).ToString();

        btnList[1].text = GlobalResources.instance.GetItemTotalCount(character.State.items, ITEM.Feather, new List<ITEM> { }, 1).ToString();

        btnList[2].text = GlobalResources.instance.GetItemTotalCount(character.State.items, ITEM.WarpCrystal, new List<ITEM> { }, 1).ToString();


        Debug.Log($"Tile Pos: {character.CurrentTile.TilePosText}, Tile State: {character.CurrentTile.GetTileState().type}");
    }

    public void Show()
    {
        gameObject.SetActive(true);
        explosionBtn.SetActive(false);
        posText.text = "";
        energyText.text = "";
        bombEngeryText.text = "";
        tileTypeText.text = "";
        posImage.enabled = true;
        InitMoveBtns();
        currentTile = character.CurrentTile;
        originEnergy = character.State.stats.energy.current;
        originItems.Clear();
        character.State.items.ForEach((Item item) =>
        {
            Item item1 = new Item();
            item1.id = item.id;
            item1.name = item.name;
            item1.count = item.count;
            item1.level = item.level;
            item1.description = item.description;
            originItems.Add(item1);
        });

        character.MoveToTile(character.CurrentTile);

        // globalDirection.gameObject.SetActive(true);
        globalDirection.transform.position = character.transform.position;
    }

    public void InitMoveBtns()
    {
        topBtn.sprite = sprites[0];
        bottomBtn.sprite = sprites[1];
        leftBtn.sprite = sprites[2];
        rightBtn.sprite = sprites[3];
        isLeft = true;
        isRight = true;
        isTop = true;
        isDown = true;
        //explosionBtn.SetActive(false);

    }

    public void OnConfirm()
    {
        stepPanel.gameObject.SetActive(true);
        resourcePanel.SetActive(true);
        gameObject.SetActive(false);
        globalDirection.gameObject.SetActive(false) ;

        Tile tile = character.CurrentTile;
        if (selectedTile != null)
        {
            character.MoveToTile(selectedTile, true);

            Tile tile1 = selectedTile;
            for (int i = 0; i < tile1.transform.childCount; i++)
            {
                GameObject item = tile1.transform.GetChild(i).gameObject;
                if (item.GetComponent<Chest>() != null)
                {
                    Debug.Log("Item position Destination...");
                    EventBus.Publish(
                        RoomSendMessageEvent.Create(
                            "spawnMove",
                            new RequestSpawnMessage
                            {
                                tileId = tile1.Id,
                                destination = tile1.Coordinates,
                                playerId = character.Id,
                                isItemBag = item.GetComponent<Chest>().isItemBag,
                            }
                        )
                    );

                    gameObject.SetActive(false);
                }
                else if (item.GetComponent<Merchant>() != null)
                {
                    Debug.Log("Merchant position Destination...");
                    EventBus.Publish(
                        RoomSendMessageEvent.Create(
                            "getMerchantData",
                            new RequestTile
                            {
                                characterId = character.Id,
                                tileId = tile1.Id,
                            }
                        )
                    );
                }
            }


        }
        else
        {
            character.MoveToTile(tile);
        }
        selectedTile = null;
        bombPrevTile = null;
        HighlightRect.Instance.ClearHighLightRect();
    }

    public void OnCancel()
    {
        stepPanel.gameObject.SetActive(true);
        resourcePanel.SetActive(true);
        gameObject.SetActive(false);
        globalDirection.gameObject.SetActive(false);

        character.CancelMoveToTile(
            originItems,
            currentTile,
            originEnergy
        );
        selectedTile = null;
        bombPrevTile = null;
        HighlightRect.Instance.ClearHighLightRect();
    }

    public void OnMoveClick(string move)
    {
        Tile tile = character.CurrentTile;
        TileState stateCurrent = tile.GetTileState();

        NextCoordinates(tile.Coordinates, move);

        if (move == "top" && (EdgeProperty)stateCurrent.walls[0] == EdgeProperty.BRIDGE)
        {
            NextCoordinates(new Coordinates(_destination.x, _destination.y), move);
        }
        else if (move == "right" && (EdgeProperty)stateCurrent.walls[1] == EdgeProperty.BRIDGE)
        {
            NextCoordinates(new Coordinates(_destination.x, _destination.y), move);
        }
        else if (move == "down" && (EdgeProperty)stateCurrent.walls[2] == EdgeProperty.BRIDGE)
        {
            NextCoordinates(new Coordinates(_destination.x, _destination.y), move);
        }
        else if (move == "left" && (EdgeProperty)stateCurrent.walls[3] == EdgeProperty.BRIDGE)
        {
            NextCoordinates(new Coordinates(_destination.x, _destination.y), move);
        }

        var gameBoard = ServiceLocator.Current.Get<GameBoard>();
        Tile tile1 = gameBoard.GetTileByCoordinates(Coordinates.FromVector2Int(_destination));

        character.MoveToTile(tile1);
        for (int i = 0; i < tile1.transform.childCount; i++)
        {
            GameObject item = tile1.transform.GetChild(i).gameObject;
            if (item.GetComponent<Chest>() != null)
            {
                Debug.Log("Item position Destination...");
                EventBus.Publish(
                    RoomSendMessageEvent.Create(
                        "spawnMove",
                        new RequestSpawnMessage
                        {
                            tileId = tile1.Id,
                            destination = tile1.Coordinates,
                            playerId = character.Id,
                            isItemBag = item.GetComponent<Chest>().isItemBag,
                        }
                    )
                );

                gameObject.SetActive(false);
            }
            else if (item.GetComponent<Merchant>() != null) 
            { 
                Debug.Log("Merchant position Destination...");
                EventBus.Publish(
                    RoomSendMessageEvent.Create(
                        "getMerchantData",
                        new RequestTile
                        {
                            characterId = character.Id,
                            tileId = tile1.Id,
                        }
                    )
                );
            }
        }

    }

    public void OnDirectionClicked(string move)
    {
        if(selectedItemId == -1)
        {
            OnMoveClick(move);
        } 
        else
        {
            NextCoordinates(character.CurrentTile.Coordinates, move, false);

            var gameBoard = ServiceLocator.Current.Get<GameBoard>();
            Tile tile1 = gameBoard.GetTileByCoordinates(Coordinates.FromVector2Int(_destination));

            if((ITEM) selectedItemId == ITEM.Feather)
            {
                Debug.Log("Feather sent : " + tile1.TilePosText);
                OnSetMoveItem(tile1, selectedItemId);
            } 
            /*else if((ITEM)selectedItemId == ITEM.Bomb)
            {
                moveItemDetailPanel.tile = tile1;
                moveItemDetailPanel.Init();
            }*/


        }
    }

    private void NextCoordinates(Coordinates coordinates, string move, bool isFeather = true)
    {
        int x = coordinates.X;
        int y = coordinates.Y;

        if (move == "left")
        {
            if (!isLeft && isFeather) return;
            x--;
            x = x < 0 ? 0 : x;
        }
        else if (move == "right")
        {
            if (!isRight && isFeather) return;
            x++;
            x = x > 25 ? 25 : x;
        }
        else if (move == "top")
        {
            if (!isTop && isFeather) return;
            y--;
            y = y < 0 ? 0 : y;
        }
        else if (move == "down")
        {
            if (!isDown && isFeather) return;
            y++;
            y = y > 25 ? 25 : y;
        }

        _destination.x = x;
        _destination.y = y;
        Debug.Log($"Current Tile pos: {x}, {y}");
    }

    public void OnBombClick(int type)
    {
        EventBus.Publish(
            RoomSendMessageEvent.Create(
                "getMoveItem",
                new RequestMoveItem
                {
                    characterId = character.Id,
                    tileId = character.CurrentTile.Id,
                    itemId = type,
                }
            )
        );
    }

    public void OnReceiveMoveItem(MoveItemMessage message)
    {
        selectedItemId = message.itemId;
        Sprite sprite = GlobalResources.instance.items[message.itemId];
        globalDirection.InitItemDirection(
            message.left == 1? sprite : null,
            message.right == 1? sprite : null,
            message.top == 1? sprite : null,
            message.down == 1? sprite : null
        );

        if ((ITEM)selectedItemId == ITEM.Bomb)
        {
            moveItemDetailPanel.tile = selectedTile;
            moveItemDetailPanel.Init();
        }
    }

    public void OnSetMoveItem(Tile tile, int itemId)
    {
        EventBus.Publish(
            RoomSendMessageEvent.Create(
                GlobalDefine.CLIENT_MESSAGE.SET_MOVE_ITEM,
                new RequestMoveItem
                {
                    characterId = character.Id,
                    tileId = tile.Id,
                    itemId = itemId,
                }
            )
        );
    }

    public void OnBombMoveTile()
    {
        if (bombPrevTile != null)
        {
            Debug.Log("Bomb is moved.");
            character.MoveToTile( bombPrevTile, true );
            HighlightRect.Instance.ClearHighLightRect();
        }
    }

    public void OnReceiveGetBombMessage(GetBombMessage message)
    {
        ItemResult result = message.itemResult;

        for (int i = 1; i < damageList.childCount; i++) 
        { 
            Destroy(damageList.GetChild(i));
        }

        if (selectedItemId > -1)
        {
            damageMainItemImage.sprite = GlobalResources.instance.items[selectedItemId];
        }

        if (result.energy != 0)
        {
            ItemCard itemCard = Instantiate(damageItem, damageList);
            itemCard.InitDate(result.energy.ToString(), GlobalResources.instance.energy);
            itemCard.gameObject.SetActive(true);
        }

        if (result.heart != 0)
        {
            ItemCard itemCard = Instantiate(damageItem, damageList);
            itemCard.InitDate(result.heart.ToString(), GlobalResources.instance.health);
            itemCard.gameObject.SetActive(true);
        }

        if (result.ultimate != 0)
        {
            ItemCard itemCard = Instantiate(damageItem, damageList);
            itemCard.InitDate(result.ultimate.ToString(), GlobalResources.instance.ultimate);
            itemCard.gameObject.SetActive(true);
        }

        if (result.stackId > -1)
        {
            ItemCard itemCard = Instantiate(damageItem, damageList);
            itemCard.InitDate("+1", GlobalResources.instance.stacks[result.stackId]);
            itemCard.gameObject.SetActive(true);
        }

        if(result.perkId > 0)
        {

        }

        if(result.powerId > 0)
        {
            ItemCard itemCard = Instantiate(damageItem, damageList);
            itemCard.InitDate("+1", GlobalResources.instance.powers[result.powerId]);
            itemCard.gameObject.SetActive(true);
        }

        explosionBtn.gameObject.SetActive( true );
        globalDirection.gameObject.SetActive(false);
    }

    public void OnSetMoveItemReceived(SetMoveItemMessage message)
    {
        selectedItemId = -1;

        if ((ITEM)message.itemId == ITEM.Feather) 
        {
            Debug.Log("Feather Message Received");
            var gameBoard = ServiceLocator.Current.Get<GameBoard>();
            Tile tile = gameBoard.Tiles[message.tileId];
            Debug.Log("Feather Position : " + tile.TilePosText);
            character.MoveToTile(tile);
        }
        else if((ITEM)message.itemId == ITEM.WarpCrystal)
        {
            var gameBoard = ServiceLocator.Current.Get<GameBoard>();
            Tile tile = gameBoard.Tiles[message.tileId];
            Debug.Log("Warp Position : " + tile.TilePosText);
            //tile.GetTileState().walls[0] == EdgeProperty.BASIC
            character.MoveToTile(tile);
        }
        else
        {
            //character.MoveToTile(character.CurrentTile);
        }

        moveItemDetailPanel.gameObject.SetActive( false );
    }

    private Tile selectedTile = null;

    public void OnClickTile(Tile tile)
    {
        if(!gameObject.activeSelf)
        {
            Show();
        }
        
        if (gameObject.activeSelf)
        {

            if (
                tile.GetTileState().type == "Void" ||
                tile.GetTileState().type == "VerticalBridge" ||
                tile.GetTileState().type == "HorizontalBridge" ||
                tile.GetTileState().type == "DoubleBridge" ||
                tile.GetTileState().type == "StairsNS" ||
                tile.GetTileState().type == "StairsSN" ||
                tile.GetTileState().type == "StairsEW" ||
                tile.GetTileState().type == "StairsWE"
            ) 
            {
                posImage.gameObject.SetActive(false);
                moveImage.gameObject.SetActive(false);
                bombPart.gameObject.SetActive(false);
                movePart.gameObject.SetActive(false);

                if (tile.GetTileState().type == "Void")
                {
                    tileTypeText.text = "VOID";
                }
                else if(tile.GetTileState().type == "VerticalBridge" || tile.GetTileState().type == "HorizontalBridge" || tile.GetTileState().type == "DoubleBridge")
                {
                    tileTypeText.text = "BRIDGE";
                }
                else if(tile.GetTileState().type == "StairsNS" || tile.GetTileState().type == "StairsSN" || tile.GetTileState().type == "StairsEW" || tile.GetTileState().type == "StairsWE")
                {
                    tileTypeText.text = "STAIRS";
                }

                return;
            }


            selectedTile = tile;
            posText.text = tile.TilePosText;
            posImage.gameObject.SetActive(true);
            posImage.enabled = true;
            Debug.Log("send message: xxxxo onclick");
            EventBus.Publish(
                RoomSendMessageEvent.Create(
                    GlobalDefine.CLIENT_MESSAGE.SET_MOVE_POINT,
                    new RequestMoveItem
                    {
                        characterId = character.Id,
                        tileId = tile.Id,
                        itemId = -1,
                    }
                )
            );
            moveImage.gameObject.SetActive( false );
            bombPart.gameObject.SetActive( true );
            movePart.gameObject.SetActive( true );
            tileTypeText.text = "";
            //movePart.rect.Set(-150, movePart.rect.y, movePart.rect.width, movePart.rect.height);
            movePart.localPosition = new Vector3(-150, movePart.localPosition.y, movePart.localPosition.z);
            RectTransform posRect = posImage.gameObject.GetComponent<RectTransform>();
            posRect.localPosition = new Vector3(0, posRect.localPosition.y, posRect.localPosition.z);

            UIGameManager.instance.bottomAttackPanel.gameObject.SetActive( false );

            for (int i = 0; i < tile.transform.childCount; i++)
            {
                GameObject item = tile.transform.GetChild(i).gameObject;

                if (item.GetComponent<Chest>() != null)
                {
                    posImage.enabled = false;

                    if (!item.GetComponent<Chest>().isItemBag)
                    {
                        posText.text = "ITEM BOX";
                    } 
                    else
                    {
                        posText.text = "ITEM BOX";
                    }
                    movePart.localPosition = new Vector3(200, movePart.localPosition.y, movePart.localPosition.z);
                    posRect.localPosition = new Vector3(180, posRect.localPosition.y, posRect.localPosition.z);

                    moveImage.sprite = GlobalResources.instance.itemBag;
                    moveImage.gameObject.SetActive(true);
                    bombPart.gameObject.SetActive(false);
                }
                else if(item.GetComponent<Portal>() != null )
                {
                    posImage.enabled = false;

                    posText.text = "PORTAL";
                    movePart.localPosition = new Vector3(200, movePart.localPosition.y, movePart.localPosition.z);
                    posRect.localPosition = new Vector3(180, posRect.localPosition.y, posRect.localPosition.z);

                    moveImage.sprite = GlobalResources.instance.portal;
                    moveImage.gameObject.SetActive(true);
                    bombPart.gameObject.SetActive(false);
                }
                else if(item.GetComponent<Merchant>() != null )
                {
                    posImage.enabled = false;

                    posText.text = "MERCHANT";
                    movePart.localPosition = new Vector3(200, movePart.localPosition.y, movePart.localPosition.z);
                    posRect.localPosition = new Vector3(180, posRect.localPosition.y, posRect.localPosition.z);

                    moveImage.sprite = GlobalResources.instance.merchant;
                    moveImage.gameObject.SetActive(true);
                    bombPart.gameObject.SetActive(false);
                }
                else if(item.GetComponent<UFB.Character.CharacterController>() != null)
                {
                    UFB.Character.CharacterController controller = item.GetComponent<UFB.Character.CharacterController>();
                    //UIGameManager.instance.ResourcePanel.OnCharacterValueEvent(controller.State);
                    //UIGameManager.instance.ResourcePanel.gameObject.SetActive(true);
                    UIGameManager.instance.movePanel.gameObject.SetActive(false);
                    UIGameManager.instance.bottomAttackPanel.gameObject.SetActive(false);
                    UIGameManager.instance.tapSelfPanel.gameObject.SetActive(false);

                    if(controller.State.type == (int) USER_TYPE.MONSTER)
                    {
                        UIGameManager.instance.bottomAttackPanel.Init(controller.State);
                    }
                    else if(controller.State.type == (int)USER_TYPE.USER)
                    {
                        UIGameManager.instance.tapSelfPanel.InitPanel();
                        /*UIGameManager.instance.ResourcePanel.OnCharacterValueEvent(controller.State);
                        UIGameManager.instance.ResourcePanel.gameObject.SetActive(true);*/
                    }
                }
            }

        }
    }

    private Tile bombPrevTile = null;

    public void OnSetMovePointMessage( SetMovePointMessage m)
    {
        bombPrevTile = null;

        var gameBoard = ServiceLocator.Current.Get<GameBoard>();

        List<Tile> tiles = new List<Tile>();
        foreach (var p in m.path)
        {
            Tile tile = gameBoard.Tiles[p.tileId];
            tiles.Add( tile );
        }
        energyText.text = $"-{tiles.Count}";
        bombEngeryText.text = $"-{tiles.Count - 1}";
        HighlightRect.Instance.ClearHighLightRect();
        HighlightRect.Instance.SetHighLightForSpawn(tiles);

        if (tiles.Count > 1) 
        {
            bombPrevTile = tiles[tiles.Count - 2];
        }
    }

    private void Update()
    {
        if (character == null || !gameObject.activeSelf) return;

        globalDirection.transform.position = character.transform.position;
    }

}
