using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UFB.Gameplay;


namespace UFB.UI
{
    public class ConnectionStatus : MonoBehaviour
    {
        public TextMeshProUGUI text;
        public Image statusImage;

        void Start()
        {
            statusImage.color = Color.red;
            text.text = "Not Connected";

            GameController.Instance.OnConnect += () => {
                statusImage.color = Color.green;
                text.text = "Connected";
            };
        }
    }

}
