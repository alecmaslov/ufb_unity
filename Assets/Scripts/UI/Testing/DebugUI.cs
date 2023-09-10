using System.Collections;
using System.Collections.Generic;
using UFB.Gameplay;
using UnityEngine;
using UnityEngine.UI;


public class DebugUI : MonoBehaviour
{
    public Button exit;

    // Start is called before the first frame update
    void Start()
    {
        exit.onClick.AddListener(() => {
            GameController.Instance.RoomClient.LeaveRoom();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
