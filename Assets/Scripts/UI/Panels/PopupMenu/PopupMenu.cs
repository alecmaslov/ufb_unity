using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UFB.Events;

namespace UFB.UI
{
    public enum PopupMenuType
    {
        Screenspace,
        Worldspace
    }

    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class PopupMenu : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _menuTitle;

        [SerializeField]
        private GameObject _buttonPrefab;

        [SerializeField]
        private Transform _buttonContainer;

        [SerializeField] protected RectTransform _panel;

        private PopupMenuEvent _event;

        public virtual void Initialize(PopupMenuEvent popupMenuEvent)
        {
            SetTitle(popupMenuEvent.title);
            foreach (var button in popupMenuEvent.buttons)
            {
                CreateButton(button.text, button.onClick);
            }
            CreateButton("Cancel", () => {
                Destroy(gameObject);
                popupMenuEvent.onCancel();
            });
            transform.position = popupMenuEvent.target.position + popupMenuEvent.positionOffset;
            _event = popupMenuEvent;
        }


        public void SetTitle(string text)
        {
            _menuTitle.text = text;
        }


        public void CreateButton(string text, Action onClick)
        {
            var button = Instantiate(_buttonPrefab, _buttonContainer);
            button.GetComponentInChildren<TextMeshProUGUI>().text = text;
            button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => onClick());
        }
    }
}
