using System.Collections;
using System.Collections.Generic;
using UFB.Character;
using UFB.Events;
using UFB.Items;
using UFB.Network.RoomMessageTypes;
using UFB.StateSchema;
using UFB.UI;
using UI.ThreeDimensional;
using UnityEngine;
using UnityEngine.UI;

public class BottomAttackPanel : MonoBehaviour
{
    public TopHeader monsterInfo;
    public Text monsterNameText;

    public GameObject detailPart;
    public PowerMoveItem powerMoveItem;

    public int idx = 0;

    public PowerMove[] moves;

    public PowerMove selectedPowermove;

    public CharacterState target;

    public Image powermoveImage;

    // ENEMY DICE
    public Image enemyStackImage;
    public GameObject enemyDiceRect;
    public GameObject playerDiceRect;

    public Image addStackImage;

    public int diceTimes = 0;

    public int totalDiceCount = 0;

    public bool isEndAttack = false;

    public void Init(CharacterState state)
    {
        target = state;
        detailPart.SetActive(false);
        monsterInfo.gameObject.SetActive(true);
        monsterInfo.OnSelectedCharacterEvent(state);
        gameObject.SetActive(true);

        enemyDiceRect.SetActive(false);
        playerDiceRect.SetActive(false);
        powermoveImage.gameObject.SetActive(false);

        idx = 0;
    }

    public void InitPowermove(Item item, EquipSlot slt, PowerMove[] _moves)
    {
        detailPart.SetActive(true);
        monsterInfo.gameObject.SetActive(false);

        moves = _moves;
        ResetPowermove();
    }

    public void ResetPowermove()
    {
        selectedPowermove = moves[idx];

        powerMoveItem.Init(selectedPowermove);
        powerMoveItem.gameObject.SetActive(true);
        EventBus.Publish(
            RoomSendMessageEvent.Create(
                GlobalDefine.CLIENT_MESSAGE.GET_HIGHLIGHT_RECT,
                new RequestGetHighlightRect
                {
                    characterId = CharacterManager.Instance.SelectedCharacter.Id,
                    powerMoveId = selectedPowermove.id
                }
            )
        );
    }

    public void OnNextPowermove()
    {
        idx++;
        if (idx >= moves.Length)
        {
            idx = 0;
        }
        ResetPowermove();
    }

    public void InitPunch()
    {

    }

    public void ConfirmAttack()
    {
        InitAttack();
    }

    public void InitAttack()
    {
        diceTimes = 0;
        InitDiceData();

        enemyStackImage.gameObject.SetActive(false);
        enemyDiceRect.SetActive(false);

        totalDiceCount = 0;
        gameObject.SetActive(true);
    }

    public void CancelAttack()
    {
        gameObject.SetActive(false);
        if (UIGameManager.instance.punchPanel.gameObject.activeSelf)
        {
            UIGameManager.instance.punchPanel.InitArrow();
        }
    }

    public void OnSelectDice()
    {
        if (selectedPowermove != null && target != null)
        {
            diceTimes++;
            EventBus.Publish(
                RoomSendMessageEvent.Create(
                    GlobalDefine.CLIENT_MESSAGE.SET_DICE_ROLL,
                    new RequestSetDiceRoll
                    {
                        characterId = target.id,
                        powerMoveId = selectedPowermove.id,
                        extraItemId = selectedPowermove.extraItemId,
                        diceTimes = diceTimes,
                    }
                )
            );
        }
        //powermoveImage.gameObject.SetActive(false);
        //DiceArea.instance.LaunchDice();
    }

    private EnemyDiceRollMessage enemyMessage;
    public void OnEnemyStackDiceRoll(EnemyDiceRollMessage e)
    {
        enemyMessage = e;
        //powermoveImage.gameObject.SetActive(false);
        playerDiceRect.SetActive(false);

        enemyStackImage.sprite = GlobalResources.instance.stacks[e.stackId];
        enemyStackImage.gameObject.SetActive(true);

        DiceArea.instance.SetDiceType(DICE_TYPE.DICE_4, true);
        StartCoroutine(LanchEnemyDiceRoll(e.enemyDiceCount));

        enemyDiceRect.SetActive(true);
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
        if (selectedPowermove.result.dice > 0 && diceTimes == 0)
        {
            playerDiceRect.SetActive(true);
            DiceArea.instance.SetDiceType((DICE_TYPE)selectedPowermove.result.dice);
            powermoveImage.sprite = selectedPowermove.id < 0 ? GlobalResources.instance.punch : GlobalResources.instance.powers[selectedPowermove.powerImageId];
        }
        else if (selectedPowermove.result.perkId == (int)PERK.VAMPIRE)
        {
            isVampired = true;
            playerDiceRect.SetActive(true);
            DiceArea.instance.SetDiceType(DICE_TYPE.DICE_6_4);
            powermoveImage.sprite = GlobalResources.instance.perks[(int)PERK.VAMPIRE];
        }
        else
        {
            powermoveImage.sprite = GlobalResources.instance.powers[(int)selectedPowermove.powerImageId];
            playerDiceRect.SetActive(false);
        }
        powermoveImage.gameObject.SetActive(true);

    }

    public void OnFinishDice()
    {
        int vampireCount = 0;
        if (isVampired)
        {
            totalDiceCount += DiceArea.instance.diceData[0].diceCount;
            vampireCount = DiceArea.instance.diceData[1].diceCount;
        }
        else
        {
            totalDiceCount += DiceArea.instance.diceResultCount;
        }

        Debug.Log($"dice times: {diceTimes}, {selectedPowermove.getDiceTime()}");

        if (diceTimes < selectedPowermove.getDiceTime())
        {
            InitDiceData();
            return;
        }

        Debug.Log($"dice count finish{totalDiceCount}");
        EventBus.Publish(
            RoomSendMessageEvent.Create(
                GlobalDefine.CLIENT_MESSAGE.SET_POWER_MOVE_ITEM,
                new RequestSetPowerMoveItem
                {
                    enemyId = HighlightRect.Instance.selectedMonster == null ? "" : HighlightRect.Instance.selectedMonster.Id,
                    characterId = UIGameManager.instance.controller.Id,
                    powerMoveId = selectedPowermove.id,
                    diceCount = totalDiceCount,
                    vampireCount = vampireCount,
                    extraItemId = selectedPowermove.extraItemId,
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
        EventBus.Publish(
            RoomSendMessageEvent.Create(
                GlobalDefine.CLIENT_MESSAGE.END_POWER_MOVE_ITEM,
                new RequestEndPowerMoveItem
                {
                    enemyId = HighlightRect.Instance.selectedMonster == null ? "" : HighlightRect.Instance.selectedMonster.Id,
                    characterId = UIGameManager.instance.controller.Id,
                    powerMoveId = selectedPowermove.id,
                    diceCount = enemyMessage.diceCount,
                    enemyDiceCount = enemyMessage.enemyDiceCount,
                    stackId = enemyMessage.stackId,
                    extraItemId = selectedPowermove.extraItemId,
                }
            )
        );

        isEndAttack = true;
        StartCoroutine(EndAttackPanel());

    }

    IEnumerator EndAttackPanel()
    {
        yield return new WaitForSeconds(3f);
        if (isEndAttack)
        {
            CancelAttack();
        }
    }

}
