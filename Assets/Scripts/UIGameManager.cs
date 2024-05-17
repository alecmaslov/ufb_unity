using UFB.Core;
using UFB.Events;
using UFB.Network.RoomMessageTypes;
using UFB.UI;
using UnityEngine;
using UFB.Character;


public class UIGameManager : MonoBehaviour
{
    public static UIGameManager instance;

    public UFB.Character.CharacterController controller;

    [SerializeField]
    SpawnPanel spawnPanel;

    [SerializeField]
    TopHeader TopStatusBar;

    [SerializeField]
    public GameObject TopPanel;

    [SerializeField]
    GameObject BottomStatusBar;

    [SerializeField]
    ResourcePanel ResourcePanel;

    [SerializeField]
    EquipPanel equipPanel;

    [SerializeField]
    StepPanel StepPanel;

    [SerializeField]
    PowerMovePanel powerMovePanel;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void OnEnable()
    {
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
    }

    private void OnDisable()
    {
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
        TopStatusBar.OnSelectedCharacterEvent(e);
        ResourcePanel.OnCharacterValueEvent(e);
        StepPanel.OnCharacterStateChanged(e);
        equipPanel.InitEquipList(e.controller.State);
    }
}
