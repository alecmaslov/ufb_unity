using Colyseus.Schema;
using System.Collections;
using System.Collections.Generic;
using UFB.Character;
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

    public GameObject[] punchBtns;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void OnEnable()
    {
        CheckPunchStatus();
    }

    public void InitInstance()
    {
        if (instance == null)
            instance = this;
    }

    public void CheckPunchStatus()
    {
        Dictionary<string, UFB.Character.CharacterController> players = CharacterManager.Instance.GetCharacterList();
        UFB.Character.CharacterController selectedPlayer = CharacterManager.Instance.PlayerCharacter;

        bool isPunch = false;
        foreach (var player in players)
        {
            int distance = Mathf.CeilToInt( Mathf.Abs(player.Value.State.coordinates.x - selectedPlayer.State.coordinates.x) + Mathf.Abs(player.Value.State.coordinates.y - selectedPlayer.State.coordinates.y));
            if(distance == 1)
            {
                isPunch = true;
            }
        }

        foreach (var item in punchBtns)
        {
            item.SetActive(isPunch);
        }
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
