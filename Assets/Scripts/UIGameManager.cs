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
using System.Net.NetworkInformation;
using UFB.Interactions;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;




public class UIGameManager : MonoBehaviour
{
    public static UIGameManager instance;

    [HideInInspector]
    public UFB.Character.CharacterController controller;

    public SpawnPanel spawnPanel;

    public TopHeader TopStatusBar;

    public GameObject TopPanel;

    public GameObject BottomStatusBar;

    public ResourcePanel ResourcePanel;

    public EquipPanel equipPanel;

    public StepPanel StepPanel;

    public PowerMovePanel powerMovePanel;

    public UIDirection uIDirection;

    public MovePanel movePanel;

    public AddExtraScore[] scoreTexts;

    public AddStackScore stackScoreText;

    public MerchantPanel merchantPanel;

    public WndPortalPanel wndPortalPanel;

    public TurnPanel turnPanel;

    public PunchPanel punchPanel;

    public BottomDrawer bottomDrawer;

    public List<Portal> portals = new List<Portal> ();

    public GameObject[] dices;

    public ErrorPanel errorPanel;

    public EquipBonusPanel equipBonusPanel;

    public StackTurnStartPanel stackTurnStartPanel;

    public ToastPanel toastPanel;

    public EndPanel endPanel;

    public FollowWorld reviveStack;

    public DefencePanel defencePanel;

    public RewardBonusPanel rewardBonusPanel;

    public BottomAttackPanel bottomAttackPanel;

    public BottomDefeatPanel bottomDefeatPanel;

    public TapSelfPanel tapSelfPanel;

    public SelectSpawnPanel selectSpawnPanel;

    #region public values

    public float curTurnTime = GlobalDefine.TURN_TIME;
    public bool isPlayerTurn = false;

    public bool isMoveTileStatus = false;
    public float delayNextTurnTime = 2f;
    public float curNextTurnTime = 0f;
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
        
        gameService.SubscribeToRoomMessage<TurnMessage>(GlobalDefine.SERVER_MESSAGE.INIT_TURN, InitTurn);
        gameService.SubscribeToRoomMessage<TurnChangeMessage>(GlobalDefine.SERVER_MESSAGE.TURN_CHANGED,TurnChanged);
        gameService.SubscribeToRoomMessage<SpawnInitMessage>(GlobalDefine.SERVER_MESSAGE.SPAWN_INIT, InitSpawn);
        gameService.SubscribeToRoomMessage<PowerMoveListMessage>(GlobalDefine.SERVER_MESSAGE.RECEIVE_POWERMOVE_LIST, equipPanel.OnReceivePowerMoveList);
        gameService.SubscribeToRoomMessage<MoveItemMessage>(GlobalDefine.SERVER_MESSAGE.RECEIVE_MOVEITEM, movePanel.OnReceiveMoveItem);
        gameService.SubscribeToRoomMessage<SetMoveItemMessage>(GlobalDefine.SERVER_MESSAGE.SET_MOVEITEM, movePanel.OnSetMoveItemReceived);
        gameService.SubscribeToRoomMessage<AddExtraScoreMessage>(GlobalDefine.SERVER_MESSAGE.ADD_EXTRA_SCORE, OnReceiveExtraScore);
        gameService.SubscribeToRoomMessage<GetBombMessage>( GlobalDefine.SERVER_MESSAGE.GET_BOMB_DAMAGE, movePanel.OnReceiveGetBombMessage );
        gameService.SubscribeToRoomMessage<GetMerchantDataMessage>(GlobalDefine.SERVER_MESSAGE.GET_MERCHANT_DATA, merchantPanel.InitMerchantData);
        gameService.SubscribeToRoomMessage<GetReSpawnMerchantMessage>( GlobalDefine.SERVER_MESSAGE.RESPAWN_MERCHANT , OnReSpawnMerchant);
        gameService.SubscribeToRoomMessage<UnEquipItemMessage>(GlobalDefine.SERVER_MESSAGE.UNEQUIP_POWER_RECEIVED, OnUnEquipPowerReceived);
        gameService.SubscribeToRoomMessage<SetHighLightRectMessage>(GlobalDefine.SERVER_MESSAGE.SET_HIGHLIGHT_RECT, OnSetHighLightRectReceived);
        gameService.SubscribeToRoomMessage<SetDiceRollMessage>(GlobalDefine.SERVER_MESSAGE.SET_DICE_ROLL, OnSetDiceRoll);
        gameService.SubscribeToRoomMessage<EnemyDiceRollMessage>(GlobalDefine.SERVER_MESSAGE.ENEMY_DICE_ROLL, OnEnemyDiceRoll);
        gameService.SubscribeToRoomMessage<GetTurnStartEquipBonusMessage>(GlobalDefine.SERVER_MESSAGE.GET_TURN_START_EQUIP, GetTurnStartBonus);
        gameService.SubscribeToRoomMessage<SetCharacterPositionMessage>(GlobalDefine.SERVER_MESSAGE.SET_CHARACTER_POSITION, OnSetCharacterPosition);
        gameService.SubscribeToRoomMessage<GetStackOnStartMessage>(GlobalDefine.SERVER_MESSAGE.GET_STACK_ON_TURN_START, OnGetStackOnStartTurn);

