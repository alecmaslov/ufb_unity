using UFB.Core;
using UFB.Events;
using UFB.Network.RoomMessageTypes;
using UFB.UI;
using UnityEngine;
using UFB.Character;
using UnityEngine.TextCore.Text;


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

    private void OnSelectedCharacterEvent(SelectedCharacterEvent e) 
    {
        controller = e.controller;
    }

    private void OnChangeCharacterStateEvent(ChangeCharacterStateEvent e)
    {
        TopStatusBar.OnSelectedCharacterEvent(e);
        ResourcePanel.OnCharacterValueEvent(e);
        StepPanel.OnCharacterStateChanged(e);
        equipPanel.InitEquipList(e.state);
    }

    private void OnUnEquipPowerReceived(BecomeZombieMessage e)
    {
        powerMovePanel.ClosePowerMovePanel();
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
