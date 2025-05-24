using System.Collections;
using System.Collections.Generic;
using UFB.Items;
using UFB.Network.RoomMessageTypes;
using UFB.StateSchema;
using UFB.UI;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBombPanel : MonoBehaviour
{
    public TopHeader monsterInfo;

    public Image bombImage;
    
    public ItemCard resultItem;
    public Transform resultList;

    public ItemCard stackItem;
    public Transform stackList;

    List<ToastBanStackMessage> banStackMessages = new List<ToastBanStackMessage>();

    public void Init(CharacterState state)
    {
        monsterInfo.OnSelectedCharacterEvent(state);
    }
    
    public void InitGetBombItem(GetBombMessage message)
    {
        monsterInfo.gameObject.SetActive(false);
        
        ItemResult result = message.itemResult;
        
        if (message.itemId > -1)
        {
            bombImage.sprite = GlobalResources.instance.items[message.itemId];
            bombImage.gameObject.SetActive(false);
        }
        
        InitBombResultList(result);
        resultList.gameObject.SetActive(false);
        //InitBombList(result);
        
        UIGameManager.instance.bottomDrawer.OpenBottomDrawer();
        //InitBombStack(result);
        gameObject.SetActive(true);

        StartCoroutine(CheckBombStatus(result));
    }
    
    public void InitBanStack(ToastBanStackMessage e)
    {
        Debug.LogError($"init ban stack message: sssss");
        banStackMessages.Add(e);
    }

    void InitBombResultList(ItemResult result, bool isBanStack = false, bool isPerk = false)
    {
        ClearResultList();
        if (result.energy != 0)
        {
            ItemCard itemCard = Instantiate(resultItem, resultList);
            itemCard.InitDate(result.energy.ToString(), GlobalResources.instance.energy);
            itemCard.gameObject.SetActive(true);
        }

        if (result.heart != 0)
        {
            ItemCard itemCard = Instantiate(resultItem, resultList);
            itemCard.InitDate(result.heart.ToString(), GlobalResources.instance.health);
            itemCard.gameObject.SetActive(true);
        }

        if (result.ultimate != 0)
        {
            ItemCard itemCard = Instantiate(resultItem, resultList);
            itemCard.InitDate(result.ultimate.ToString(), GlobalResources.instance.ultimate);
            itemCard.gameObject.SetActive(true);
        }

        if (result.stackId > -1)
        {
            ItemCard itemCard = Instantiate(resultItem, resultList);
            itemCard.InitDate("+1", GlobalResources.instance.stacks[result.stackId]);
            if (isBanStack)
            {
                itemCard.InitBanImage();
            }
            itemCard.gameObject.SetActive(true);
        }

        if(result.perkId >= 0)
        {
            ItemCard itemCard = Instantiate(resultItem, resultList);
            itemCard.InitDate("", GlobalResources.instance.perks[result.perkId]);
            if (isPerk)
            {
                itemCard.InitBanImage();
            }
            itemCard.gameObject.SetActive(true);
        }
        
        if(result.powerId >= 0)
        {
            ItemCard itemCard = Instantiate(resultItem, resultList);
            itemCard.InitDate("+1", GlobalResources.instance.powers[result.powerId]);
            itemCard.gameObject.SetActive(true);
        }
    }

    void InitBombStack(ItemResult result, bool isOld = true)
    {
        bool isBanStack = false;
        banStackMessages.ForEach(msg =>
        {
            if (result.stackId == msg.stack1)
            {
                isBanStack = true;
                int banStackId = msg.stack2;
                int count = UIGameManager.instance.GetStackCount((STACK) banStackId);
                AddResultItem(isOld ? count + 1 : count, GlobalResources.instance.stacks[banStackId], true, isOld? Color.gray : new Color(0.735849f, 0, 0));
                InitBombResultList(result, true);
            }
        });
        
        if (!isBanStack)
        {
            if (result.stackId > -1)
            {
                int count = UIGameManager.instance.GetStackCount((STACK) result.stackId);
                AddResultItem(isOld ? Mathf.Max(0, count - 1) : count , GlobalResources.instance.stacks[result.stackId], true, isOld? Color.gray : Color.green);
            }
        }
    }
    
    void ClearStackList()
    {
        for (int i = 1; i < stackList.childCount; i++)
        {
            Destroy(stackList.GetChild(i).gameObject);
        }
    }

    void ClearResultList()
    {
        for (int i = 1; i < resultList.childCount; i++)
        {
            Destroy(resultList.GetChild(i).gameObject);
        }
    }
    
    public void AddResultItem(int count, Sprite sprite, bool isText = true, Color? color = null)
    {
        ItemCard itemCard = Instantiate(stackItem, stackList);
        itemCard.InitText(count.ToString());
        itemCard.InitImage(sprite);
        if(itemCard.countText != null)
            itemCard.countText.gameObject.SetActive(isText);

        if (color != null)
        {
            itemCard.InitTextBG(color.Value);
        }
        
        itemCard.gameObject.SetActive(true);
    }
    
    IEnumerator CheckBombStatus(ItemResult result)
    {
        ClearStackList();
        yield return new WaitForSeconds(1f);
        bombImage.gameObject.SetActive(true);
        resultList.gameObject.SetActive(true);
        InitBombStack(result);
        
        yield return new WaitForSeconds(1f);
        ClearStackList();
        InitBombStack(result, false);
        
        yield return new WaitForSeconds(1f);
        ClearStackList();
        ClearResultList();
        banStackMessages.Clear();
        
        monsterInfo.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(1f);
        bombImage.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
    
    
}
