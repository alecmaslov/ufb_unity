using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomSlotPanel : MonoBehaviour
{
    public Transform powerPanel;
    public Image powerItem;


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

    }

    public void OnQuestPanel()
    {

    }
}
