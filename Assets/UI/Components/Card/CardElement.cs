using UnityEngine;
using UnityEngine.UIElements;

namespace UFB.UI
{
    // Define the custom control type.
    public class CardElement : VisualElement
    {
        // Expose the custom control to UXML and UI Builder.
        public new class UxmlFactory : UxmlFactory<CardElement> { }

        private VisualElement portraitImage => this.Q("image");
        private Label attackBadge => this.Q<Label>("attack-badge");
        private Label healthBadge => this.Q<Label>("health-badge");

        // Use the Init() approach instead of a constructor because 
        // we don't have children yet.
        public void Init(Texture2D image, int health, int attack)
        {
            portraitImage.style.backgroundImage = image;
            attackBadge.text = health.ToString();
            healthBadge.text = attack.ToString();
        }

        // Custom controls need a default constructor. 
        public CardElement() { }
    }
}