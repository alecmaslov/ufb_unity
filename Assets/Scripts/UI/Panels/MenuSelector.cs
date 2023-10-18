using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UFB.UI
{
    public abstract class MenuSelector<T> : MonoBehaviour
    {
        [SerializeField]
        private Image _displayContentImage;

        [SerializeField]
        private List<T> _selectionOptions = new List<T>();
        public T CurrentSelection
        {
            get => _currentSelection;
            protected set
            {
                _currentSelection = value;
                OnSelectionChanged?.Invoke(value);
            }
        }

        private T _currentSelection;

        public delegate void OnSelectionChange(T selection);
        public event OnSelectionChange OnSelectionChanged;

        public abstract void OnClickImage();

        private void Awake()
        {
            if (_selectionOptions.Count > 0)
            {
                CurrentSelection = _selectionOptions[0];
            }
            else
            {
                Debug.LogWarning(
                    "MenuSelector has no selection options, assign them in the inspector"
                );
            }
        }

        public void SetDisplayContent(Sprite sprite)
        {
            _displayContentImage.sprite = sprite;
        }

        public virtual void OnClickNext()
        {
            int index = _selectionOptions.IndexOf(CurrentSelection);
            index = (index + 1) % _selectionOptions.Count;
            CurrentSelection = _selectionOptions[index];
        }

        public virtual void OnClickLast()
        {
            int index = _selectionOptions.IndexOf(CurrentSelection);
            index -= 1;
            if (index < 0)
            {
                index = _selectionOptions.Count - 1;
            }
            CurrentSelection = _selectionOptions[index];
        }
    }
}
