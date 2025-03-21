using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace UFB.UI
{
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager Instance;
        public Menu[] Menus => GetComponentsInChildren<Menu>();
        public Stack<Menu> menuStack = new Stack<Menu>();
        public Menu initialMenu;

        private Dictionary<string, object> _menuData = new Dictionary<string, object>();

        private void Start()
        {
            Instance = this;
        }

        // public void
        public void SetMenuData(string key, object value)
        {
            _menuData[key] = value;
        }

        // public void SetMenuData<T>(Action<T> setAction)
        // {
        //     var key = typeof(T).Name;
        //     if (_menuData.ContainsKey(key))
        //     {
        //         setAction((T)_menuData[key]);
        //     }
        //     else
        //     {
        //         _menuData[key] = Activator.CreateInstance<T>();
        //         setAction((T)_menuData[key]);
        //     }
        // }

        public object GetMenuData(string key)
        {
            if (_menuData.ContainsKey(key))
            {
                return _menuData[key];
            }
            return null;
        }

        public void OnEnable()
        {
            if (initialMenu != null)
            {
                menuStack.Clear();
                foreach (var menu in Menus)
                {
                    menu.gameObject.SetActive(false);
                }
                //OpenMenu(initialMenu);
            }
        }

        public void OpenMenu(string menuName)
        {
            Menu menuToOpen = Menus.FirstOrDefault(menu => menu.gameObject.name == menuName);
            
            if (menuToOpen == null)
            {
                Debug.LogError($"Menu {menuName} not found", gameObject);
                return;
            }

            OpenMenu(menuToOpen);
        }
        

        public void OpenMenu(Menu menu, bool isSetting = true)
        {
            if (menuStack.Count > 0 && isSetting)
            {
                menuStack.Peek().CloseMenu();
            }

            menu.OpenMenu();
            menuStack.Push(menu);
        }

        public void CloseMenu()
        {
            // Remove the current menu
            if (menuStack.Count == 0)
            {
                return;
            }

            Menu topMenu = menuStack.Pop();
            topMenu.CloseMenu();

            // Enable the previous menu
            if (menuStack.Count > 0)
            {
                menuStack.Peek().OpenMenu();
            }
        }
    }
}