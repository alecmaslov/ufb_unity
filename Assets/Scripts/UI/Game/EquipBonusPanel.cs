using System.Collections;
using System.Collections.Generic;
using UFB.Network.RoomMessageTypes;
using UnityEngine;

public class EquipBonusPanel : MonoBehaviour
{
    public Transform equipPanel;
    public EquipBonusItem equipBonusItem;

    public void InitData(EquipBonus[] bonuses)
    {
        InitList();
        Debug.Log("equip bonus" + bonuses.Length);
        if(bonuses.Length == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        foreach (var item in bonuses)
        {
            Debug.Log("bonus: " + item);
            EquipBonusItem equip = Instantiate(equipBonusItem, equipPanel);
            equip.InitData(item);
        }

        gameObject.SetActive(true);
    }

    void InitList()
    {
        for (int i = 1; i < equipPanel.childCount; i++) 
        {
            Destroy( equipPanel.GetChild(i).gameObject );
        }
    }
}
