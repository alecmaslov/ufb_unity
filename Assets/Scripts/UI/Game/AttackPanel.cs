using System.Collections;
using System.Collections.Generic;
using UFB.Character;
using UFB.Events;
using UFB.Items;
using UFB.Network.RoomMessageTypes;
using UFB.StateSchema;
using UFB.UI;
using UI.ThreeDimensional;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AttackPanel : MonoBehaviour
{
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

    public int diceTimes = 0;

    public int totalDiceCount = 0;
    
    public UIObject3D bottomCharacter3D;
    public UIObject3D topCharacter3D;

    public bool isEndAttack = false;

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
            
            topCharacter3D.ObjectPrefab = HighlightRect.Instance.selectedMonster.transform;

            Debug.Log("character class: " + HighlightRect.Instance.selectedMonster.State.characterClass);

            if (HighlightRect.Instance.selectedMonster.State.characterClass.Contains("Earwig"))
            {
                Debug.Log("character class: is there" + HighlightRect.Instance.selectedMonster.State.characterClass);

                topCharacter3D.TargetOffset = new Vector2(-0.1f, -0.3f);
            }
            else
            {
                Debug.Log("character class: is not there" + HighlightRect.Instance.selectedMonster.State.characterClass);

                topCharacter3D.TargetOffset = new Vector2(-0.1f, 0);
            }

        } 
        else
        {

        }
    }

    public void Init(PowerMove _powermove)
    {
        InitEnemyState();
        pm = _powermove;
        diceTimes = 0;
        InitDiceData();

        //powermoveText.text = pm.name.ToString();
        //powerText.text = UIGameManager.instance.powerMovePanel.powerItem.name;

        //panelDetail.SetActive(true);
        //resultPanelDetail.SetActive(false);

        enemyStackImage.gameObject.SetActive(false);
        enemyStackDiceRect.SetActive(false);

        //InitCostList();
        //InitResultList();

        if (pm.result.stacks != null)
        {
            foreach (var stack in pm.result.stacks)
            {
                addStackImage.sprite = GlobalResources.instance.stacks[stack.id];
            }
        }

        InitOthers();
        totalDiceCount = 0;
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
        _healthBar.SetRangedValueState(state.stats.health, state);
        _energyBar.SetRangedValueState(state.stats.energy, state);
        _ultimateBar.SetRangedValueState(state.stats.ultimate, state);

        UFB.Character.CharacterController obj = UIGameManager.instance.controller;
        Debug.Log(obj);
        if (obj != null)
        {
            bottomCharacter3D.ObjectPrefab = obj.transform;
        }

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
        powermoveImage.gameObject.SetActive(false);
        UFB.Events.EventBus.Publish(
            RoomSendMessageEvent.Create(
                GlobalDefine.CLIENT_MESSAGE.SET_POWER_MOVE_ITEM,
                new RequestSetPowerMoveItem
                {
                    enemyId = HighlightRect.Instance.selectedMonster == null ? "" : HighlightRect.Instance.selectedMonster.Id,
                    characterId = UIGameManager.instance.controller.Id,
                    powerMoveId = pm.id,
                    extraItemId = pm.extraItemId,
                }
            )
        );
    }

    public void OnSelectDice()
    {
        diceTimes++;
        UFB.Events.EventBus.Publish(
            RoomSendMessageEvent.Create(
                GlobalDefine.CLIENT_MESSAGE.SET_DICE_ROLL,
                new RequestSetDiceRoll
                {
                    characterId = UIGameManager.instance.controller.Id,
                    powerMoveId = pm.id,
                    extraItemId = pm.extraItemId,
                    diceTimes = diceTimes,
                }
            )
        );
        powermoveImage.gameObject.SetActive(false);
        //DiceArea.instance.LaunchDice();
    }

    private EnemyDiceRollMessage enemyMessage;
    public void OnEnemyStackDiceRoll(EnemyDiceRollMessage e)
    {
        enemyMessage = e;
        powermoveImage.gameObject.SetActive(false);
        diceRect.SetActive(false);

        enemyStackImage.sprite = GlobalResources.instance.stacks[e.stackId];
        enemyStackImage.gameObject.SetActive(true);

        DiceArea.instance.SetDiceType(DICE_TYPE.DICE_4, true);
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
    public bool isVampired = false;
    public void InitDiceData()
    {
        isEndAttack = false;

        isVampired = false;
        if (pm.result.dice > 0 && diceTimes == 0)
        {
            diceRect.SetActive(true);
            DiceArea.instance.SetDiceType((DICE_TYPE) pm.result.dice);
            powermoveImage.sprite = pm.id < 0 ? GlobalResources.instance.punch : GlobalResources.instance.powers[pm.powerImageId];
        }
        else if(pm.result.perkId == (int) PERK.VAMPIRE)
        {
            isVampired = true;
            diceRect.SetActive(true);
            DiceArea.instance.SetDiceType(DICE_TYPE.DICE_6_4);
            powermoveImage.sprite = GlobalResources.instance.perks[(int) PERK.VAMPIRE];
        }
        else
        {
            powermoveImage.sprite = GlobalResources.instance.powers[(int) pm.powerImageId];
            diceRect.SetActive(false);
        }
        powermoveImage.gameObject.SetActive(true);

    }

    public void OnFinishDice()
    {
        int vampireCount = 0;
        if(isVampired)
        {
            totalDiceCount += DiceArea.instance.diceData[0].diceCount;
            vampireCount = DiceArea.instance.diceData[1].diceCount;
        }
        else
        {
            totalDiceCount += DiceArea.instance.diceResultCount;
        }

        Debug.Log($"dice times: {diceTimes}, {pm.getDiceTime()}");

        if (diceTimes < pm.getDiceTime()) 
        {
            InitDiceData();
            return;
        }
        
        Debug.Log($"dice count finish{totalDiceCount}" );
        UFB.Events.EventBus.Publish(
            RoomSendMessageEvent.Create(
                GlobalDefine.CLIENT_MESSAGE.SET_POWER_MOVE_ITEM,
                new RequestSetPowerMoveItem
                {
                    enemyId = HighlightRect.Instance.selectedMonster == null? "" : HighlightRect.Instance.selectedMonster.Id,
                    characterId = UIGameManager.instance.controller.Id,
                    powerMoveId = pm.id,
                    diceCount = totalDiceCount,
                    vampireCount = vampireCount,
                    extraItemId = pm.extraItemId,
                }
            )
        );
        isEndAttack = true;

        StartCoroutine(EndAttackPanel());
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
                    enemyDiceCount = enemyMessage.enemyDiceCount,
                    stackId = enemyMessage.stackId,
                    extraItemId = pm.extraItemId,
                }
            )
        );

        isEndAttack = true;
        StartCoroutine(EndAttackPanel());

    }

    IEnumerator EndAttackPanel()
    {
        yield return new WaitForSeconds(3f);
        if(isEndAttack)
        {
            OnCancelBtnClicked();
        }
    }

    public void OnCancelBtnClicked()
    {
        gameObject.SetActive(false);
        if( UIGameManager.instance.punchPanel.gameObject.activeSelf)
        {
            UIGameManager.instance.punchPanel.InitArrow();
        }
    }
}
