using Colyseus.Schema;
using System.Collections;
using System.Collections.Generic;
using UFB.Network.RoomMessageTypes;
using UnityEngine;
using UnityEngine.UI;

public class AddStackScore : AddExtraScore
{
    public Image stackImage;

    public void OnReceiveMessageData(AddExtraScoreMessage message)
    {
        base.OnReceiveExtraScore(message);
        if(message.stackId > -1)
        {
            stackImage.sprite = GlobalResources.instance.stacks[message.stackId];
        }
    }
}
