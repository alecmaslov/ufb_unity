using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UFB.Network;

namespace UFB.UI
{
    public class UIToast : MonoBehaviour
    {
        // public static UIToast Instance { get; private set; }
        public GameObject toastPrefab;

        private void Awake()
        {
            // if (Instance != null)
            // {
            //     Destroy(gameObject);
            //     return;
            // }

            // Instance = this;
        }

        public void ShowToast(string message)
        {
            GameObject toast = Instantiate(toastPrefab, transform);
            toast.GetComponent<UIToastItem>().Initialize(message);
        }

        public void ShowMessage(NotificationMessage message)
        {
            ShowToast(message.message);
        }
    }

}
