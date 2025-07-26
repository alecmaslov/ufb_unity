using System.Collections;
using System.Collections.Generic;
using UFB.Camera;
using UFB.Character;
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
    Text equipbonusText;
    
    [SerializeField]
    Transform moveList;

    public EquipSlot slot;

    public Item powerItem;


    public Transform equipBonusList;
    public ItemCard bonusItem;
    public Image bonusPowerImage;
    public GameObject equipBonusPanel;
    
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
            /*pm.GetComponent<Button>().onClick.AddListener(() =>
            {
                UFB.Events.EventBus.Publish(
                    RoomSendMessageEvent.Create(
                        GlobalDefine.CLIENT_MESSAGE.GET_HIGHLIGHT_RECT,
                        new RequestGetHighlightRect
                        {
                            characterId = CharacterManager.Instance.SelectedCharacter.Id,
                            powerMoveId = move.id
                        }
                    )
                );
                UIGameManager.instance.targetScreenPanel.InitData(pm.pm);
                gameObject.SetActive(false);
            });*/
        }

        gameObject.SetActive(true);
        UIGameManager.instance.equipPanel.gameObject.SetActive(false);

    }
    
    public void UnEquipPower()
    {

        UFB.Events.EventBus.Publish(
            RoomSendMessageEvent.Create(
                GlobalDefine.CLIENT_MESSAGE.UN_EQUIP_POWER,
                new RequestGetPowerMoveList
                {
                    characterId = CharacterManager.Instance.SelectedCharacter.Id,
                    powerId = powerItem.id,
                }
            )
        );
    }

    public void OnEquipBonus()
    {
        UFB.Events.EventBus.Publish(
            RoomSendMessageEvent.Create(
                GlobalDefine.CLIENT_MESSAGE.EQUIP_BONUS_LIST,
                new RequestGetPowerMoveList
                {
                    characterId = CharacterManager.Instance.SelectedCharacter.Id,
                    powerId = powerItem.id,
                }
            )
        );
    }

    public void ShowEquipBonus(EquipBonus[] bonuses)
    {
        if(bonuses.Length == 0)
        {
            equipbonusText.text = "NO EQUIP BONUS";
            return;
        }
        
        equipbonusText.text = "EQUIP BONUS";
        
        for (int i = 1; i < equipBonusList.childCount; i++)
        {
            Destroy(equipBonusList.GetChild(i).gameObject);
        }
        
        bonusPowerImage.sprite = GlobalResources.instance.powers[bonuses[0].id];

        if (bonuses[0].items != null)
        {
            foreach (var item in bonuses[0].items)
            {
                ItemCard card = Instantiate(bonusItem, equipBonusList.transform);
                card.InitData(item.count.ToString(), GlobalResources.instance.items[item.id], false);
            }
        }
        
        if (bonuses[0].stacks != null)
        {
            foreach (var item in bonuses[0].stacks)
            {
                ItemCard card = Instantiate(bonusItem, equipBonusList.transform);
                card.InitData(item.count.ToString(), GlobalResources.instance.stacks[item.id], false);
            }
        }
        
        if (bonuses[0].randomItems != null)
        {
            foreach (var item in bonuses[0].randomItems)
            {
                ItemCard card = Instantiate(bonusItem, equipBonusList.transform);
                card.InitData(item.count.ToString(), GlobalResources.instance.items[item.id], false);
            }
        }
        
        equipBonusPanel.SetActive(true);
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
