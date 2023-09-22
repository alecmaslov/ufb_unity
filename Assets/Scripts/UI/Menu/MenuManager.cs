using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace UFB.UI
{
    public class MenuManager : MonoBehaviour
    {
        public Menu[] Menus => GetComponentsInChildren<Menu>();
        public Stack<Menu> menuStack = new Stack<Menu>();
        public Menu initialMenu;

        private Dictionary<string, object> _menuData = new Dictionary<string, object>();

        // public void
        public void SetMenuData(string key, object value)
        {
            _menuData[key] = value;
        }

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
                OpenMenu(initialMenu);
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
        

        public void OpenMenu(Menu menu)
        {
            if (menuStack.Count > 0)
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