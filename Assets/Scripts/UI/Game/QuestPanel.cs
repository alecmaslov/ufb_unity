using Colyseus.Schema;
using System.Collections;
using System.Collections.Generic;
using UFB.Events;
using UFB.Items;
using UFB.Network.RoomMessageTypes;
using UFB.StateSchema;
using UnityEngine;
using UnityEngine.UI;

public class QuestPanel : MonoBehaviour
{
    public Text title;

    public Transform questList;
    public QuestItem questItem;

    public void OnQuestClicked(Quest quest)
    {
        EventBus.Publish(
            RoomSendMessageEvent.Create(
                "setActiveQuest",
                new RequestActiveQuest
                {
                    characterId = UIGameManager.instance.controller.Id,
                    quest = quest,
                }
            )
        );
        ClearList();
        StartCoroutine(ActiveList());
    }

    public void InitData()
    {
        title.text = "CHOOSE A QUEST";

        ClearList();
        Quest[] quests = UIGameManager.instance.merchantPanel.questData;
        if (quests == null) return;
        foreach (var quest in quests)
        {
            QuestItem q = Instantiate(questItem, questList);
            q.InitDate(quest);
            q.GetComponent<Button>().onClick.AddListener(() => {
                OnQuestClicked(quest);
            });
            q.gameObject.SetActive(true);
        }
    }

    public void InitActiveData()
    {
        ClearList();
        ArraySchema<Quest> quests = UIGameManager.instance.controller.State.quests;
        if (quests == null) return;

        title.text = "ACTIVE QUESTS";

        quests.ForEach(quest =>
        {
            QuestItem q = Instantiate(questItem, questList);
            q.InitDate(quest);
            /*q.GetComponent<Button>().onClick.AddListener(() => {
                OnQuestClicked(quest);
            });*/
            q.gameObject.SetActive(true);
        });
    
    }

    public void ClearList()
    {
        for (int i = 1; i < questList.childCount; i++)
        {
            Destroy(questList.GetChild(i).gameObject);
        }
    }

    IEnumerator ActiveList()
    {
        yield return new WaitForSeconds(1f);
        InitActiveData();
    }
}
