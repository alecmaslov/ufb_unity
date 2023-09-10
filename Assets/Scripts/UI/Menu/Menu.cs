using UnityEngine;
using System.Collections.Generic;

namespace UFB.UI
{
    public abstract class Menu : MonoBehaviour
    {

        protected MenuManager _menuManager;

        public virtual void Start()
        {
            _menuManager = GetComponentInParent<MenuManager>();
            if (_menuManager == null)
            {
                Debug.LogError("MenuManager not found in parent", gameObject);
            }
        }

        public virtual void OpenMenu()
        {
            gameObject.SetActive(true);
        }

        public virtual void CloseMenu()
        {
            gameObject.SetActive(false);
        }
    }
}