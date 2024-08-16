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
using UFB.Events;
using UI.ThreeDimensional;


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

    public TurnPanel turnPanel;

    public TargetScreenPanel targetScreenPanel;

    public GameObject bottomDrawer;

    public List<Portal> portals = new List<Portal> ();

    public GameObject[] dices;

    #region public values

    public float curTurnTime = 120;
    public bool isPlayerTurn = false;

    #endregion

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
        
        gameService.SubscribeToRoomMessage<TurnMessage>(GlobalDefine.SERVER_MESSAGE.INIT_TURN,InitTurn);
        gameService.SubscribeToRoomMessage<TurnChangeMessage>(GlobalDefine.SERVER_MESSAGE.TURN_CHANGED,TurnChanged);
        gameService.SubscribeToRoomMessage<SpawnInitMessage>(GlobalDefine.SERVER_MESSAGE.SPAWN_INIT, InitSpawn);
        gameService.SubscribeToRoomMessage<PowerMoveListMessage>(GlobalDefine.SERVER_MESSAGE.RECEIVE_POWERMOVE_LIST, equipPanel.OnReceivePowerMoveList);
        gameService.SubscribeToRoomMessage<MoveItemMessage>(GlobalDefine.SERVER_MESSAGE.RECEIVE_MOVEITEM, movePanel.OnReceiveMoveItem);
        gameService.SubscribeToRoomMessage<SetMoveItemMessage>(GlobalDefine.SERVER_MESSAGE.SET_MOVEITEM, movePanel.OnSetMoveItemReceived);
        gameService.SubscribeToRoomMessage<AddExtraScoreMessage>(GlobalDefine.SERVER_MESSAGE.ADD_EXTRA_SCORE, OnReceiveExtraScore);
        gameService.SubscribeToRoomMessage<GetBombMessage>( GlobalDefine.SERVER_MESSAGE.GET_BOMB_DAMAGE, movePanel.OnReceiveGetBombMessage );
        gameService.SubscribeToRoomMessage<GetMerchantDataMessage>(GlobalDefine.SERVER_MESSAGE.GET_MERCHANT_DATA, merchantPanel.InitMerchantData);
        gameService.SubscribeToRoomMessage<GetReSpawnMerchantMessage>( GlobalDefine.SERVER_MESSAGE.RESPAWN_MERCHANT , OnReSpawnMerchant);
        gameService.SubscribeToRoomMessage<BecomeZombieMessage>(GlobalDefine.SERVER_MESSAGE.UNEQUIP_POWER_RECEIVED, OnUnEquipPowerReceived);
        gameService.SubscribeToRoomMessage<SetHighLightRectMessage>(GlobalDefine.SERVER_MESSAGE.SET_HIGHLIGHT_RECT, OnSetHighLightRectReceived);

    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ChangeCharacterStateEvent>(OnChangeCharacterStateEvent);
        EventBus.Unsubscribe<SelectedCharacterEvent>(OnSelectedCharacterEvent);
    }

    private void InitTurn(TurnMessage e)
    {
        isPlayerTurn = controller.Id == e.characterId;
        turnPanel.InitData(e.curTime);
        curTurnTime = e.curTime;
    }

    private void TurnChanged(TurnChangeMessage e) 
    {
        CharacterManager.Instance.OnSelectCharacter(e.characterId);
        curTurnTime = e.curTime;
        isPlayerTurn = controller.Id == e.characterId;
        turnPanel.InitData(curTurnTime);
    }

    private void InitSpawn(SpawnInitMessage m)
    {
        spawnPanel.isSpawn = !TopPanel.gameObject.activeSelf;
        TopStatusBar.gameObject.SetActive(true);
        TopPanel.SetActive(true);
        BottomStatusBar.SetActive(true);
        spawnPanel.InitSpawn(m);
    }

    private void OnSetHighLightRectReceived(SetHighLightRectMessage e)
    {
        List<Tile> tiles = new List<Tile>();

        foreach(var id in e.tileIds)
        {
            if(id != "")
            {
                tiles.Add(ServiceLocator.Current.Get<GameBoard>().Tiles[id]);
            }
        }

        HighlightRect.Instance.SetHighLightRect(tiles);

    }


    private void OnReSpawnMerchant(GetReSpawnMerchantMessage message)
    {
        Debug.Log("===> respawn event");
        
        Tile target = ServiceLocator.Current.Get<GameBoard>().Tiles[message.tileId];
        SpawnItemEvent sEvent = new SpawnItemEvent(message.oldTileId);
        sEvent.tileId = message.oldTileId;
        sEvent.target = target.transform;
        sEvent.targetTileId = message.tileId;
        sEvent.tile = target;
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

    public void OnChangeMonsterControl()
    {

    }

    public void OnDiceStop()
    {

    }

    public void OnDiceStart()
    {

    }

    #region Unity Function

    private void Update()
    {
        if (curTurnTime <= 0) 
        {
            if (isPlayerTurn)
            {
                EventBus.Publish(
                    RoomSendMessageEvent.Create(
                        "endTurn",
                        new RequestEndTurnMessage
                        {
                            characterId = controller.Id,
                        }
                    )
                );
            }
            isPlayerTurn = false;
        }
        else
        {
            turnPanel.SetTurnTime(curTurnTime);
            curTurnTime -= Time.deltaTime;
        }
    }
    #endregion
}
