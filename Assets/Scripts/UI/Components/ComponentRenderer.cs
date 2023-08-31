using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

// [RequireComponent(typeof(UIDocument))]
namespace UFB.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class UIComponentRenderer : MonoBehaviour
    {
        public List<UIComponent> components; // whatever we want to dynamically 
        // inject into the document
        private UIDocument document;


        
    }
}