using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnPanel : MonoBehaviour
{
    public Text turnTimeText;
    public Text topTurnTimeText;

    public Text desText;

    public void InitData(float turnTime)
    {
        turnTimeText.text = Utility.GetTurnTimeString(turnTime);
        topTurnTimeText.text = Utility.GetTurnTimeString(turnTime);
                
        desText.text = UIGameManager.instance.isPlayerTurn? $"Your Turn!" : $"Monster Turn!";
        
        gameObject.SetActive(true);
    }

    public void SetTurnTime(float turnTime)
    {
        turnTimeText.text = Utility.GetTurnTimeString(turnTime);
        topTurnTimeText.text = Utility.GetTurnTimeString(turnTime);
    }
}
