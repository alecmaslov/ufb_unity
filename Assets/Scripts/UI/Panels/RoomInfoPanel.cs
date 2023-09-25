using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UFB.Gameplay;
using UFB.Core;
using UFB.Network;

namespace UFB.UI
{
    public class RoomInfoPanel : MonoBehaviour
    {
        public TextMeshProUGUI roomIdText;

        public void Awake()
        {
            // roomIdText.text = GameManager.Instance.NetworkManager.Room.RoomId;
            roomIdText.text = ServiceLocator.Current.Get<GameService>().Room.RoomId;

            // GameManager.Instance.OnGameLoaded += () =>
            // {
            // };
        }


    }
}