        gameService.SubscribeToRoomMessage<ToastBanStackMessage>(GlobalDefine.SERVER_MESSAGE.RECEIVE_BAN_STACK, OnReceiveBanStackMessage);
        gameService.SubscribeToRoomMessage<ToastPerkMessage>(GlobalDefine.SERVER_MESSAGE.RECEIVE_PERK_TOAST, OnReceivePerkMessage);
        gameService.SubscribeToRoomMessage<ToastStackPerkMessage>(GlobalDefine.SERVER_MESSAGE.RECEIVE_STACK_PERK_TOAST, OnReceiveStackPerkMessage);
        gameService.SubscribeToRoomMessage<ToastBanStackMessage>(GlobalDefine.SERVER_MESSAGE.RECEIVE_STACK_ITEM_TOAST, OnReceiveStackItemMessage);

        gameService.SubscribeToRoomMessage<GameEndMessage>(GlobalDefine.SERVER_MESSAGE.GAME_END_STATUS, OnGameEndMessage);
        gameService.SubscribeToRoomMessage<GameEndMessage>(GlobalDefine.SERVER_MESSAGE.STACK_REVIVE_ACTIVE, OnReceiveReviveStackMessage);

        // DEFENCE PANEL PART - AI, other player's attack
        gameService.SubscribeToRoomMessage<DefenceAttackMessage>(GlobalDefine.SERVER_MESSAGE.DEFENCE_ATTACK, OnReceiveDefenceAttackMessage);
        gameService.SubscribeToRoomMessage<EndAttackMessage>(GlobalDefine.SERVER_MESSAGE.AI_END_ATTACK, OnReceiveAIDefenceEndAttackMessage);

        // DEAD MONSTER PART
        gameService.SubscribeToRoomMessage<DeadMonsterMessage>(GlobalDefine.SERVER_MESSAGE.DEAD_MONSTER, OnReceiveDeadMonsterMessage);

        // REWARD BONUS...
        gameService.SubscribeToRoomMessage<RewardBonusMessage>(GlobalDefine.SERVER_MESSAGE.REWARD_BONUS, OnReceiveRewardBonusMessage);

        // CLICK TILE WHEN MOVE
        gameService.SubscribeToRoomMessage<SetMovePointMessage>(GlobalDefine.SERVER_MESSAGE.SET_MOVE_POINT, OnReceiveSetMovePointMessage);

        //REQUEST SLOT ITEMS
        gameService.SubscribeToRoomMessage<GetEquipSlotMessage>(GlobalDefine.SERVER_MESSAGE.GET_EQUIP_SLOT_LIST, OnGetEquipSlotList);
        
        gameService.SubscribeToRoomMessage<TurnChangeMessage>(GlobalDefine.SERVER_MESSAGE.RECONNECT_ROOM, GetRoomStateData);
        
