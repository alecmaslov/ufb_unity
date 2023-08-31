using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

namespace UFB.UI
{
    [CreateAssetMenu(fileName = "Button", menuName = "UI/Components/Button")]
    public class ButtonUIComponent : UIComponent
    {
        public string buttonText;
        public StyleSheet styleSheet;
        public UnityEvent onClick;
        public UnityEvent onHover;
        public UnityEvent onHoverExit;

        public override VisualElement Render()
        {
            VisualElement buttonElement = treeAsset.CloneTree();

            // Configure the button based on the data in this instance
            Button button = buttonElement.Q<Button>();
            if (button != null)
            {
                button.text = buttonText;
                button.styleSheets.Add(styleSheet);
                if (onClick != null) button.clickable.clicked += () => onClick.Invoke();

                // Similarly, add hooks for onHover and onHoverExit
            }

            return buttonElement;
        }
    }
}


// private void OnEnable()
// {
//     // VisualElement root = component.CloneTree().contentContainer;
//     // component.Instantiate().contentContainer;
//     // Button button = root.Q<Button>();
// }