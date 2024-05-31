using Colyseus.Schema;
using System.Collections;
using System.Collections.Generic;
using UFB.Events;
using UFB.Items;
using UFB.StateSchema;
using UnityEngine;
using UnityEngine.UI;

public class StepPanel : MonoBehaviour
{
    public static StepPanel instance { get; private set; }

    [SerializeField] 
    MovePanel movePanel;

    [SerializeField]
    GameObject resourcePanel;

    [SerializeField]
    Text meleeText;

    [SerializeField]
    Text manaText;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void InitInstance()
    {
        if (instance == null)
            instance = this;
    }

    public void OnMoveBtn()
    {
        resourcePanel.SetActive(false);
        gameObject.SetActive(false);
        movePanel.Show();
    }

    public void OnHitBtn()
    {

    }

    public void OnConfirmBtn()
    {
        gameObject.SetActive(false);
    }

    public void OnManaBtn()
    {

    }

    public void OnCharacterStateChanged(ChangeCharacterStateEvent e)
    {
        ArraySchema<Item> items = e.state.items;
        items.ForEach(item =>
        {
            ITEM type = (ITEM) item.id;
            if(type == ITEM.Melee)
            {
                meleeText.text = item.count.ToString();
            } 
            else if(type == ITEM.Mana)
            {
                manaText.text = item.count.ToString();
            }

        });
    }
}
