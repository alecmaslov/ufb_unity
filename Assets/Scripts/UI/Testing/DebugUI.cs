using System.Collections;
using System.Collections.Generic;
using UFB.Gameplay;
using UnityEngine;
using UnityEngine.UI;


public class DebugUI : MonoBehaviour
{
    public Button exit;

    public Slider xCoord;

    public Slider yCoord;

    public Button move;

    // Start is called before the first frame update
    void Start()
    {
        exit.onClick.AddListener(() => {
            GameManager.Instance.LeaveGame();
        });

        move.onClick.AddListener(() => {
            // GameManager.Instance.RoomClient.MoveMyPlayer(new UFB.Map.Coordinates((int)xCoord.value, (int)yCoord.value));
        });
    }
    
}