        // Request Equip Bonus Detail
        gameService.SubscribeToRoomMessage<GetTurnStartEquipBonusMessage>(GlobalDefine.SERVER_MESSAGE.EQUIP_BONUS_LIST, OnShowEquipBonusReceived);

        
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ChangeCharacterStateEvent>(OnChangeCharacterStateEvent);
        EventBus.Unsubscribe<SelectedCharacterEvent>(OnSelectedCharacterEvent);
    }

    private void InitTurn(TurnMessage e)
    {
        isPlayerTurn = CharacterManager.Instance.PlayerCharacter.Id == e.characterId;
        turnPanel.InitData(e.curTime);
        curTurnTime = e.curTime;

        if (isPlayerTurn) 
        { 
            CameraManager.instance.isRotate = true;
            //CameraManager.instance.isCameraMove = false;
        }

    }

    private void GetRoomStateData(TurnChangeMessage e)
    {
        
        Debug.Log("GetRoomStateData: " + e.curTime + ", " + e.characterId);
        curTurnTime = e.curTime;
        CharacterManager.Instance.OnSelectCharacter(e.characterId);
        isPlayerTurn = CharacterManager.Instance.PlayerCharacter.Id == e.characterId;

        spawnPanel.isSpawn = !TopPanel.gameObject.activeSelf;
        TopStatusBar.gameObject.SetActive(true);
        TopPanel.SetActive(true);
        bottomDrawer.gameObject.SetActive(true);

        if (isPlayerTurn) 
        {
            bottomDefeatPanel.gameObject.SetActive(false);
            reviveStack.gameObject.SetActive(false);
        }
        else
        {
            bottomAttackPanel.gameObject.SetActive(false);
            movePanel.gameObject.SetActive(false);
            tapSelfPanel.gameObject.SetActive(false);
            equipBonusPanel.gameObject.SetActive(false);
            bottomDrawer.CloseBottomDrawer();
        }
        
        HighlightRect.Instance.ClearHighLightRect();
        InteractionManager.Instance.isSpawn = false;
        
        //REQUEST EQUIP ITEM LIST
        EventBus.Publish(
            RoomSendMessageEvent.Create(
                GlobalDefine.SERVER_MESSAGE.GET_EQUIP_SLOT_LIST,
                new RequestCharacterId
                {
                    characterId = controller.Id,
                }
            )
        );
    }
    
    private void TurnChanged(TurnChangeMessage e) 
    {
        CharacterManager.Instance.OnSelectCharacter(e.characterId);
        
        curTurnTime = e.curTime;
        isPlayerTurn = CharacterManager.Instance.PlayerCharacter.Id == e.characterId;
        turnPanel.InitData(curTurnTime);
        
        if (isPlayerTurn) 
        {
            bottomDefeatPanel.gameObject.SetActive(false);
            bottomDrawer.gameObject.SetActive(true);
            reviveStack.gameObject.SetActive(false);
            EventBus.Publish(
                RoomSendMessageEvent.Create(
                    GlobalDefine.CLIENT_MESSAGE.TURN_START_EQUIP,
                    new RequestEndTurnMessage
                    {
                        characterId = controller.Id,
                    }
                )
            );
        } 
        else
        {
            bottomAttackPanel.gameObject.SetActive(false);
            movePanel.gameObject.SetActive(false);
            tapSelfPanel.gameObject.SetActive(false);
            equipBonusPanel.gameObject.SetActive(false);
            bottomDrawer.CloseBottomDrawer();
            // bottomDrawer.SetActive(false);
        }
        HighlightRect.Instance.ClearHighLightRect();
        
        //REQUEST EQUIP ITEM LIST
        EventBus.Publish(
            RoomSendMessageEvent.Create(
                GlobalDefine.SERVER_MESSAGE.GET_EQUIP_SLOT_LIST,
                new RequestCharacterId
                {
                    characterId = controller.Id,
                }
            )
        );

    }

    private void GetTurnStartBonus(GetTurnStartEquipBonusMessage e)
    {
        equipBonusPanel.InitData(e.bonuses);
    }

    private void OnGameEndMessage( GameEndMessage e )
    {
        endPanel.InitData((END_TYPE) e.endType);
    }

    private void OnReceiveReviveStackMessage(GameEndMessage e)
    {
        reviveStack.lookAt = CharacterManager.Instance.GetCharacterFromId(e.characterId).transform;
        reviveStack.gameObject.SetActive(true);
    }

    private void InitSpawn(SpawnInitMessage m)
    {
        Debug.LogError("InitSpawn: " + m.characterId);
        
        spawnPanel.isSpawn = !TopPanel.gameObject.activeSelf;
        TopStatusBar.gameObject.SetActive(true);
        TopPanel.SetActive(true);
        BottomStatusBar.SetActive(true);
        spawnPanel.InitSpawn(m);
    }

    private void OnReceiveSetMovePointMessage(SetMovePointMessage m)
    {
        movePanel.OnSetMovePointMessage(m);
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

    private void OnSetCharacterPosition(SetCharacterPositionMessage e) 
    {
        Debug.LogError("move set position: " + e.characterId);
        Debug.LogError(e.path);
        CharacterManager.Instance.OnSetCharacterTilePosition(e);
    }

    private void OnGetStackOnStartTurn(GetStackOnStartMessage e) {
        stackTurnStartPanel.InitData(e);
    }

    private void OnSetDiceRoll(SetDiceRollMessage e) {
        if (stackTurnStartPanel.isStackTurn)
        {
            stackTurnStartPanel.OnLanuchDiceRoll(e);
        }
        else if (defencePanel.gameObject.activeSelf)
        {
            defencePanel.OnLanuchDiceRoll(e);
        }
        else if (bottomDefeatPanel.gameObject.activeSelf) 
        {
            bottomDefeatPanel.OnLanuchDiceRoll(e);
        }
        else if (bottomAttackPanel.gameObject.activeSelf) 
        {
            bottomAttackPanel.OnLanuchDiceRoll(e);
        }
    }

    private void OnEnemyDiceRoll(EnemyDiceRollMessage e) 
    {
        if (isPlayerTurn) 
        { 
            //attackPanel.OnEnemyStackDiceRoll(e);
            bottomAttackPanel.OnEnemyStackDiceRoll(e);
        }
        else
        {
            //defencePanel.OnEnemyStackDiceRoll(e);
            bottomDefeatPanel.OnEnemyStackDiceRoll(e);

        }
    }


    private void OnReSpawnMerchant(GetReSpawnMerchantMessage message)
    {
        Debug.Log("===> respawn event");
        
        Tile target = ServiceLocator.Current.Get<GameBoard>().Tiles[message.tileId];
        SpawnItemEvent sEvent = new SpawnItemEvent();
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
        TopStatusBar.OnSelectedCharacterEvent(CharacterManager.Instance.PlayerCharacter.State);
        ResourcePanel.OnCharacterValueEvent(state);
        StepPanel.OnCharacterStateChanged(state);
        equipPanel.InitEquipList(state);

        if(CharacterManager.Instance.PlayerCharacter.State.id == state.id)
        {
            CameraManager.instance.cameraTarget.parent = null;
        } 
        else
        {
            CameraManager.instance.cameraTarget.parent = CharacterManager.Instance.GetCharacterFromId(state.id).transform;
        }
        CameraManager.instance.cameraTarget.localPosition = Vector3.zero;
    }

    private void OnUnEquipPowerReceived(UnEquipItemMessage e)
    {
        powerMovePanel.ClosePowerMovePanel();
    }

    private void OnShowEquipBonusReceived(GetTurnStartEquipBonusMessage e)
    {
        powerMovePanel.ShowEquipBonus(e.bonuses);
    }
    
    private void OnReceiveBanStackMessage(ToastBanStackMessage e)
    {
        toastPanel.InitBanStackMessage(e);
    }

    private void OnReceivePerkMessage(ToastPerkMessage e)
    {
        toastPanel.InitPerkPopupMessage(e);
    }

    private void OnReceiveStackPerkMessage(ToastStackPerkMessage e)
    {
        toastPanel.InitStackPerkMessage(e);
    }

    private void OnReceiveStackItemMessage(ToastBanStackMessage e)
    {
        toastPanel.InitStackItemMessage(e);
    }

    // DEFENCE PANEL PART - AI ATTACK
    private void OnReceiveDefenceAttackMessage(DefenceAttackMessage e)
    {
        CharacterState origin = CharacterManager.Instance.GetCharacterFromId(e.originId).State;
        CharacterState target = CharacterManager.Instance.GetCharacterFromId(e.targetId).State;
        //defencePanel.Init(e.pm, origin, target);
        bottomDefeatPanel.Init(e.pm, origin, target);
    }

    private void OnReceiveAIDefenceEndAttackMessage(EndAttackMessage e)
    {
        // defencePanel.OnClosePanel();
        bottomDefeatPanel.OnClosePanel();
    }

    private void OnReceiveDeadMonsterMessage(DeadMonsterMessage e) 
    {
        UFB.Character.CharacterController obj = CharacterManager.Instance.GetCharacterFromId(e.characterId);

        if (obj != null) 
        {
            obj.gameObject.SetActive(false);
        }
    }

    private void OnReceiveRewardBonusMessage(RewardBonusMessage e) 
    {
        Debug.Log("Receive reward item");
        if (CharacterManager.Instance.SelectedCharacter.Id == e.characterId) 
        { 
            rewardBonusPanel.InitData(e);
        }
    }

    private void OnReceiveExtraScore(AddExtraScoreMessage message)
    {

        EventBus.Publish(message);

       /* foreach (AddExtraScore item in scoreTexts)
        {
            item.OnReceiveExtraScore(message);
        }*/
        stackScoreText.OnReceiveMessageData(message);
        ResourcePanel.OnCharacterValueEvent(controller.State);
        StepPanel.OnCharacterStateChanged(controller.State);
        equipPanel.InitEquipList(controller.State);

        //TopStatusBar.OnSelectedCharacterEvent(CharacterManager.Instance.PlayerCharacter.State);

    }

    private void OnGetEquipSlotList(GetEquipSlotMessage message)
    {
        equipPanel.GetSlotDataList(message);
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
        EventBus.Publish(
            RoomSendMessageEvent.Create(
                GlobalDefine.CLIENT_MESSAGE.END_TURN,
                new RequestEndTurnMessage
                {
                    characterId = controller.Id,
                }
            )
        );
        isPlayerTurn = false;
    }

    public void OnReduceHealthCharacter()
    {
        EventBus.Publish(
            RoomSendMessageEvent.Create(
                "testHealth",
                new RequestTestMessage
                {
                    characterId = controller.Id,
                }
            )
        );
    }

    public void InitSpawnItems(MapState mapState)
    {
        int i = 0;
        foreach (UFB.StateSchema.SpawnEntity entity in mapState.spawnEntities.items.Values)
        {
            // var tile = Tiles[entity.tileId];
            Debug.Log(
                $"Spawning entity {entity.prefabAddress}"
            );
            if(entity.prefabAddress == "Entities/chest" /*|| entity.prefabAddress == "Entities/ItemBag"*/)
            {
                AddSpawnItem(entity, i);
                i++;
            }
        }
        HighlightRect.Instance.SetHighLightForSpawn(itemTileList);
    }
    
    public List<Tile> itemTileList = new List<Tile>();
    public void AddSpawnItem(UFB.StateSchema.SpawnEntity spawnEntity, int i)
    {
        var tile = ServiceLocator.Current.Get<GameBoard>().Tiles[spawnEntity.tileId];
        itemTileList.Add(tile);
    }
    
    public bool IsCharacterCameraControl()
    {
        return !(spawnPanel.gameObject.activeSelf || ResourcePanel.gameObject.activeSelf || equipPanel.gameObject.activeSelf || powerMovePanel.gameObject.activeSelf || 
            merchantPanel.gameObject.activeSelf || wndPortalPanel.gameObject.activeSelf || /*turnPanel.gameObject.activeSelf ||*/ 
            punchPanel.gameObject.activeSelf || errorPanel.gameObject.activeSelf || equipBonusPanel.gameObject.activeSelf || stackTurnStartPanel.gameObject.activeSelf ||
            endPanel.gameObject.activeSelf || defencePanel.gameObject.activeSelf || rewardBonusPanel.gameObject.activeSelf);
    }

    public void OnChangeMouseTileStatus()
    {
        isMoveTileStatus = !isMoveTileStatus;
    }

    public void OnNotificationMessage(string title, string message)
    {
        NotificationMessage _message = new NotificationMessage();
        _message.message = message;
        _message.type = title;

        EventBus.Publish(new RoomReceieveMessageEvent<NotificationMessage>(_message));
    }

    public int GetItemCount(ITEM type, CharacterState characterState)
    {
        int k = 0;

        characterState.items.ForEach(item =>
        {
            if (item.id == (int)type)
            {
                k = item.count;
            }
        });
        
        return k;
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
                        GlobalDefine.CLIENT_MESSAGE.END_TURN,
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

        if (CharacterManager.Instance.PlayerCharacter.State.stats.energy.current == 0 && isPlayerTurn)
        {
            curNextTurnTime += Time.deltaTime;
            if (curNextTurnTime > delayNextTurnTime)
            {
                curNextTurnTime = 0;
                EventBus.Publish(
                    RoomSendMessageEvent.Create(
                        GlobalDefine.CLIENT_MESSAGE.END_TURN,
                        new RequestEndTurnMessage
                        {
                            characterId = controller.Id,
                        }
                    )
                );
                isPlayerTurn = false;
            }
        }
        else
        {
            curNextTurnTime = 0;
        }
        
    }
    #endregion
}
