using System.Collections;
using System.Collections.Generic;
using UFB.Core;
using UFB.Network.RoomMessageTypes;
using UnityEngine;
using UnityEngine.UI;

public class AddExtraScore : MonoBehaviour
{
    public string scoreType = "";
    public Sprite[] options;

    public Image bgImage;
    public Text scoreText;
    private Color transColor = new Color(1, 1, 1, 0);
    private float transValue = 1;
    private float delay = 2;

    public void OnReceiveExtraScore(AddExtraScoreMessage message)
    {
        Debug.Log($"message: {message.type}, type: {scoreType}, status: {message.type == scoreType}, score: {message.score}" );
        if (message.type == scoreType)
        {
            Init(message.score);
        }
    }

    public void Init(int score)
    {
        if (score < 0)
        {
            bgImage.sprite = options[0];
        } 
        else
        {
            bgImage.sprite = options[1];
        }

        scoreText.text = score.ToString();

        transColor = new Color(1, 1, 1, 1);
        bgImage.color = transColor;
        scoreText.color = transColor;
        transValue = delay;
    }

    private void Update()
    {
        transValue -= Time.deltaTime;
        if (transValue < 0) 
        { 
            transValue = 0;
            return;
        }
        transColor.a = transValue;
        bgImage.color = transColor;
        scoreText.color = transColor;
    }

}
