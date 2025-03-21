using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UFB.Character;
using UFB.Core;
using UFB.Events;
using UFB.Items;
using UFB.Map;
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
    public GameObject punchPart;
    public GameObject powerListPart;
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
    public GameObject addedStackPart;

    public int diceTimes = 0;

    public int totalDiceCount = 0;

    public bool isEndAttack = false;

    public Text stackCountText;

    public Transform powermoveListPanel;
    public PowerMoveItem listItemPrefab;

    public void Init(CharacterState state)
    {
        target = state;
        monsterInfo.OnSelectedCharacterEvent(state);
        InitMonsterInfo();

        idx = 0;
    }

    void InitMonsterInfo()
    {
        detailPart.SetActive(false);
        powerListPart.SetActive(false);
        monsterInfo.gameObject.SetActive(true);
        gameObject.SetActive(true);

        enemyDiceRect.SetActive(false);
        playerDiceRect.SetActive(false);
        powermoveImage.gameObject.SetActive(false);
        powermoveImage.transform.parent.gameObject.SetActive(false);
        punchPart.SetActive(false);
    }

    public void InitPowermove(Item item, EquipSlot slt, PowerMove[] _moves)
    {
        detailPart.SetActive(false);
        powerListPart.SetActive(true);
        punchPart.SetActive(false);
        monsterInfo.gameObject.SetActive(false);
        moves = _moves;
        // ResetPowermove();
        
        InitPowermoveList();
    }

    void InitPowermoveList()
    {
        for (int i = 1; i < powermoveListPanel.childCount; i++)
        {
            Destroy(powermoveListPanel.GetChild(i).gameObject);
        }

        foreach (var item in moves)
        {
            var p = Instantiate(listItemPrefab, powermoveListPanel);
            p.Init(item);
            p.gameObject.SetActive(true);
        }
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

    public void OnClickPowermoveItem(PowerMove pm)
    {
        powerListPart.SetActive(false);
        selectedPowermove = pm;
        powerMoveItem.Init(selectedPowermove);
        powerMoveItem.InitResultList();
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
        punchPart.SetActive(true);
        detailPart.SetActive(false);
        powerListPart.SetActive(false);
        monsterInfo.gameObject.SetActive(false);

        UIGameManager.instance.StepPanel.SetHighLightBtn();
        
        EventBus.Publish(
            RoomSendMessageEvent.Create(
                GlobalDefine.CLIENT_MESSAGE.GET_HIGHLIGHT_RECT,
                new RequestGetHighlightRect
                {
                    characterId = CharacterManager.Instance.SelectedCharacter.Id,
                    powerMoveId = -1
                }
            )
        );

    }

    public void OnPunchBtn(int type)
    {
        if (punchPart.activeSelf)
        {
            // CHECK CONDITION
            int count = 0;
            if (type == 1) // MELEE
            {
                count = UIGameManager.instance.GetItemCount(ITEM.Melee);
            }
            else // MANA
            {
                count = UIGameManager.instance.GetItemCount(ITEM.Mana);
            }

            if (count == 0)
            {
                UIGameManager.instance.OnNotificationMessage("error", "You don't have enough Item to do that!");
                return;
            }
            
            PowerMove pm = new PowerMove();
            if (type == 0)
            {
                pm.id = -1;
            }
            else if (type == 1)
            {
                pm.id = -100;
            }
            
            pm.name = "";
            pm.powerImageId = 33;
            pm.powerIds = new int[1];

            Item item = new Item();
            item.name = "";
            item.count = 1;

            if (type == 0)
            {
                item.id = (int)ITEM.Melee;
            }
            else if (type == 1)
            {
                item.id = (int)ITEM.Mana;
            }

            pm.result = new PowerMoveResult();

            /*if (arrow != null)
            {
                pm.costList = new Item[2] {
                    item,
                    arrow,
                };

                ResultItem resultItem = new ResultItem();
                if (arrow.id == (int)ITEM.FireArrow)
                {
                    resultItem.id = (int)STACK.Burn;
                    resultItem.count = 1;
                    pm.result.stacks = new ResultItem[1] { resultItem };
                }
                else if (arrow.id == (int)ITEM.BombArrow)
                {
                    pm.result.perkId = (int)PERK.PULL;
                }
                else if (arrow.id == (int)ITEM.IceArrow)
                {
                    resultItem.id = (int)STACK.Freeze;
                    resultItem.count = 1;
                    pm.result.stacks = new ResultItem[1] { resultItem };
                }
                else if (arrow.id == (int)ITEM.VoidArrow)
                {
                    resultItem.id = (int)STACK.Void;
                    resultItem.count = 1;
                    pm.result.stacks = new ResultItem[1] { resultItem };
                }

                pm.id -= arrow.id;
                Debug.Log(pm.id);

            }
            else
            {*/
            pm.costList = new Item[1] {
                item
            };
            /*}*/

            pm.result.dice = (int)DICE_TYPE.DICE_4;
            pm.light = 2;
            pm.coin = 0;
            pm.range = 1;

            selectedPowermove = pm;

            ConfirmAttack();

        }
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

        if (selectedPowermove.result.stacks != null)
        {
            foreach (var stack in selectedPowermove.result.stacks)
            {
                addStackImage.sprite = GlobalResources.instance.stacks[stack.id];
                addedStackPart.SetActive(true);
                StartCoroutine(ResetAddedStackPart());
            }
        }

        totalDiceCount = 0;
        gameObject.SetActive(true);
        detailPart.SetActive(false); 
        punchPart.SetActive(false);
        monsterInfo.gameObject.SetActive(true);
    }

    public void CancelAttack()
    {
        //gameObject.SetActive(false);
        if (UIGameManager.instance.bottomDrawer.IsExpanded)
        {
            UIGameManager.instance.equipPanel.ClearHighLightItems();
            UIGameManager.instance.bottomDrawer.CloseBottomDrawer();
            HighlightRect.Instance.ClearHighLightRect();
        }


        InitMonsterInfo();
        if (UIGameManager.instance.punchPanel.gameObject.activeSelf)
        {
            UIGameManager.instance.punchPanel.InitArrow();
        }
    }

    public void CancelPunch()
    {
        InitMonsterInfo();
        selectedPowermove = null;
        UIGameManager.instance.StepPanel.SetHighLightBtn(true);
    }

    public void CancelPowerMoveDetail()
    {
        InitMonsterInfo();
    }

    public void CancelPowerList()
    {
        UIGameManager.instance.equipPanel.ClearHighLightItems();
        powerListPart.SetActive(false);
        detailPart.SetActive(false);
        monsterInfo.gameObject.SetActive(true);
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
        Debug.Log( "enemy dice attack start:::" );
        enemyMessage = e;
        //powermoveImage.gameObject.SetActive(false);
        playerDiceRect.SetActive(false);

        enemyStackImage.sprite = GlobalResources.instance.stacks[e.stackId];
        enemyStackImage.gameObject.SetActive(true);
        enemyStackImage.transform.parent.gameObject.SetActive(true);

        stackCountText.text = target.stacks[e.stackId].count.ToString();

        DiceArea.instance.SetDiceType(DICE_TYPE.DICE_4, false, true);
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
        enemyStackImage.transform.parent.gameObject.SetActive(false);
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
            StartCoroutine(OnStartDiceRect(0.1f));
        }
        else if (selectedPowermove.result.perkId == (int)PERK.VAMPIRE)
        {
            isVampired = true;
            playerDiceRect.SetActive(true);
            DiceArea.instance.SetDiceType(DICE_TYPE.DICE_6_4);
            powermoveImage.sprite = GlobalResources.instance.perks[(int)PERK.VAMPIRE];
            StartCoroutine(OnStartDiceRect(0.1f));
        }
        else
        {
            powermoveImage.sprite = GlobalResources.instance.powers[(int)selectedPowermove.powerImageId];
            playerDiceRect.SetActive(false);
        }
        powermoveImage.gameObject.SetActive(true);
        powermoveImage.transform.parent.gameObject.SetActive(false);

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

    public int GetRange(CharacterState state)
    {
        if (target == null)
        {
            return 0;
        }
        
        Tile CurrentTile = ServiceLocator.Current.Get<GameBoard>().Tiles[state.currentTileId];
        Tile TargetTile = ServiceLocator.Current.Get<GameBoard>().Tiles[target.currentTileId];

        return Mathf.Abs(CurrentTile.Coordinates.X - TargetTile.Coordinates.X) + 
               Mathf.Abs(CurrentTile.Coordinates.Y - TargetTile.Coordinates.Y);
    }
    
    public void OnFinishEnemy()
    {
        enemyStackImage.gameObject.SetActive(false);
        enemyStackImage.transform.parent.gameObject.SetActive(false);
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
        yield return new WaitForSeconds(5f);
        if (isEndAttack)
        {
            CancelAttack();
        }
    }

    IEnumerator OnStartDiceRect(float delay = 2f)
    {
        yield return new WaitForSeconds(delay);
        OnSelectDice();
    }

    IEnumerator ResetAddedStackPart()
    {
        yield return new WaitForSeconds(2f);
        addedStackPart.SetActive(false);
    }
}
