using UnityEngine;
using UFB.Events;
using UFB.Interactions;
using TMPro;

namespace UFB.UI
{
    public class InteractionModeSelector : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _buttonText;

        private void OnEnable()
        {
            EventBus.Subscribe<InteractionModeChangeEvent>(OnInteractionModeChangeEvent);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<InteractionModeChangeEvent>(OnInteractionModeChangeEvent);
        }

        public void ChangeMode() { }

        private void OnInteractionModeChangeEvent(InteractionModeChangeEvent e)
        {
            _buttonText.text = e.Mode.ToString();
        }
    }
}
