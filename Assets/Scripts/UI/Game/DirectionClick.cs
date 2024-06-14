using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionClick : MonoBehaviour, IClickable
{
    public string dirct = "";
    public void OnClick()
    {
        Debug.Log("Direction Clicked");
        UIGameManager.instance.movePanel.OnDirectionClicked(dirct);
    }

}
