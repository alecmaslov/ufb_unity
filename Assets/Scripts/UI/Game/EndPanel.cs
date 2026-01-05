using System.Collections;
using System.Collections.Generic;
using UFB.Character;
using UFB.Items;
using UFB.StateSchema;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndPanel : MonoBehaviour
{
    public Text desText;
    public Image panelImage;
    
    public Text totalGoldText;
    public EndResultItem endResultItem;
    public Transform endResultPanel;

    public Sprite[] sprites;

    private int totalGold = 0;
    
    public void InitData(END_TYPE type)
    {
        if (type == END_TYPE.DEFEAT) 
        { 
            panelImage.sprite = sprites[1];
            desText.text = "DEFEAT";
        } 
        else
        {
            panelImage.sprite = sprites[0];
            desText.text = "VICTORY";
        }

        totalGold = CharacterManager.Instance.PlayerCharacter.State.stats.coin;
        
        for (var i = 1; i < endResultPanel.childCount; i++)
        {
            Destroy(endResultPanel.GetChild(i).gameObject);
        }
        CharacterManager.Instance.PlayerCharacter.State.powers.ForEach(AddEndResultPower);
        CharacterManager.Instance.PlayerCharacter.State.items.ForEach(AddEndResultItem);
        CharacterManager.Instance.PlayerCharacter.State.stacks.ForEach(AddEndResultStack);
        
        
        totalGoldText.text = totalGold.ToString();
        
        gameObject.SetActive(true);
    }

    private void AddEndResultPower(Item data)
    {
        var item = Instantiate(endResultItem, endResultPanel);

        item.InitData(GlobalResources.instance.powers[data.id], data.sell.ToString());
        
        item.gameObject.SetActive(true);
        
        totalGold += data.sell;
    }
    
    private void AddEndResultItem(Item data)
    {
        if(data.id is (int)ITEM.Mana or (int) ITEM.Melee) return;
        
        var item = Instantiate(endResultItem, endResultPanel);

        item.InitData(GlobalResources.instance.items[data.id], data.sell.ToString());
        
        item.gameObject.SetActive(true);
        
        totalGold += data.sell;
    }
    
    private void AddEndResultStack(Item data)
    {
        var item = Instantiate(endResultItem, endResultPanel);

        item.InitData(GlobalResources.instance.stacks[data.id], data.sell.ToString());
        
        item.gameObject.SetActive(true);
        
        totalGold += data.sell;
    }
    
    public void OnEndScene()
    {
        SceneManager.LoadScene(GlobalDefine.MAIN_SCENE);
    }
}
