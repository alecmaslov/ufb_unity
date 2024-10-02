using System.Collections;
using System.Collections.Generic;
using UFB.Character;
using UFB.Items;
using UFB.UI;
using UnityEngine;
using UnityEngine.UI;

public class BottomSlotPanel : MonoBehaviour
{
    public Transform powerPanel;
    public Image powerItem;

    public Transform killPanel;
    public KillItemCard killCard;

    public Text killCardText;
    public int killIdx = 0;

    public void OnPowerPanel()
    {
        for(int i = 1; i < powerPanel.childCount; i++)
        {
            Destroy(powerPanel.GetChild(i).gameObject);
        }

        UIGameManager.instance.controller.State.powers.ForEach(power => 
        { 
            Image p = Instantiate(powerItem, powerPanel);
            p.sprite = GlobalResources.instance.powers[power.id];
            p.gameObject.SetActive(true);
        });
    }

    public void OnKillPanel()
    {
        for(int i = 1;i < killPanel.childCount; i++)
        {
            Destroy (killPanel.GetChild(i).gameObject);
        }

        if(killIdx == 0)
        {
            killCardText.text = "Players";
            Dictionary<string, UFB.Character.CharacterController> players = CharacterManager.Instance.GetCharacterList();

            foreach (var item in players)
            {
                if(item.Value.State.type == (int) USER_TYPE.USER)
                {
                    KillItemCard card = Instantiate(killCard, killPanel);
                    card.OnSelectedCharacterEvent(item.Value.State);
                    card.gameObject.SetActive(true);
                }
            }
        }
        else if(killIdx == 1)
        {
            killCardText.text = "Monsters";

            CharacterManager.Instance.monsterKeys.ForEach(key => {
                UFB.Character.CharacterController monster = CharacterManager.Instance.GetCharacterFromId(key);
                KillItemCard card = Instantiate(killCard, killPanel);
                card.OnSelectedCharacterEvent(monster.State);
                card.gameObject.SetActive(true);
            });
        } 
        else
        {
            killCardText.text = "Killed";

        }
    }

    public void OnNextKillBtn()
    {
        killIdx++;
        killIdx = Mathf.Min(killIdx, 2);
        OnKillPanel();
    }

    public void OnPrevKillBtn()
    {
        killIdx--;
        killIdx = Mathf.Max(killIdx, 0);
        OnKillPanel();
    }

    public void OnQuestPanel()
    {

    }
}
