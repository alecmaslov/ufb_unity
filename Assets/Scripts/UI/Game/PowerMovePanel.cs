using System.Collections;
using System.Collections.Generic;
using UFB.Camera;
using UFB.Events;
using UFB.Network.RoomMessageTypes;
using UFB.StateSchema;
using UFB.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PowerMovePanel : MonoBehaviour
{
    public static PowerMovePanel instance;

    [SerializeField]
    PowerMoveItem moveitem;

    [SerializeField]
    Image powerImage;

    [SerializeField]
    Text powerNameText;

    [SerializeField]
    Text powerLevelText;

    [SerializeField]
    Transform moveList;

    public EquipSlot slot;

    public Item powerItem;

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

    public void Init(Item item, EquipSlot slt, PowerMove[] moves)
    {

        if(gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            return;
        }

        slot = slt;
        powerItem = item;
        powerImage.sprite = GlobalResources.instance.powers[item.id];
        powerNameText.text = item.name;
        powerLevelText.text = $"LEVEL {item.level}";

        //INIT MOVE LIST PART
        InitMoveList();
        foreach (PowerMove move in moves)
        {
            PowerMoveItem pm = Instantiate(moveitem, moveList);
            pm.Init(move);
            pm.gameObject.SetActive(true);
            pm.GetComponent<Button>().onClick.AddListener(() =>
            {
                UIGameManager.instance.attackPanel.Init(pm);
            });
        }

        gameObject.SetActive(true);
        UIGameManager.instance.equipPanel.gameObject.SetActive(false);

    }

    public void UnEquipPower()
    {

        UFB.Events.EventBus.Publish(
            RoomSendMessageEvent.Create(
                "unEquipPower",
                new RequestGetPowerMoveList
                {
                    powerId = powerItem.id,
                }
            )
        );
    }

    public void ClosePowerMovePanel()
    {
        slot.ResetImage();
        //EquipPanel.instance.OnInitEquipView(slot.slotIdx);
        gameObject.SetActive(false);
    }

    private void InitMoveList()
    {
        for(int i = 1; i < moveList.childCount; i++)
        {
            Destroy(moveList.GetChild(i).gameObject);
        }
    }

}
