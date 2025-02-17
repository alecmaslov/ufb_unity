using System.Collections;
using System.Collections.Generic;
using UFB.Character;
using UFB.Items;
using UFB.Network.RoomMessageTypes;
using UFB.StateSchema;
using UFB.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class BottomDefeatPanel : MonoBehaviour
{
    public Image powermoveImage;

    public Image avatarImage;

    [HideInInspector]
    public PowerMove pm;

    public GameObject diceRect;
    // ENEMY DICE
    public Image enemyStackImage;
    public GameObject enemyStackDiceRect;
    public Image addStackImage;
    public GameObject addedStackPart;

    public int diceTimes = 0;

    public int totalDiceCount = 0;

    public Image moveImage;

    public GameObject redPanelImage;

    public void Init(PowerMove _powermove, CharacterState origin, CharacterState target)
    {
        UIGameManager.instance.bottomDrawer.OpenBottomDrawer();

        //InitEnemyState(target);
        InitCharacterState(origin);

        pm = _powermove;
        InitDiceData();

        enemyStackImage.gameObject.SetActive(false);
        enemyStackImage.transform.parent.gameObject.SetActive(false);
        enemyStackDiceRect.SetActive(false);

        if (pm.result.stacks != null)
        {
            foreach (var stack in pm.result.stacks)
            {
                addStackImage.sprite = GlobalResources.instance.stacks[stack.id];
                addedStackPart.SetActive(true);
                StartCoroutine(ResetAddedStackPart());
            }
        }

        totalDiceCount = 0;
        diceTimes = 0;
        gameObject.SetActive(true);
        redPanelImage.SetActive(true);
    }

    public void InitCharacterState(CharacterState state)
    {
        Addressables
        .LoadAssetAsync<UfbCharacter>("UfbCharacter/" + state.characterClass)
        .Completed += (op) =>
        {
            if (op.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                avatarImage.sprite = op.Result.avatar;
            }
            else
                Debug.LogError("Failed to load character avatar: " + op.OperationException.Message);
        };
    }

    public void InitEnemyState(CharacterState target)
    {
        Addressables
        .LoadAssetAsync<UfbCharacter>("UfbCharacter/" + target.characterClass)
        .Completed += (op) =>
        {
            if (op.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                avatarImage.sprite = op.Result.avatar;
            }
            else
                Debug.LogError("Failed to load character avatar: " + op.OperationException.Message);
        };

    }


    public bool isVampired = false;
    public void InitDiceData()
    {
        isVampired = false;
        if (pm.result.dice > 0 && diceTimes == 0)
        {
            diceRect.SetActive(true);
            DiceArea.instance.SetDiceType((DICE_TYPE)pm.result.dice, false, true);
            powermoveImage.sprite = pm.id < 0 ? GlobalResources.instance.punch : GlobalResources.instance.powers[pm.powerImageId];
            moveImage.sprite = pm.id < 0 ? GlobalResources.instance.punch : GlobalResources.instance.powers[pm.powerImageId];
        }
        else if (pm.result.perkId == (int)PERK.VAMPIRE)
        {
            isVampired = true;
            diceRect.SetActive(true);
            DiceArea.instance.SetDiceType(DICE_TYPE.DICE_6_4, false, true);
            powermoveImage.sprite = GlobalResources.instance.perks[(int)PERK.VAMPIRE];
            moveImage.sprite = GlobalResources.instance.perks[(int)PERK.VAMPIRE];

        }
        else
        {
            powermoveImage.sprite = GlobalResources.instance.powers[(int)pm.powerImageId];
            moveImage.sprite = GlobalResources.instance.powers[(int)pm.powerImageId];
            diceRect.SetActive(false);
        }
        powermoveImage.gameObject.SetActive(true);

    }

    public void OnClosePanel()
    {
        enemyStackImage.gameObject.SetActive(false);
        enemyStackImage.transform.parent.gameObject.SetActive(false);
        redPanelImage.SetActive(false);
        gameObject.SetActive(false);

        if (UIGameManager.instance.bottomDrawer.IsExpanded)
        {
            UIGameManager.instance.bottomDrawer.CloseBottomDrawer();
            HighlightRect.Instance.ClearHighLightRect();
        }

    }

    public void OnCancelBtnClicked()
    {
        gameObject.SetActive(false);
    }

    private EnemyDiceRollMessage enemyMessage;

    public void OnEnemyStackDiceRoll(EnemyDiceRollMessage e)
    {
        enemyMessage = e;
        powermoveImage.gameObject.SetActive(false);
        diceRect.SetActive(false);

        enemyStackImage.sprite = GlobalResources.instance.stacks[e.stackId];
        enemyStackImage.gameObject.SetActive(true);
        enemyStackImage.transform.parent.gameObject.SetActive(true);

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
        enemyStackImage.transform.parent.gameObject.SetActive(false);
        DiceArea.instance.LaunchDice(data, true);
    }

    public void OnLanuchDiceRoll(SetDiceRollMessage message)
    {
        //Debug.Log(message.diceData);
        DiceArea.instance.LaunchDice(message.diceData);
        powermoveImage.gameObject.SetActive(false);
    }

    IEnumerator ResetAddedStackPart()
    {
        yield return new WaitForSeconds(2f);
        addedStackPart.SetActive(false);
    }
}
