using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;
using System.Collections.Generic;

namespace UFB.UI
{
    [CreateAssetMenu(fileName = "UIComponentList", menuName = "UI/Components/List")]
    public class ListUIComponent : UIComponent
    {
        public List<UIComponent> components;

        public override VisualElement Render()
        {
            VisualElement root = new VisualElement();
            foreach (var component in components)
            {
                root.Add(component.Render());
            }
            return root;
        }
    }
}