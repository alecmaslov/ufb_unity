using UFB.Core;
using UFB.Events;
using UFB.Network.RoomMessageTypes;
using UFB.UI;
using UnityEngine;
using UFB.Character;
using UnityEngine.TextCore.Text;
using UFB.Items;
using UFB.StateSchema;
using UFB.Map;
using UFB.Entities;
using System.Collections.Generic;


public class UIGameManager : MonoBehaviour
{
    public static UIGameManager instance;

    public UFB.Character.CharacterController controller;

    public SpawnPanel spawnPanel;

    public TopHeader TopStatusBar;

    public GameObject TopPanel;

    [SerializeField]
    GameObject BottomStatusBar;

    public ResourcePanel ResourcePanel;

    public EquipPanel equipPanel;

    public StepPanel StepPanel;

    public PowerMovePanel powerMovePanel;

    public UIDirection uIDirection;

    public MovePanel movePanel;

    public AddExtraScore[] scoreTexts;

    public AddStackScore stackScoreText;

    public AttackPanel attackPanel;

    public MerchantPanel merchantPanel;

    public WndPortalPanel wndPortalPanel;

    public List<Portal> portals = new List<Portal> ();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void OnEnable()
    {
        EventBus.Subscribe<ChangeCharacterStateEvent>(OnChangeCharacterStateEvent);
        EventBus.Subscribe<SelectedCharacterEvent>(OnSelectedCharacterEvent);

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

        gameService.SubscribeToRoomMessage<PowerMoveListMessage>(
            "ReceivePowerMoveList",
            equipPanel.OnReceivePowerMoveList
        );

        gameService.SubscribeToRoomMessage<MoveItemMessage>(
            "ReceiveMoveItem",
            movePanel.OnReceiveMoveItem
        );

        gameService.SubscribeToRoomMessage<SetMoveItemMessage>(
            "SetMoveItem",
            movePanel.OnSetMoveItemReceived
        );

        gameService.SubscribeToRoomMessage<AddExtraScoreMessage>(
            "addExtraScore",
            OnReceiveExtraScore
        );

        gameService.SubscribeToRoomMessage<GetBombMessage>(
            "getBombDamage",
            movePanel.OnReceiveGetBombMessage
        );

        gameService.SubscribeToRoomMessage<GetMerchantDataMessage>(
            "getMerchantData",
            merchantPanel.InitMerchantData
        );
        
        gameService.SubscribeToRoomMessage<GetReSpawnMerchantMessage>(
            "respawnMerchant",
            OnReSpawnMerchant
        );

        gameService.SubscribeToRoomMessage<BecomeZombieMessage>("unEquipPowerReceived", OnUnEquipPowerReceived);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ChangeCharacterStateEvent>(OnChangeCharacterStateEvent);
        EventBus.Unsubscribe<SelectedCharacterEvent>(OnSelectedCharacterEvent);
    }

    private void InitSpawn(SpawnInitMessage m)
    {
        spawnPanel.isSpawn = !TopPanel.gameObject.activeSelf;
        TopStatusBar.gameObject.SetActive(true);
        TopPanel.SetActive(true);
        BottomStatusBar.SetActive(true);
        spawnPanel.InitSpawn(m);
    }

    private void OnReSpawnMerchant(GetReSpawnMerchantMessage message)
    {
        Debug.Log("===> respawn event");
        
        Transform target = ServiceLocator.Current.Get<GameBoard>().Tiles[message.tileId].transform;
        SpawnItemEvent sEvent = new SpawnItemEvent(message.oldTileId);
        sEvent.tileId = message.oldTileId;
        sEvent.target = target;
        sEvent.targetTileId = message.tileId;
        EventBus.Publish(
            sEvent
        );
    }

    private void OnSelectedCharacterEvent(SelectedCharacterEvent e) 
    {
        controller = e.controller;
    }

    private void OnChangeCharacterStateEvent(ChangeCharacterStateEvent e)
    {
        CharacterState state = e.state;
        attackPanel.InitCharacterState(state);
        TopStatusBar.OnSelectedCharacterEvent(state);
        ResourcePanel.OnCharacterValueEvent(state);
        StepPanel.OnCharacterStateChanged(state);
        equipPanel.InitEquipList(state);
    }

    private void OnUnEquipPowerReceived(BecomeZombieMessage e)
    {
        powerMovePanel.ClosePowerMovePanel();
    }

    private void OnReceiveExtraScore(AddExtraScoreMessage message)
    {
        foreach (AddExtraScore item in scoreTexts)
        {
            item.OnReceiveExtraScore(message);
        }
        stackScoreText.OnReceiveMessageData(message);

        attackPanel.InitCharacterState(controller.State);
        TopStatusBar.OnSelectedCharacterEvent(controller.State);
        ResourcePanel.OnCharacterValueEvent(controller.State);
        StepPanel.OnCharacterStateChanged(controller.State);
        equipPanel.InitEquipList(controller.State);
    }

    public int GetItemCount(ITEM type)
    {
        int count = 0;

        controller.State.items.ForEach(item => {
            if(item.id == (int) type)
            {
                count = item.count;
                return;
            }
        });

        return count;
    }

    public void OnTest() 
    {
        EventBus.Publish(
            RoomSendMessageEvent.Create(
                "testPath",
                new RequestGetPowerMoveList
                {
                    powerId = 0,
                }
            )
        );
    }
}
