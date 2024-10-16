using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UFB.Core;
using UFB.Network.RoomMessageTypes;
using UFB.Events;
using UnityEngine.TextCore.Text;
using UFB.Map;

public class SpawnPanel : MonoBehaviour
{
    public static SpawnPanel instance;

    [SerializeField]
    GameObject spawnConfirmPanel;

    [SerializeField]
    Text spawnText;

    [SerializeField]
    Sprite[] spawnImages;

    [SerializeField]
    Image spawnImage;

    [SerializeField]
    Image spawnImage1;

    [SerializeField]
    GameObject cancelBtn;

    [SerializeField]
    Image itemImage;

    [SerializeField]
    Image powerImage;

    [SerializeField]
    Text coinText;

    [SerializeField]
    GameObject posPanel;

    [SerializeField]
    GlobalResources global;

    [SerializeField]
    Text moveText;

    [HideInInspector]
    [SerializeField]
    public UFB.Character.CharacterController character;

    private int panelIdx;
    private int itemIdx;
    private int playerId;
    private int powerIdx;
    private int coin;
    private string characterId;
    private string tileId;

    private int step = 0;

    public bool isSpawn = false;
    [SerializeField]
    MovePanel movePanel;

    [SerializeField]
    GameObject sideStep;

    private string _spawnId = "";

    private void Start()
    {
 /*       var gameService = ServiceLocator.Current.Get<GameService>();
        if (gameService.Room == null)
        {
            Debug.LogError("Room is null");
            return;
        }
        gameService.SubscribeToRoomMessage<SpawnInitMessage>(
            "spawnInit",
            InitSpawn
        );*/
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void InitInstance()
    {
        if (instance == null)
            instance = this;
    }

    public void InitSpawn(SpawnInitMessage m)
    {
        Debug.Log($"ddeedddd itemId: {m.item}, powerId: {m.power}");
        itemIdx = m.item;
        powerIdx = m.power;
        coin = m.coin;
        characterId = m.characterId;
        tileId = m.tileId;
        _spawnId = m.spawnId;

        Tile tile = ServiceLocator.Current.Get<GameBoard>().Tiles[tileId];
        //InitSpawnPanel()
        Sprite sprite = m.spawnId == "default"? spawnImages[0] : spawnImages[1];

        InitSpawnPanel(sprite, tile.TilePosText, global.items[itemIdx], global.powers[powerIdx], coin.ToString());

        if(!isSpawn)
        {
            OnConfirmClick();
        }

        gameObject.SetActive(true);
    }

    public void InitSpawnPanel(Sprite panelImage, string posText, Sprite itemSprite, Sprite powerSprite, string coin)
    {
        cancelBtn.SetActive(true);
        spawnConfirmPanel.SetActive(false);
        spawnImage.sprite = panelImage;
        spawnImage1.sprite = panelImage;
        spawnText.text = posText;
        itemImage.sprite = itemSprite;
        powerImage.sprite = powerSprite;
        coinText.text = coin;
        step = 0;
    }

    public void OnConfirmClick()
    {
        if (step == 0)
        {
            posPanel.SetActive(false);
            cancelBtn.SetActive(false);
            spawnConfirmPanel.SetActive(true);
        }
        else if(step == 1) 
        {
            posPanel.SetActive(true);
            spawnConfirmPanel.SetActive(false);
            spawnImage.gameObject.SetActive(false);
            EventBus.Publish(
                new SpawnItemEvent(tileId)
            );
        }
        else if(step == 2)
        {
            //spawnText.text = "MOVE TO";
            EventBus.Publish(
                RoomSendMessageEvent.Create(
                    "getSpawn",
                    new RequestGetSpawnMessage
                    {
                        coinCount = coin,
                        itemId = itemIdx,
                        powerId = powerIdx,
                        spawnId = _spawnId,
                    }
                )
            );
            

            if(!isSpawn)
            {
                movePanel.gameObject.SetActive(true);
            } else
            {
                sideStep.SetActive(true);
            }
            gameObject.SetActive(false);
            moveText.text = "Move To";
        }
        step++;
    }

    public void OnCancelClick()
    {
        if (!isSpawn)
        {
            movePanel.gameObject.SetActive(true);
        }
        gameObject.SetActive(false);
    }
}
