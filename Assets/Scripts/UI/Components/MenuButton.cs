using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuButton : VisualElement
{
    public new class UxmlFactory : UxmlFactory<MainMenuButton> {}

    private Label buttonLabel => this.Q<Label>("button-label");

    public MainMenuButton()
    {
        // Default constructor
    }

    public void Init(string buttonText, System.Action clickAction)
    {
        buttonLabel.text = buttonText;
        RegisterCallback<ClickEvent>(evt => clickAction());
    }
}