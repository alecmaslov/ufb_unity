using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UFB.Items;
using UFB.Network.RoomMessageTypes;
using UnityEngine;
using UnityEngine.UI;

public class ItemResultPanel : MonoBehaviour
{
    public Transform resultList;
    public ItemCard resultItem;
    
    public List<ResultItem> itemList = new List<ResultItem>();
    public List<ResultItem> stackList = new List<ResultItem>();
    public List<ResultItem> powerList = new List<ResultItem>();
    public int coin;
    
    public GameObject bottomPanel;
    public Image heartBtn;
    public Text heartText;
    public Image crystalBtn;
    public Text crystalText;
    public GameObject heartResultPart;
    public GameObject crystalResultPart;
    
    public void InitPanel( List<ResultItem> items, List<ResultItem> stacks, List<ResultItem> powers, int _coin )
    {
        itemList.Clear();
        stackList.Clear();
        powerList.Clear();
        ClearResultList();
        
        itemList = items;
        stackList = stacks;
        powerList = powers;
        coin = _coin;

        bottomPanel.SetActive(false);
        
        gameObject.SetActive(true);
        
        StartCoroutine(CheckResult());
    }

    public void InitResult(bool isOld = true)
    {
        if (itemList.Count > 0)
        {
            foreach (var result in itemList)
            {
                ResultItem item = GetResultItemFromId(result.id);
                
                int count = UIGameManager.instance.GetItemCount((ITEM) item.id);
                if(isOld) count -= item.count;
                if ((ITEM)item.id == ITEM.HeartPiece || (ITEM)item.id == ITEM.EnergyShard)
                {
                    bottomPanel.SetActive(true);
                    UIGameManager.instance.bottomDrawer.OpenBottomDrawer();
                    heartBtn.sprite = GlobalResources.instance.divideTo4[count % 5];
                    crystalBtn.sprite = GlobalResources.instance.divideTo3[count % 4];
                    
                    int heartCount = UIGameManager.instance.GetItemCount(ITEM.HeartCrystal);
                    heartText.text = heartCount.ToString();
                    
                    int crysticalCount = UIGameManager.instance.GetItemCount(ITEM.EnergyCrystal);
                    crystalText.text = crysticalCount.ToString();
                    
                    crystalResultPart.SetActive((ITEM)item.id == ITEM.EnergyShard && crysticalCount % 3 == 0);
                    heartResultPart.SetActive((ITEM)item.id == ITEM.HeartPiece && heartCount % 4 == 0);
                }
                else
                {
                    bottomPanel.SetActive(false);
                    AddResultItem(count, GlobalResources.instance.items[item.id], true, isOld? new Color(0.67f,0.67f,0.67f) : (item.count > 0? Color.green : Color.red));
                }
            }
        }
        
        if (powerList.Count > 0)
        {
            foreach (var item in powerList)
            {
                int count = UIGameManager.instance.GetPowerCount((POWER) item.id);
                if(isOld) count -= item.count;
                AddResultItem(count, GlobalResources.instance.powers[item.id], true, isOld? new Color(0.67f,0.67f,0.67f) : (item.count > 0 ? Color.green : Color.red));
            }
        }
        
        if (stackList.Count > 0)
        {
            foreach (var item in stackList)
            {
                int count = UIGameManager.instance.GetStackCount((STACK) item.id);
                if(isOld) count -= item.count;
                AddResultItem(count, GlobalResources.instance.stacks[item.id], true, isOld? new Color(0.67f,0.67f,0.67f) : (item.count > 0 ? Color.green : Color.red));
            }
        }

        if (coin != 0)
        {
            int count = UIGameManager.instance.controller.State.stats.coin;
            if(isOld) count -= coin;
            AddResultItem(count, GlobalResources.instance.coin, true, isOld? new Color(0.67f,0.67f,0.67f) : (coin > 0 ? Color.green : Color.red));
        }
    }
    
    IEnumerator CheckResult()
    {
        yield return new WaitForSeconds(0.5f);
        InitResult();
        yield return new WaitForSeconds(0.5f);
        ClearResultList();
        InitResult(false);
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }

