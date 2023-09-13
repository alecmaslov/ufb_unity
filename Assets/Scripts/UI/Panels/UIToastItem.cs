using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UFB.Network;

namespace UFB.UI
{
    public class UIToastItem : MonoBehaviour
    {
        public TextMeshProUGUI toastText;

        public void Initialize(string message)
        {
            toastText.text = message;
            StartCoroutine(ShowToast());
        }

        private IEnumerator ShowToast()
        {
            yield return new WaitForSeconds(2f);
            Destroy(gameObject);
        }
    }

}
