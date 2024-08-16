using System.Collections;
using System.Collections.Generic;
using UFB.Character;
using UFB.Network.RoomMessageTypes;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class TargetScreenPanel : MonoBehaviour
{
    [HideInInspector]
    public PowerMoveItem powerMoveItem;

    [HideInInspector]
    public PowerMove pm;

    
    public Text powermoveText;
    public Image powermoveImage;
    public Transform powerCostList;
    public Transform resultList;
    public ItemCard costItem;
    public ItemCard resultItem;
    public Image targetImage;


    public void InitData(PowerMoveItem _power)
    {
        powerMoveItem = _power;
        pm = _power.pm;
        powermoveImage.sprite = GlobalResources.instance.powers[pm.powerImageId];
        powermoveText.text = pm.name.ToString();
        InitCostList();
        InitResultList();

        gameObject.SetActive(true);
        UIGameManager.instance.bottomDrawer.SetActive(false);
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
        }
        else
        {
            targetImage.gameObject.SetActive(false);
        }
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


        PowerMoveResult result = pm.result;

        if (result == null) return;

        if (result.coin > 0)
        {
            ItemCard itemCard = Instantiate(resultItem, resultList);
            itemCard.InitDate(result.coin.ToString(), GlobalResources.instance.coin);
            itemCard.gameObject.SetActive(true);
        }

        if (result.energy > 0)
        {
            ItemCard itemCard = Instantiate(resultItem, resultList);
            itemCard.InitDate(result.energy.ToString(), GlobalResources.instance.energy);
            itemCard.gameObject.SetActive(true);
        }

        if (result.ultimate > 0)
        {
            ItemCard itemCard = Instantiate(resultItem, resultList);
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
            }
        }

        if (result.items != null)
        {
            foreach (var item in result.items)
            {
                ItemCard itemCard = Instantiate(costItem, resultList);
                itemCard.InitDate(item.count.ToString(), GlobalResources.instance.items[item.id]);
                itemCard.gameObject.SetActive(true);
            }
        }

    }

    public void OnTapOtherEnemy()
    {
        HighlightRect.Instance.OnSetOtherMonster();
        if(HighlightRect.Instance.selectedMonster != null)
        {
            CameraManager.instance.SetEnemyTarget(HighlightRect.Instance.selectedMonster.transform);
        }

    }

    public void OnPowerMoveBtn()
    {
        if (powerMoveItem != null) 
        { 
            UIGameManager.instance.attackPanel.Init(powerMoveItem);
        }
    }

    public void OnClosePanel()
    {
        gameObject.SetActive(false);
        UIGameManager.instance.bottomDrawer.SetActive(true);
        HighlightRect.Instance.ClearHighLightRect();
        CameraManager.instance.SetEnemyTarget(UIGameManager.instance.controller.transform);
        UIGameManager.instance.powerMovePanel.gameObject.SetActive(true);

    }
}