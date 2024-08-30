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

    public void OnPunchBtn()
    {
        UIGameManager.instance.punchPanel.InitData();
    }

    public void OnConfirmBtn()
    {
        gameObject.SetActive(false);
    }

    public void OnManaBtn()
    {

    }

    public void OnCharacterStateChanged(CharacterState e)
    {
        CharacterState state = UIGameManager.instance.controller.State;

        state.items.ForEach(item =>
        {
            ITEM type = (ITEM) item.id;

            if(type == ITEM.Mana)
            {
                
            }

        });
    }
}
