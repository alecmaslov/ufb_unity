using System.Collections;
using System.Collections.Generic;
using UFB.Events;
using UFB.Items;
using UFB.Network.RoomMessageTypes;
using UFB.StateSchema;
using UFB.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AttackPanel : MonoBehaviour
{
    [HideInInspector]
    public PowerMoveItem powerMoveItem;

    [HideInInspector]
    public PowerMove pm;

    [SerializeField]
    private LinearIndicatorBar _healthBar;

    [SerializeField]
    private LinearIndicatorBar _energyBar;

    [SerializeField]
    private LinearIndicatorBar _ultimateBar;

    [SerializeField]
    Text meleeText;

    [SerializeField]
    Text manaText;

    [SerializeField]
    Text powermoveText;

    [SerializeField]
    Image powermoveImage;

    [SerializeField]
    Image powermoveResultImage;

    [SerializeField]
    Transform powerCostList;

    [SerializeField]
    Transform resultList;

    [SerializeField]
    ItemCard costItem;

    [SerializeField]
    ItemCard resultItem;

    [SerializeField]
    GameObject resultPanel;

    [SerializeField]
    Transform panelResutlList;

    [SerializeField]
    ItemCard panelResultItem;

    [SerializeField]    
    GameObject panelDetail;

    [SerializeField]
    GameObject resultPanelDetail;

    public Transform enemyStackList;

    public ItemCard enemyStackItem;

    public Text powerText;

    public TopHeader topHeader;

    public GameObject diceRect;

    // ENEMY DICE
    public Image enemyStackImage;
    public GameObject enemyStackDiceRect;

    public Image addStackImage;

    public void InitCharacterState(CharacterState e)
    {
        InitOthers();
    }

    public void InitEnemyState()
    {
        if (HighlightRect.Instance.selectedMonster != null) 
        { 
            topHeader.OnSelectedCharacterEvent(HighlightRect.Instance.selectedMonster.State);
            for(int i = 1; i < enemyStackList.childCount; i++)
            {
                Destroy(enemyStackList.GetChild(i).gameObject);
            }

            HighlightRect.Instance.selectedMonster.State.stacks.ForEach(stack =>
            {
                if(stack.count > 0)
                {
                    ItemCard ic = Instantiate(enemyStackItem, enemyStackList);
                    ic.InitDate(stack.count.ToString(), GlobalResources.instance.stacks[stack.id]);
                    ic.gameObject.SetActive(true);
                }
            });
            
        } 
        else
        {

        }
    }

    public void Init(PowerMoveItem _power)
    {
        InitEnemyState();
        powerMoveItem = _power;
        pm = powerMoveItem.pm;
        InitDiceData();
        powermoveImage.gameObject.SetActive(true);
        powermoveImage.sprite = GlobalResources.instance.powers[pm.powerImageId];
        powermoveResultImage.sprite = GlobalResources.instance.powers[pm.powerImageId];
        powermoveText.text = pm.name.ToString();
        powerText.text = UIGameManager.instance.powerMovePanel.powerItem.name;

        //panelDetail.SetActive(true);
        //resultPanelDetail.SetActive(false);

        enemyStackImage.gameObject.SetActive(false);
        enemyStackDiceRect.gameObject.SetActive(false);

        InitCostList();
        InitResultList();
        InitOthers();

        gameObject.SetActive(true);
    }

    public void InitCostList()
    {
        for (int i = 1; i < powerCostList.childCount; i++)
        {
            Destroy(powerCostList.GetChild(i).gameObject);
        }

        foreach (var cost in pm.costList)
        {
            ItemCard itemCard = Instantiate(costItem, powerCostList);
            itemCard.InitDate(cost.count.ToString(), GlobalResources.instance.items[cost.id]);
            itemCard.gameObject.SetActive(true);
        }

        if (pm.coin > 0)
        {
            ItemCard itemCard = Instantiate(costItem, powerCostList);
            itemCard.InitDate(pm.coin.ToString(), GlobalResources.instance.coin);
            itemCard.gameObject.SetActive(true);
        }

        if (pm.light > 0)
        {
            ItemCard itemCard = Instantiate(costItem, powerCostList);
            itemCard.InitDate(pm.light.ToString(), GlobalResources.instance.lightImage);
            itemCard.gameObject.SetActive(true);
        }

        if (pm.range > 0)
        {
            ItemCard itemCard = Instantiate(costItem, powerCostList);
            itemCard.InitDate(pm.range.ToString(), GlobalResources.instance.range);
            itemCard.gameObject.SetActive(true);
        }

    }

    public void InitResultList()
    {
        for (int i = 1; i < resultList.childCount; i++)
        {
            Destroy(resultList.GetChild(i).gameObject);
        }

        for (int i = 1; i < panelResutlList.childCount; i++) 
        { 
            Destroy(panelResutlList.GetChild(i).gameObject);
        }

        PowerMoveResult result = pm.result;
        
        if (result == null) return;

        if (result.coin > 0) 
        {
            ItemCard itemCard = Instantiate(resultItem, resultList);
            itemCard.InitDate(result.coin.ToString(), GlobalResources.instance.coin);
            itemCard.gameObject.SetActive(true);

            itemCard = Instantiate(panelResultItem, panelResutlList);
            itemCard.InitDate(result.coin.ToString(), GlobalResources.instance.coin);
            itemCard.gameObject.SetActive(true);

        }

        if (result.energy > 0)
        {
            ItemCard itemCard = Instantiate(resultItem, resultList);
            itemCard.InitDate(result.energy.ToString(), GlobalResources.instance.energy);
            itemCard.gameObject.SetActive(true);

            itemCard = Instantiate(panelResultItem, panelResutlList);
            itemCard.InitDate(result.energy.ToString(), GlobalResources.instance.energy);
            itemCard.gameObject.SetActive(true);
        }

        if (result.ultimate > 0) 
        {
            ItemCard itemCard = Instantiate(resultItem, resultList);
            itemCard.InitDate(result.ultimate.ToString(), GlobalResources.instance.ultimate);
            itemCard.gameObject.SetActive(true);

            itemCard = Instantiate(panelResultItem, panelResutlList);
            itemCard.InitDate(result.ultimate.ToString(), GlobalResources.instance.ultimate);
            itemCard.gameObject.SetActive(true);
        }

        if (result.stacks != null)
        {
            foreach (var stack in result.stacks)
            {
                ItemCard itemCard = Instantiate(costItem, resultList);
                itemCard.InitDate(stack.count.ToString(), GlobalResources.instance.stacks[stack.id]);
                itemCard.gameObject.SetActive(true);

                itemCard = Instantiate(panelResultItem, panelResutlList);
                itemCard.InitDate(stack.count.ToString(), GlobalResources.instance.stacks[stack.id]);
                itemCard.gameObject.SetActive(true);

                addStackImage.sprite = GlobalResources.instance.stacks[stack.id];
            }
        }

        if (result.items != null)
        {
            foreach (var item in result.items)
            {
                ItemCard itemCard = Instantiate(costItem, resultList);
                itemCard.InitDate(item.count.ToString(), GlobalResources.instance.items[item.id]);
                itemCard.gameObject.SetActive(true);

                itemCard = Instantiate(panelResultItem, panelResutlList);
                itemCard.InitDate(item.count.ToString(), GlobalResources.instance.items[item.id]);
                itemCard.gameObject.SetActive(true);
            }
        }

    }

    public void InitOthers()
    {
        CharacterState state = UIGameManager.instance.controller.State;
        _healthBar.SetRangedValueState(state.stats.health);
        _energyBar.SetRangedValueState(state.stats.energy);
        _ultimateBar.SetRangedValueState(state.stats.ultimate);

        state.items.ForEach(item =>
        {
            ITEM type = (ITEM)item.id;
            if (type == ITEM.Melee)
            {
                meleeText.text = item.count.ToString();
                item.OnCountChange((short newCount, short oldCount) =>
                {
                    meleeText.text = newCount.ToString();
                });
            }
            else if (type == ITEM.Mana)
            {
                manaText.text = item.count.ToString();
                item.OnCountChange((short newCount, short oldCount) =>
                {
                    manaText.text = newCount.ToString();
                });
            }
        });
    }

    public void OnConfirmBtnClicked()
    {
        UFB.Events.EventBus.Publish(
            RoomSendMessageEvent.Create(
                "setPowerMove",
                new RequestSetPowerMoveItem
                {
                    characterId = UIGameManager.instance.controller.Id,
                    powerMoveId = pm.id,
                }
            )
        );

        //gameObject.SetActive(false);

        panelDetail.SetActive(false);
        resultPanelDetail.SetActive(true);
    }

    public void OnSelectDice()
    {
        UFB.Events.EventBus.Publish(
            RoomSendMessageEvent.Create(
                GlobalDefine.CLIENT_MESSAGE.SET_DICE_ROLL,
                new RequestSetDiceRoll
                {
                    characterId = UIGameManager.instance.controller.Id,
                    powerMoveId = pm.id,
                }
            )
        );
        //DiceArea.instance.LaunchDice();
    }

    private EnemyDiceRollMessage enemyMessage;
    public void OnEnemyStackDiceRoll(EnemyDiceRollMessage e)
    {
        enemyMessage = e;
        powermoveImage.gameObject.SetActive(false);
        diceRect.gameObject.SetActive(false);

        enemyStackImage.sprite = GlobalResources.instance.stacks[e.stackId];
        enemyStackImage.gameObject.SetActive(true);

        DiceArea.instance.SetDiceType(DICE_TYPE.DICE_4);
        StartCoroutine(LanchEnemyDiceRoll(e.enemyDiceCount));
        
        enemyStackDiceRect.gameObject.SetActive(true);
    }

    IEnumerator LanchEnemyDiceRoll(int diceCount)
    {
        yield return new WaitForSeconds(1.5f);
        DiceData[] data = new DiceData[1];
        data[0] = new DiceData();
        data[0].type = DICE_TYPE.DICE_4;
        data[0].diceCount = diceCount;
        enemyStackImage.gameObject.SetActive(false);
        DiceArea.instance.LaunchDice(data, true);
    }

    public void OnLanuchDiceRoll(SetDiceRollMessage message)
    {
        DiceArea.instance.LaunchDice(message.diceData);
    }

    public void InitDiceData()
    {
        if(pm.result.dice > 0)
        {
            diceRect.SetActive(true);
            DiceArea.instance.SetDiceType((DICE_TYPE) pm.result.dice);
        } 
        else
        {
            diceRect.SetActive(false);
        }

    }

    public void OnFinishDice()
    {
        powermoveImage.gameObject.SetActive(false);
        int diceCount = DiceArea.instance.diceResultCount;
        Debug.Log($"dice count finish{diceCount}" );
        UFB.Events.EventBus.Publish(
            RoomSendMessageEvent.Create(
                GlobalDefine.CLIENT_MESSAGE.SET_POWER_MOVE_ITEM,
                new RequestSetPowerMoveItem
                {
                    enemyId = HighlightRect.Instance.selectedMonster == null? "" : HighlightRect.Instance.selectedMonster.Id,
                    characterId = UIGameManager.instance.controller.Id,
                    powerMoveId = pm.id,
                    diceCount = diceCount
                }
            )
        );
    }

    public void OnFinishEnemy()
    {
        enemyStackImage.gameObject.SetActive(false);
        int diceCount = DiceArea.instance.diceResultCount;
        UFB.Events.EventBus.Publish(
            RoomSendMessageEvent.Create(
                GlobalDefine.CLIENT_MESSAGE.END_POWER_MOVE_ITEM,
                new RequestEndPowerMoveItem
                {
                    enemyId = HighlightRect.Instance.selectedMonster == null ? "" : HighlightRect.Instance.selectedMonster.Id,
                    characterId = UIGameManager.instance.controller.Id,
                    powerMoveId = pm.id,
                    diceCount = enemyMessage.diceCount,
                    enemyDiceCount = enemyMessage.enemyDiceCount
                }
            )
        );
    }

    IEnumerator EndAttackPanel()
    {
        yield return new WaitForSeconds(1f);
        OnCancelBtnClicked();
    }

    public void OnCancelBtnClicked()
    {
        gameObject.SetActive(false);
    }
}
