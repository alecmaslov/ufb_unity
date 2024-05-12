using System.Collections;
using System.Collections.Generic;
using UFB.Character;
using UFB.Core;
using UFB.StateSchema;
using UFB.UI;
using UnityEngine;
using UnityEngine.UI;

public class EquipPanel : MonoBehaviour
{
    public static EquipPanel instance;

    [SerializeField]
    Transform scrollView;

    [SerializeField]
    EquipItem item;

    [SerializeField]
    List<EquipSlot> equipItems;

    [SerializeField]
    PowerMovePanel powerMovePanel;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void OnInitEquipView()
    {
        if(gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);
        
    }

    public void InitEquipList(CharacterState state)
    {
        state.OnChange(() =>
        {
            InitScrollView();

            state.powers.ForEach(power =>
            {
                if (power != null && power.count > 0)
                {
                    EquipItem go = Instantiate(item, scrollView);
                    go.Init(GlobalResources.instance.powers[power.id], power.name, $"LEVEL {power.level}", $"-{power.cost}");
                    go.GetComponent<Button>().onClick.AddListener(() => OnClickEquip(power));
                    go.gameObject.SetActive(true);
                }
            });
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
        powerMovePanel.Init(equip, equipItems[0]);

        equipItems[0].Init(equip);

        gameObject.SetActive(false);
    }
}
