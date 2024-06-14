using System.Collections;
using System.Collections.Generic;
using UFB.Events;
using UFB.Items;
using UFB.StateSchema;
using UFB.UI;
using UnityEngine;
using UnityEngine.UI;

public class AttackPanel : MonoBehaviour
{
    [HideInInspector]
    public PowerMoveItem powerMoveItem;

    [SerializeField]
    private LinearIndicatorBar _healthBar;

    [SerializeField]
    private LinearIndicatorBar _energyBar;

    [SerializeField]
    private LinearIndicatorBar _ultimateBar;

    [SerializeField]
    Text meleeText;

    [SerializeField]
    Text manaText;

    [SerializeField]
    Image itemImage;


    public void InitCharacterState(ChangeCharacterStateEvent e)
    {
        _healthBar.SetRangedValueState(e.state.stats.health);
        _energyBar.SetRangedValueState(e.state.stats.energy);
        _ultimateBar.SetRangedValueState(e.state.stats.ultimate);

        e.state.items.ForEach(item =>
        {
            ITEM type = (ITEM)item.id;
            if (type == ITEM.Melee)
            {
                meleeText.text = item.count.ToString();
            }
            else if (type == ITEM.Mana)
            {
                manaText.text = item.count.ToString();
            }
        });

    }

    public void Init(PowerMoveItem _power)
    {
        powerMoveItem = _power;

        gameObject.SetActive(true);
    }
}
