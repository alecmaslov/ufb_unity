using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UFB.Core;
using UFB.Network.RoomMessageTypes;
using UFB.Events;
using UnityEngine.TextCore.Text;

public class SpawnPanel : MonoBehaviour
{
    [SerializeField]
    GameObject spawnConfirmPanel;

    [SerializeField]
    Text spawnText;

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

    [HideInInspector]
    [SerializeField]
    public UFB.Character.CharacterController character;

    private int panelIdx;
    private int itemIdx;
    private int playerId;
    private int powerIdx;
    private int coin;
    private string characterId;

    private int step = 0;

    private void Start()
    {
        var gameService = ServiceLocator.Current.Get<GameService>();
        if (gameService.Room == null)
        {
            Debug.LogError("Room is null");
            return;
        }
        gameService.SubscribeToRoomMessage<SpawnInitMessage>(
            "spawnInit",
            InitSpawn
        );
    }

    private void InitSpawn(SpawnInitMessage m)
    {
        Debug.Log("ddeedddd");
        itemIdx = m.itemId;
        powerIdx = m.powerId;
        coin = m.coin;
        characterId = m.characterId;
        //InitSpawnPanel()

        InitSpawnPanel(spawnImage.sprite, character.CurrentTile.TilePosText, global.items[itemIdx], global.powers[powerIdx], coin.ToString());
    }

    public void InitSpawnPanel(Sprite panelImage, string posText, Sprite itemSprite, Sprite powerSprite, string coin)
    {
        cancelBtn.SetActive(true);
        spawnConfirmPanel.SetActive(false);
        spawnImage.sprite = panelImage;
        spawnImage1.sprite = itemSprite;
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
        else
        {
            posPanel.SetActive(true);
            spawnConfirmPanel.SetActive(false);
            spawnImage.gameObject.SetActive(false);
            //spawnText.text = "MOVE TO";
            EventBus.Publish(
                RoomSendMessageEvent.Create(
                    "getSpawn",
                    new RequestGetSpawnMessage
                    {
                        coinCount = coin,
                        itemId = itemIdx,
                        powerId = powerIdx,

                    }
                )
            );
            gameObject.SetActive(false);

        }
        step++;
    }

    public void OnCancelClick()
    {

    }
}
