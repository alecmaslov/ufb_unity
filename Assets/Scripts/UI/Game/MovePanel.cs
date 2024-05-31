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
    private GameObject globalDirection;

    private bool isLeft = true;
    private bool isRight = true;
    private bool isTop = true;
    private bool isDown = true;

    private float originEnergy;

    Vector2Int _destination;

    // Start is called before the first frame update
    private void Start()
    {

    }

    public void OnCharacterMoved(CharacterMovedMessage m)
    {
        InitMoveBtns();
        if(m.left == 0)
        {
            leftBtn.sprite = sprites[5];
            isLeft = false;
        }
        if(m.right == 0)
        {
            rightBtn.sprite = sprites[5];
            isRight = false;
        }
        if(m.top == 0) 
        {
            topBtn.sprite = sprites[5];
            isTop = false;
        }
        if(m.down == 0)
        {
            bottomBtn.sprite = sprites[5];
            isDown = false;
        }

        Debug.Log($"Tile Pos: {character.CurrentTile.TilePosText}, Tile State: {character.CurrentTile.GetTileState().type}");
    }

    public void Show()
    {
        gameObject.SetActive(true);
        InitMoveBtns();
        currentTile = character.CurrentTile;
        originEnergy = character.State.stats.energy.current;

        character.MoveToTile(character.CurrentTile);

        globalDirection.SetActive(true);
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
}

    public void OnConfirm()
    {
        stepPanel.gameObject.SetActive(true);
        resourcePanel.SetActive(true);
        gameObject.SetActive(false);
        globalDirection.SetActive(false) ;

        Tile tile = character.CurrentTile;
        character.MoveToTile(tile);
    }

    public void OnCancel()
    {
        stepPanel.gameObject.SetActive(true);
        resourcePanel.SetActive(true);
        gameObject.SetActive(false);
        globalDirection.SetActive(false);

        character.CancelMoveToTile(
            currentTile,
            originEnergy
        );
    }

    public void OnMoveClick(string move)
    {
        Tile tile = character.CurrentTile;

        NextCoordinates(tile.Coordinates, move);


        var gameBoard = ServiceLocator.Current.Get<GameBoard>();
        Tile tile1 = gameBoard.GetTileByCoordinates(Coordinates.FromVector2Int(_destination));
        TileState state = tile1.GetTileState();
        Debug.Log("STATE TILE TYPE: " +  state.type);
        if(state.type == "Bridge" || state.type == "Floor")
        {
            NextCoordinates(tile1.Coordinates, move);
        }
        tile1 = gameBoard.GetTileByCoordinates(Coordinates.FromVector2Int(_destination));
        character.MoveToTile(tile1);
        for (int i = 0; i < tile1.transform.childCount; i++)
        {
            GameObject item = tile1.transform.GetChild(i).gameObject;
            if (item.GetComponent<Chest>() != null )
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

                gameObject.SetActive( false );
            }
        }

    }

    private void NextCoordinates(Coordinates coordinates, string move)
    {
        int x = coordinates.X;
        int y = coordinates.Y;

        if (move == "left")
        {
            if (!isLeft) return;
            x--;
            x = x < 0 ? 0 : x;
        }
        else if (move == "right")
        {
            if (!isRight) return;
            x++;
            x = x > 25 ? 25 : x;
        }
        else if (move == "top")
        {
            if (!isTop) return;
            y--;
            y = y < 0 ? 0 : y;
        }
        else if (move == "down")
        {
            if (!isDown) return;
            y++;
            y = y > 25 ? 25 : y;
        }

        _destination.x = x;
        _destination.y = y;
        Debug.Log($"Current Tile pos: {x}, {y}");
    }

    private void Update()
    {
        if (character == null || !gameObject.active) return;

        globalDirection.transform.position = character.transform.position;
    }

}
