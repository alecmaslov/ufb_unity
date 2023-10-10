using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace UFB.UI
{
    public class EntityPopupMenu : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _entityNameText;

        [SerializeField]
        private GameObject _buttonPrefab;

        [SerializeField]
        private Transform _buttonContainer;

        private GameObject _entity;

        public void Initialize(GameObject entity)
        {
            _entity = entity;
            _entityNameText.text = entity.name;
        }

        public void CreateButton(string text, Action onClick)
        {
            var button = Instantiate(_buttonPrefab, _buttonContainer);
            button.GetComponentInChildren<TextMeshProUGUI>().text = text;
            button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => onClick());
        }
    }
}
