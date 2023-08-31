using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UFB.UI
{

    // [RequireComponent(typeof(UIElements.UIPanel))]
    public class PageRenderer : MonoBehaviour
    {
        [SerializeField]
        private List<ScriptableObject> uiComponents; // List of scriptable objects implementing UIComponent

        private VisualElement root;

        private void Awake()
        {
            // var panel = GetComponent<UIElements.UIPanel>();
            // root = panel.visualTree;
        }

        private void Start()
        {
            RenderComponents();
        }

        private void RenderComponents()
        {
            foreach (var component in uiComponents)
            {
                // if (component is UIComponent uiComponent)
                // {
                //     var element = uiComponent.Render();
                //     root.Add(element);
                // }
            }
        }
    }
}