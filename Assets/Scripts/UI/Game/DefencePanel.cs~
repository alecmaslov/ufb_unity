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

public class DefencePanel : MonoBehaviour
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
    Image powermoveImage;


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

    public void InitCharacterState(CharacterState state)
    {
        _healthBar.SetRangedValueState(state.stats.health, state);
        _energyBar.SetRangedValueState(state.stats.energy, state);
        _ultimateBar.SetRangedValueState(state.stats.ultimate, state);

        UFB.Character.CharacterController obj = UIGameManager.instance.controller;
        Debug.Log(obj);

        if (obj != null)
        {
            bottomCharacter3D.ObjectPrefab = obj.transform;
            if (obj.State.characterClass.Contains("Earwig"))
            {
                bottomCharacter3D.TargetOffset = new Vector2(-0.1f, -0.3f);
            }
            else
            {
                bottomCharacter3D.TargetOffset = new Vector2(-0.1f, 0);
            }
        }
    }

    public void InitEnemyState(CharacterState target)
    {
        topHeader.OnSelectedCharacterEvent(target);
        for (int i = 1; i < enemyStackList.childCount; i++)
        {
            Destroy(enemyStackList.GetChild(i).gameObject);
        }
        target.stacks.ForEach(stack =>
        {
            if (stack.count > 0)
            {
                ItemCard ic = Instantiate(enemyStackItem, enemyStackList);
                ic.InitDate(stack.count.ToString(), GlobalResources.instance.stacks[stack.id]);
                ic.gameObject.SetActive(true);
            }
        });
        UFB.Character.CharacterController obj = CharacterManager.Instance.GetCharacterFromId(target.id);
        Debug.Log(obj);
        if (obj != null) 
        { 
            topCharacter3D.ObjectPrefab = obj.transform;

            if(obj.State.characterClass.Contains("Earwig"))
            {
                topCharacter3D.TargetOffset = new Vector2(-0.1f, -0.3f);
            }
            else
            {
                topCharacter3D.TargetOffset = new Vector2(-0.1f, 0);
            }

        }
    }

    public void Init(PowerMove _powermove, CharacterState origin, CharacterState target)
    {
        InitEnemyState(target);
        InitCharacterState(origin);

        pm = _powermove;
        InitDiceData();

        enemyStackImage.gameObject.SetActive(false);
        enemyStackDiceRect.SetActive(false);

        if (pm.result.stacks != null)
        {
            foreach (var stack in pm.result.stacks)
            {
                addStackImage.sprite = GlobalResources.instance.stacks[stack.id];
            }
        }

        totalDiceCount = 0;
        diceTimes = 0;
        gameObject.SetActive(true);
    }

    public void OnClosePanel()
    {
        enemyStackImage.gameObject.SetActive(false);
        gameObject.SetActive(false);
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
        
        enemyStackDiceRect.SetActive(true);
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
        Debug.Log(message.diceData);
        DiceArea.instance.LaunchDice(message.diceData);
        powermoveImage.gameObject.SetActive(false);
    }

    public bool isVampired = false;
    public void InitDiceData()
    {
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
