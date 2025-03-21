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
    public Text[] textBtns;

    public Image meleeHighLight;
    public Image manaHighLight;
    
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

        /*foreach (var item in punchBtns)
        {
            item.SetActive(isPunch);
        }*/
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
                textBtns[0].text = item.count.ToString();
            }
            else if(type == ITEM.Melee)
            {
                textBtns[1].text = item.count.ToString();
            }

            item.OnCountChange((short newCount, short oldCount) =>
            {
                if (type == ITEM.Mana)
                {
                    textBtns[0].text = item.count.ToString();
                }
                else if (type == ITEM.Melee)
                {
                    textBtns[1].text = item.count.ToString();
                }
            });

        });
    }

    public void SetHighLightBtn(bool isDefault = false)
    {
        int meleeCount = UIGameManager.instance.GetItemCount(ITEM.Melee);
        int manaCount = UIGameManager.instance.GetItemCount(ITEM.Mana);

        if (isDefault)
        {
            meleeHighLight.color = new Color(1, 1, 1, 0);
            manaHighLight.color = new Color(1, 1, 1, 0);
        }
        else
        {
            meleeHighLight.color = meleeCount == 0 ? Color.red : Color.yellow;
            manaHighLight.color = manaCount == 0 ? Color.red : Color.yellow;
        }
    }
}
