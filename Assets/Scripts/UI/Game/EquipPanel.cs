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
        InitScrollView();

        state.powers.ForEach(power =>
        {
            Debug.Log($"power: {power.count}");
            if (power != null && power.count > 0)
            {
                EquipItem go = Instantiate(item, scrollView);
                go.Init(GlobalResources.instance.powers[power.id], power.name, $"LEVEL {power.level}", $"-{1}");
                go.GetComponent<Button>().onClick.AddListener(() => OnClickEquip(power));
                go.gameObject.SetActive(true);
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
                GlobalDefine.CLIENT_MESSAGE.EQUIP_POWER,
                new RequestGetPowerMoveList
                {
                    characterId = CharacterManager.Instance.SelectedCharacter.Id,
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

    public void GetSlotDataList(GetEquipSlotMessage message)
    {
        foreach (var item in equipItems)
        {
            item.ResetImage();
        }
        
        int k = 0;
        foreach (SlotItemData slotdata in message.data)
        {
            equipItems[k].Init(slotdata.power, slotdata.powermoves);
            k++;
        }
    }
    
    public void ClearHighLightItems()
    {
        foreach (var item in equipItems)
        {
            item.highlight.SetActive(false);
        }
    }
}
