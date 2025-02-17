using System;
using System.Collections;
using System.Collections.Generic;
using UFB.Character;
using UFB.Events;
using UFB.Items;
using UFB.Network.RoomMessageTypes;
using UFB.StateSchema;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class PunchPanel : MonoBehaviour
{
    public Image targetImage;
    public FollowWorld punchBtn;
    
    [Serializable]
    public struct ITEM_DETAIL
    {
        public ITEM type;
        public Text count;
    }

    public ITEM_DETAIL[] itemDetails;

    public ItemCard arrowItem;

    public GameObject arrowText;

    public GameObject detailPanel;

    public GameObject arrowPanel;

    Item arrow;

    public void InitData()
    {
        UIGameManager.instance.bottomDrawer.gameObject.SetActive(false);
        arrow = null;
        InitArrow();

        EventBus.Publish(
            RoomSendMessageEvent.Create(
                GlobalDefine.CLIENT_MESSAGE.GET_HIGHLIGHT_RECT,
                new RequestGetHighlightRect
                {
                    characterId = CharacterManager.Instance.SelectedCharacter.Id,
                    //powerMoveId = move.id
                    powerMoveId = -1
                }
            )
        );
        //UIGameManager.instance.targetScreenPanel.InitData(pm);

        gameObject.SetActive(true);
    }

    public void InitArrow()
    {
        foreach (var item in itemDetails)
        {
            item.count.text = UIGameManager.instance.GetItemCount(item.type).ToString();
        }
    }

    public void OnAddArrowItem(int idx)
    {
        if(idx >= 0)
        {
            arrowText.SetActive(false);
            arrowItem.gameObject.SetActive(true);
            arrowItem.InitImage(GlobalResources.instance.items[(int)itemDetails[idx].type]);
            arrow = new Item();
            arrow.id = (int)itemDetails[idx].type;
            arrow.count = 1;
        }
        else
        {
            arrow = null;
            arrowText.SetActive(true);
            arrowItem.gameObject.SetActive(false);
        }
        arrowPanel.SetActive(false);
    }

    public void OnOpenDetailPanel()
    {
        detailPanel.SetActive(true);
    }

    public void OnOpenArrowPanel()
    {
        arrowPanel.SetActive(true); 
    }

    public void OnTapOtherEnemy()
    {
        HighlightRect.Instance.OnSetOtherMonster();
        if (HighlightRect.Instance.selectedMonster != null)
        {
            CameraManager.instance.SetEnemyTarget(HighlightRect.Instance.selectedMonster.transform);
        }
    }

    public void SetTargetImage()
    {
        if (HighlightRect.Instance.selectedMonster != null)
        {
            Addressables
                .LoadAssetAsync<UfbCharacter>("UfbCharacter/" + HighlightRect.Instance.selectedMonster.State.characterClass)
                .Completed += (op) =>
                {
                    if (
                        op.Status
                        == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded
                    )
                    {
                        targetImage.sprite = op.Result.avatar;
                    }
                    else
                        Debug.LogError(
                            "Failed to load character avatar: " + op.OperationException.Message
                        );
                };
            targetImage.gameObject.SetActive(true);
            punchBtn.lookAt = HighlightRect.Instance.selectedMonster.transform;
            punchBtn.gameObject.SetActive(true);
        }
        else
        {
            targetImage.gameObject.SetActive(false);
            punchBtn.gameObject.SetActive(false);
        }
    }

    public void OnCloseArrowPanel()
    {
        arrowPanel.SetActive(false);
    }

    public void OnCloseDetailPanel()
    {
        detailPanel.SetActive(false);
    }

    public void OnClosePunchPanel()
    {
        UIGameManager.instance.bottomDrawer.gameObject.SetActive(true);
        HighlightRect.Instance.ClearHighLightRect();
        CameraManager.instance.SetEnemyTarget(UIGameManager.instance.controller.transform);
        gameObject.SetActive(false);
    }

    public void OnMeleeBtn()
    {
        PowerMove pm = new PowerMove();
        pm.id = -1;
        pm.name = "";
        pm.powerImageId = 33;
        pm.powerIds = new int[1];

        Item item = new Item();
        item.name = "";
        item.count = 1;
        item.id = (int)ITEM.Melee;
        pm.result = new PowerMoveResult();
        Debug.Log(arrow);
        if (arrow != null)
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
        {
            pm.costList = new Item[1] {
                item
            };
        }

        pm.result.dice = (int)DICE_TYPE.DICE_4;
        pm.light = 2;
        pm.coin = 0;
        pm.range = 1;
        UIGameManager.instance.attackPanel.Init(pm);
    }

    public void OnManaBtn()
    {
        PowerMove pm = new PowerMove();
        pm.id = -100;
        pm.name = "";
        pm.powerImageId = 33;
        pm.powerIds = new int[1];

        Item item = new Item();
        item.name = "";
        item.count = 1;
        item.id = (int)ITEM.Mana;
        pm.result = new PowerMoveResult();

        if (arrow != null)
        {
            pm.costList = new Item[2] {
                item,
                arrow,
            };
            ResultItem resultItem = new ResultItem();
            if (arrow.id == (int) ITEM.FireArrow)
            {
                resultItem.id = (int) STACK.Burn;
                resultItem.count = 1;
                pm.result.stacks = new ResultItem[1] { resultItem };
            }
            else if(arrow.id == (int) ITEM.BombArrow)
            {
                pm.result.perkId = (int) PERK.PULL;
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
        }
        else
        {
            pm.costList = new Item[1] {
                item
            };
        }

        pm.result.dice = (int)DICE_TYPE.DICE_4;
        pm.light = 2;
        pm.coin = 0;
        pm.range = 1;
        UIGameManager.instance.attackPanel.Init(pm);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
