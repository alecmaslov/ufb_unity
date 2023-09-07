using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UFB.UI
{
    public class TextInputDialog : MonoBehaviour
    {
        public TextMeshProUGUI titleText;
        public TMP_InputField inputField;
        public Button submitButton;
        public Button cancelButton;

        public delegate void OnSubmitHandler(string input);
        public delegate void OnCancelHandler();


        public event OnSubmitHandler OnSubmit;
        public event OnCancelHandler OnCancel;


        void Start()
        {
            submitButton.onClick.AddListener(() =>
            {
                OnSubmit?.Invoke(inputField.text);
                Destroy(gameObject);
            });

            cancelButton.onClick.AddListener(() =>
            {
                OnCancel?.Invoke();
                Destroy(gameObject);
            });
        }

        public void Initialize(string title, string input = "")
        {
            titleText.text = title;
            inputField.text = input;
        }
    }

}
