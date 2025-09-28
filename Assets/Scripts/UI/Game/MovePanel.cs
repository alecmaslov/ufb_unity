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

    public GameObject bombDamagePanel;
    
    public Image damageMainItemImage;

    public Text posText;
    public Text energyText;
    public Text bombEngeryText;
    public Image posImage;
    public Text featherText;
    public Text featherText1;
    
    public RectTransform energyPart;
    public RectTransform movePart;
    public RectTransform bombPart;
    public Image moveImage;
    public Text tileTypeText;

    public GameObject defaultPart;
    public GameObject lessPart;
    public Text lessPartPosText;
    public Text lessPartenergyText;
    public Text lessPartFeatherText;

    public GameObject wrapBtn;
    public Text wrapText;
    
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
        // InitMoveBtns();

        /*Sprite leftSprite = sprites[0];
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


        Debug.Log($"Tile Pos: {character.CurrentTile.TilePosText}, Tile State: {character.CurrentTile.GetTileState().type}");*/
    }

    public void Show()
    {
        gameObject.SetActive(true);
        explosionBtn.SetActive(false);
        UIGameManager.instance.tapSelfPanel.gameObject.SetActive(false);
        posText.text = "";
        energyText.text = "";
        bombEngeryText.text = "";
        tileTypeText.text = "";
        posImage.enabled = true;
        
        wrapBtn.SetActive(false);
        wrapText.text = "1";
        
        // InitMoveBtns();
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

        // character.MoveToTile(character.CurrentTile);
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
        int energyNeededCount = Mathf.Abs(int.Parse(energyText.text));
        
        if(character.State.stats.energy.current < energyNeededCount)
        {
            UIGameManager.instance.OnNotificationMessage("error", "Energy is not enough.");
            return;
        }

        stepPanel.gameObject.SetActive(true);
        resourcePanel.SetActive(true);
        gameObject.SetActive(false);

        Tile tile = character.CurrentTile;
        if (selectedTile != null)
        {
            character.MoveToTile(selectedTile, true, isFeather);

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
        UIGameManager.instance.bottomDrawer.CloseBottomDrawer();
    }

    public void OnCancel()
    {
        stepPanel.gameObject.SetActive(true);
        resourcePanel.SetActive(true);
        gameObject.SetActive(false);

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

    public void InitBomBList(ItemResult result, bool isBanStack = false, bool isPerk = false)
    {
        for (int i = 1; i < damageList.childCount; i++) 
        { 
            Destroy(damageList.GetChild(i).gameObject);
        }
        
        if (result.energy != 0)
        {
            ItemCard itemCard = Instantiate(damageItem, damageList);
            itemCard.InitData(result.energy.ToString(), GlobalResources.instance.energy);
            itemCard.gameObject.SetActive(true);
        }

        if (result.heart != 0)
        {
            ItemCard itemCard = Instantiate(damageItem, damageList);
            itemCard.InitData(result.heart.ToString(), GlobalResources.instance.health);
            itemCard.gameObject.SetActive(true);
        }

        if (result.ultimate != 0)
        {
            ItemCard itemCard = Instantiate(damageItem, damageList);
            itemCard.InitData(result.ultimate.ToString(), GlobalResources.instance.ultimate);
            itemCard.gameObject.SetActive(true);
        }

        if (result.stackId > -1)
        {
            ItemCard itemCard = Instantiate(damageItem, damageList);
            itemCard.InitData("+1", GlobalResources.instance.stacks[result.stackId]);
            if (isBanStack)
            {
                itemCard.InitBanImage();
            }
            itemCard.gameObject.SetActive(true);
        }

        if(result.perkId >= 0)
        {
            ItemCard itemCard = Instantiate(damageItem, damageList);
            itemCard.InitData("", GlobalResources.instance.perks[result.perkId]);
            if (isPerk)
            {
                itemCard.InitBanImage();
            }
            itemCard.gameObject.SetActive(true);
        }
        
        if(result.powerId >= 0)
        {
            ItemCard itemCard = Instantiate(damageItem, damageList);
            itemCard.InitData("+1", GlobalResources.instance.powers[result.powerId]);
            itemCard.gameObject.SetActive(true);
        }
    }
    
    public void OnReceiveGetBombMessage(GetBombMessage message)
    {
        ItemResult result = message.itemResult;
        
        if (message.itemId > -1)
        {
            damageMainItemImage.sprite = GlobalResources.instance.items[message.itemId];
        }

        InitBomBList(result);
        
        //explosionBtn.gameObject.SetActive( true );
        UIGameManager.instance.bottomDrawer.OpenBottomDrawer();

        if (UIGameManager.instance.isPlayerTurn)
        {
            bombDamagePanel.gameObject.SetActive(true);
            bombDamagePanel.transform.GetChild(0).gameObject.SetActive(true);
            UIGameManager.instance.attackResultPanel.InitBombResult(result);
        }
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
            UIGameManager.instance.bottomDrawer.OpenBottomDrawer();
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
                energyPart.gameObject.SetActive(false);

                if (tile.GetTileState().type == "Void")
                {
                    tileTypeText.text = "VOID";
                    UIGameManager.instance.selectNamePanel.UpdateTarget(tile.transform, "VOID");
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
            posText.gameObject.SetActive(true);
            posText.text = tile.TilePosText;
            lessPartPosText.text = tile.TilePosText;
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
                        isFeather = CharacterManager.Instance.PlayerCharacter.State.items.ContainsKey(ITEM.Feather) && CharacterManager.Instance.PlayerCharacter.State.items[(int) ITEM.Feather].count > 0
                    }
                )
            );
            
            wrapBtn.SetActive( false );
            moveImage.gameObject.SetActive( false );
            movePart.gameObject.SetActive( true );
            energyPart.gameObject.SetActive( true );

            int bombTotalCount = UIGameManager.instance.ResourcePanel.bombDetailPanel.GetTotalItemCount();
            bombPart.gameObject.SetActive(bombTotalCount > 0);
            movePart.localPosition = new Vector3(bombTotalCount > 0? -200 : 0, movePart.localPosition.y, movePart.localPosition.z);

            tileTypeText.text = "";
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
                        moveImage.sprite = GlobalResources.instance.treasureImage;
                        posText.gameObject.SetActive(false);
                        posText.text = "LOOT BOX";
                        UIGameManager.instance.selectNamePanel.UpdateTarget(tile.transform, "LOOT BOX");
                    } 
                    else
                    {
                        moveImage.sprite = GlobalResources.instance.itemBag;
                        posText.gameObject.SetActive(false);
                        posText.text = "LOOT BAG";
                        UIGameManager.instance.selectNamePanel.UpdateTarget(tile.transform, "LOOT BAG");
                    }
                    movePart.localPosition = new Vector3(200, movePart.localPosition.y, movePart.localPosition.z);
                    posRect.localPosition = new Vector3(111, posRect.localPosition.y, posRect.localPosition.z);

                    moveImage.gameObject.SetActive(true);
                    bombPart.gameObject.SetActive(false);
                }
                else if(item.GetComponent<Portal>() != null )
                {
                    posImage.enabled = false;
                    posText.gameObject.SetActive(false);
                    posText.text = "PORTAL";
                    UIGameManager.instance.selectNamePanel.UpdateTarget(tile.transform, "PORTAL");
                    movePart.localPosition = new Vector3(200, movePart.localPosition.y, movePart.localPosition.z);
                    posRect.localPosition = new Vector3(111, posRect.localPosition.y, posRect.localPosition.z);

                    moveImage.sprite = item.GetComponent<Portal>()._portalParameters.portalIndex == 0? GlobalResources.instance.blue_portal : GlobalResources.instance.green_portal;
                    moveImage.gameObject.SetActive(true);
                    bombPart.gameObject.SetActive(false);

                    wrapText.text = character.State.items.ContainsKey(ITEM.WarpCrystal) &&
                                    character.State.items[(int)ITEM.WarpCrystal].count > 0
                        ? $"{character.State.items[(int)ITEM.WarpCrystal].count}"
                        : "0";
                    wrapText.color = character.State.items.ContainsKey(ITEM.WarpCrystal) &&
                                     character.State.items[(int)ITEM.WarpCrystal].count > 0? Color.white : Color.red;
                    wrapBtn.SetActive(true);
                }
                else if(item.GetComponent<Merchant>() != null )
                {
                    posImage.enabled = false;
                    posText.gameObject.SetActive(false);
                    posText.text = "MERCHANT";
                    UIGameManager.instance.selectNamePanel.UpdateTarget(tile.transform, "MERCHANT");
                    movePart.localPosition = new Vector3(200, movePart.localPosition.y, movePart.localPosition.z);
                    posRect.localPosition = new Vector3(111, posRect.localPosition.y, posRect.localPosition.z);

                    moveImage.sprite = GlobalResources.instance.merchant;
                    moveImage.gameObject.SetActive(true);
                    bombPart.gameObject.SetActive(false);
                }
                else if(item.GetComponent<UFB.Character.CharacterController>() != null && item.gameObject.activeSelf)
                {
                    UFB.Character.CharacterController controller = item.GetComponent<UFB.Character.CharacterController>();
                    if (controller.State.stats.health.current != 0)
                    {
                        //UIGameManager.instance.ResourcePanel.OnCharacterValueEvent(controller.State);
                        //UIGameManager.instance.ResourcePanel.gameObject.SetActive(true);
                        UIGameManager.instance.movePanel.gameObject.SetActive(false);
                        UIGameManager.instance.bottomAttackPanel.gameObject.SetActive(false);
                        UIGameManager.instance.tapSelfPanel.gameObject.SetActive(false);

                        if(controller.State.type == (int) USER_TYPE.MONSTER)
                        {
                            UIGameManager.instance.bottomAttackPanel.Init(controller.State);
                            UIGameManager.instance.selectNamePanel.UpdateTarget(tile.transform, controller.State.displayName);
                        }
                        else if(controller.State.type == (int)USER_TYPE.USER)
                        {
                            UIGameManager.instance.tapSelfPanel.InitPanel();
                            UIGameManager.instance.selectNamePanel.UpdateTarget(tile.transform, controller.State.displayName);
                            /*UIGameManager.instance.ResourcePanel.OnCharacterValueEvent(controller.State);
                            UIGameManager.instance.ResourcePanel.gameObject.SetActive(true);*/
                        }
                    }

                }
            }

        }
    }

    private Tile bombPrevTile = null;
    private bool isFeather = false;
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

        if (m.portalNextTileId != "")
        {
            Tile tile = gameBoard.Tiles[m.portalNextTileId];
            tiles.Add(tile);
        }
        
        energyText.text = $"-{m.cost}";
        bombEngeryText.text = $"-{m.cost}";
        featherText.text = $"-{m.featherCount}";
        featherText1.text = $"-{m.featherCount}";
        isFeather = m.featherCount > 0;
        
        HighlightRect.Instance.ClearHighLightRect();
        HighlightRect.Instance.SetHighLightForSpawn(tiles);

        if (tiles.Count > 1) 
        {
            bombPrevTile = tiles[tiles.Count - 2];
        }

        if (character.State.stats.energy.current < m.cost || 
            (character.State.items.ContainsKey(ITEM.Feather) && character.State.items[(int)ITEM.Feather].count < m.featherCount))
        {
            lessPartenergyText.text = $"{Mathf.Max(m.cost - character.State.stats.energy.current, 0)}";
            lessPartFeatherText.text = $"{Mathf.Max(character.State.items.ContainsKey(ITEM.Feather)? (m.featherCount - character.State.items[(int)ITEM.Feather].count): 0, 0)}";
            lessPart.SetActive(true);
            defaultPart.SetActive(false);
        }
        else
        {
            lessPart.SetActive(false);
            defaultPart.SetActive(true);
        }
    }

    public void OnWarpCrystalBtn()
    {
        int wrapNeededCount = Mathf.Abs(int.Parse(wrapText.text));

        if (wrapNeededCount == 0)
        {
            UIGameManager.instance.OnNotificationMessage("error", "WarpCrystal is empty.");
            return;
        }
        
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
        selectedTile = null;
        bombPrevTile = null;
        gameObject.SetActive(false);
        HighlightRect.Instance.ClearHighLightRect();
        UIGameManager.instance.bottomDrawer.CloseBottomDrawer();
    }
    
    private void Update()
    {

    }

}