    public void ClearResultList()
    {
        for (int i = 1; i < resultList.childCount; i++)
        {
            Destroy(resultList.GetChild(i).gameObject);
        }
    }
    
    public void AddResultItem(int count, Sprite sprite, bool isText = true, Color? color = null)
    {
        ItemCard itemCard = Instantiate(resultItem, resultList);
        itemCard.InitText(count.ToString());
        itemCard.InitImage(sprite);
        if (itemCard.countText != null)
        {
            itemCard.countText.gameObject.SetActive(isText);
            if(itemCard.countText.transform.parent != null)
                itemCard.countText.transform.parent.gameObject.SetActive(isText);
        }

        if (color != null)
        {
            itemCard.InitTextBG(color.Value);
        }
        
        itemCard.gameObject.SetActive(true);
    }

    public ResultItem GetResultItemFromId(int type)
    {
        ITEM id = (ITEM)type;
        ITEM realId = id;
        int count = 1;
        
        if(id == ITEM.FlameChili2) {
            realId = ITEM.FlameChili;
            count = 2;
        } else if(id == ITEM.FlameChili3) {
            realId = ITEM.FlameChili;
            count = 3;
        } else if(id == ITEM.IceTea2) {
            realId = ITEM.IceTea;
            count = 2;
        } else if(id == ITEM.IceTea3) {
            realId = ITEM.IceTea;
            count = 3;
        } else if(id == ITEM.HeartPiece2) {
            realId = ITEM.HeartPiece;
            count = 2;
        } else if(id == ITEM.Potion2) {
            realId = ITEM.Potion;
            count = 2;
        } else if(id == ITEM.Potion3) {
            realId = ITEM.Potion;
            count = 3;
        } else if(id == ITEM.Feather2) {
            realId = ITEM.Feather;
            count = 2;
        } else if(id == ITEM.Feather3) {
            realId = ITEM.Feather;
            count = 3;
        } else if(id == ITEM.Arrow2) {
            realId = ITEM.Arrow;
            count = 2;
        } else if(id == ITEM.Arrow3) {
            realId = ITEM.Arrow;
            count = 3;
        } else if(id == ITEM.Bomb2) {
            realId = ITEM.Bomb;
            count = 2;
        } else if(id == ITEM.Bomb3) {
            realId = ITEM.Bomb;
            count = 3;
        } else if(id == ITEM.Melee2) {
            realId = ITEM.Melee;
            count = 2;
        } else if(id == ITEM.Mana2) {
            realId = ITEM.Mana;
            count = 2;
        } else if(id == ITEM.Quiver2) {
            realId = ITEM.Quiver;
            count = 2;
        } else if(id == ITEM.BombBag2) {
            realId = ITEM.BombBag;
            count = 2;
        } else if(id == ITEM.WarpCrystal2) {
            realId = ITEM.WarpCrystal;
            count = 2;
        } else if(id == ITEM.Elixir2) {
            realId = ITEM.Elixir;
            count = 2;
        } else if(id == ITEM.BombArrow2) {
            realId = ITEM.BombArrow;
            count = 2;
        } else if(id == ITEM.FireArrow2) {
            realId = ITEM.FireArrow;
            count = 2;
        } else if(id == ITEM.IceArrow2) {
            realId = ITEM.IceArrow;
            count = 2;
        } else if(id == ITEM.VoidArrow2) {
            realId = ITEM.VoidArrow;
            count = 2;
        } else if(id == ITEM.FireBomb2) {
            realId = ITEM.FireBomb;
            count = 2;
        } else if(id == ITEM.IceBomb2) {
            realId = ITEM.IceBomb;
            count = 2;
        } else if(id == ITEM.VoidBomb2) {
            realId = ITEM.VoidBomb;
            count = 2;
        } else if(id == ITEM.caltropBomb2) {
            realId = ITEM.caltropBomb;
            count = 2;
        }

        return new ResultItem
        {
            id = (int)realId,
            count = count
        };
    }
    
}
