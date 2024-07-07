using System.Collections;
using System.Collections.Generic;
using UFB.Items;
using UnityEngine;
using UnityEngine.UI;

public class QuestPanel : MonoBehaviour
{
    public Text title;

    public Transform questList;
    public QuestItem questItem;

    public void OnQuestClicked()
    {
        ClearList();
    }

    public void InitData()
    {

    }

    public void ClearList()
    {
        for (int i = 1; i < questList.childCount; i++)
        {
            Destroy(questList.GetChild(i).gameObject);
        }
    }
}
