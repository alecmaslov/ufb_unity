using System.Collections;
using System.Collections.Generic;
using UFB.StateSchema;
using UnityEngine;
using UnityEngine.UI;

public class QuestItem : MonoBehaviour
{
    public Image questImage;
    public Text questName;
    public Text questDescription;
    public Transform resultListPanel;
    public Text coinText;
    public Image itemImage;
    public Image powerImage;
    public Image meleeImage;
    public Image manaImage;

    public Quest selectedQuest;
    public void InitDate(Quest quest)
    {
        selectedQuest = quest;
        questImage.sprite = GlobalResources.instance.quests[quest.id];
        questName.text = quest.name;
        questDescription.text = $"(Normal) {quest.description}";
        coinText.text = quest.coin.ToString();
        itemImage.sprite = GlobalResources.instance.items[quest.itemId];
        powerImage.sprite = GlobalResources.instance.powers[quest.powerId];
        meleeImage.gameObject.SetActive(quest.melee != 0);
        manaImage.gameObject.SetActive(quest.mana != 0);
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
