using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor;

namespace UFB.UI
{

    public class TestComponent : VisualElement
    {
        #region Custom VisualElement boilerplate
        public new class UxmlFactory : UxmlFactory<TestComponent> { }

        public TestComponent() { }

        #endregion

        void OnEnable()
        {
            var myCustomElement = this.Q(className: "my-custom-element");
            myCustomElement.RegisterCallback<AttachToPanelEvent>(e =>
                { /* do something here when element is added to UI */ });
            myCustomElement.RegisterCallback<DetachFromPanelEvent>(e =>
                { /* do something here when element is removed from UI */ });
        }

        public TestComponent(SerializedProperty property, string label = "")
        {
            Init(property, label);
        }

        public void Init(SerializedProperty property, string label = "")
        {

        }

    }

}