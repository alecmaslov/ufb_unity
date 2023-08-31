using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;


namespace UFB.UI
{
    public abstract class UIComponent : ScriptableObject
    {
        public VisualTreeAsset treeAsset;
        public abstract VisualElement Render();
    }

}