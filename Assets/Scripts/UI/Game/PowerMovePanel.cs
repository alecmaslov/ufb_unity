using System.Collections;
using System.Collections.Generic;
using UFB.StateSchema;
using UFB.UI;
using UnityEngine;
using UnityEngine.UI;

public class PowerMovePanel : MonoBehaviour
{
    public static PowerMovePanel instance;

    [SerializeField]
    PowerMoveItem item;

    [SerializeField]
    Image powerImage;

    [SerializeField]
    Text powerNameText;

    [SerializeField]
    Text powerLevelText;

    public EquipSlot slot;

    public Item powerItem;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void Init(Item item, EquipSlot slt)
    {
        slot = slt;
        powerItem = item;
        powerImage.sprite = GlobalResources.instance.powers[item.id];
        powerNameText.text = item.name;
        powerLevelText.text = $"LEVEL {item.level}";
        gameObject.SetActive(true);

    }

    public void UnEquipPower()
    {
        slot.ResetImage();
        EquipPanel.instance.OnInitEquipView();
        gameObject.SetActive(false);
    }


}
