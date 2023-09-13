using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UFB.Gameplay;

namespace UFB.UI
{
    public class RoomInfoPanel : MonoBehaviour
    {
        public TextMeshProUGUI roomIdText;

        public void Awake()
        {
            // ask, don't tell
            GameManager.Instance.OnGameLoaded += () =>
            {
                roomIdText.text = GameManager.Instance.NetworkManager.Room.RoomId;
            };
        }


    }
}

