using System.Collections;
using System.Collections.Generic;
using UFB.Character;
using UFB.Core;
using UFB.Events;
using UFB.Network.RoomMessageTypes;
using UFB.StateSchema;
using UFB.UI;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class EquipPanel : MonoBehaviour
{
    public static EquipPanel instance;

    [SerializeField]
    Transform scrollView;

    [SerializeField]
    EquipItem item;

    [SerializeField]
    List<EquipSlot> equipItems;

    [SerializeField]
    PowerMovePanel powerMovePanel;

    private Item seletedItem = null;

    public int slotIdx = 0;

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

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
    }

    public void OnInitEquipView(int slot)
    {
        slotIdx = slot;
        if(gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);
        InitEquipList(CharacterManager.Instance.PlayerCharacter.State);
        UIGameManager.instance.powerMovePanel.gameObject.SetActive(false);
    }

    public void InitEquipList(CharacterState state)
    {
        Debug.Log($"power count: {state.powers.Count}");
        InitScrollView();

        state.powers.ForEach(power =>
        {
            if (power != null && power.count > 0)
            {
                EquipItem go = Instantiate(item, scrollView);
                go.Init(GlobalResources.instance.powers[power.id], power.name, $"LEVEL {power.level}", $"-{power.cost}");
                go.GetComponent<Button>().onClick.AddListener(() => OnClickEquip(power));
                go.gameObject.SetActive(true);

                power.OnChange(() =>
                {
                    Debug.Log($"power count: {power.count}");
                    if(power.count == 0)
                    {
                        Destroy(go.gameObject);
                    }
                });
            }
        });
    }

    private void InitScrollView()
    {
        for (int i = 1; i < scrollView.childCount; i++)
        {
            Destroy(scrollView.GetChild(i).gameObject);
        }
    }

    public void OnClickEquip(Item equip)
    {
        Debug.Log($"equip logic system..");

        EventBus.Publish(
            RoomSendMessageEvent.Create(
                "getPowerMoveList",
                new RequestGetPowerMoveList
                {
                    powerId = equip.id
                }
            )
        );

        seletedItem = equip;
    }

    public void OnReceivePowerMoveList(PowerMoveListMessage message)
    {
        powerMovePanel.Init(seletedItem, equipItems[slotIdx], message.powermoves);

        equipItems[slotIdx].Init(seletedItem, message.powermoves);

        gameObject.SetActive(false);
    }
}
