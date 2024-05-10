using System.Collections;
using System.Collections.Generic;
using UFB.Character;
using UFB.Core;
using UFB.StateSchema;
using UnityEngine;
using UnityEngine.UI;

public class EquipPanel : MonoBehaviour
{
    [SerializeField]
    Transform scrollView;

    [SerializeField]
    EquipItem item;

    public void OnInitEquipView()
    {
        gameObject.SetActive(true);
        
        InitScrollView();
        
        CharacterState state = ServiceLocator.Current.Get<CharacterManager>().PlayerCharacter.State;

        state.powers.ForEach(power =>
        {
            if (power != null && power.count > 0)
            {
                EquipItem go = Instantiate(item);
                go.Init(GlobalResources.instance.powers[power.id], power.name, $"LEVEL {power.level}", $"-{power.cost}");
                go.GetComponent<Button>().onClick.AddListener(() => OnClickEquip(power));
            }
        });
    }

    private void InitScrollView()
    {
        for (int i = 1; i < scrollView.childCount; i++)
        {
            Destroy(scrollView.GetChild(i).gameObject);
        }
    }

    public void OnClickEquip(Item equip)
    {
        Debug.Log($"equip logic system..");
    }
}
