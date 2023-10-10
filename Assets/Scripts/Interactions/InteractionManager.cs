using UnityEngine;
using UFB.Events;
using UFB.Map;
using UFB.Core;
using UFB.UI;

namespace UFB.Events
{
    public class InteractionModeChangeEvent
    {
        public Interactions.InteractionMode Mode { get; }

        public InteractionModeChangeEvent(Interactions.InteractionMode mode)
        {
            Mode = mode;
        }
    }
}

namespace UFB.Interactions
{
    public enum InteractionMode
    {
        FocusEntity,
        SelectTile,
        CameraControl
    }

    public interface IInteractionController
    {
        void OnClick();
        void OnFocus();
        void OnUnfocus();
    }

    // maybe an InteractionManager coordinates this stuff, and an interactionController
    // determines how that interaction occurs. When a certain mode is activated, the interactionController
    // is called to handle the interaction

    [RequireComponent(typeof(ClickObject))]
    public class InteractionManager : MonoBehaviour
    {
        public InteractionMode Mode { get; private set; }
        private ClickObject _clickObject;

        private void Awake()
        {
            _clickObject = GetComponent<ClickObject>();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<RaycastClickableEvent>(OnRaycastClickableEvent);
            EventBus.Subscribe<InteractionModeChangeEvent>(OnInteractionModeChangeEvent);
            EventBus.Publish(new InteractionModeChangeEvent(InteractionMode.FocusEntity));
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<RaycastClickableEvent>(OnRaycastClickableEvent);
            EventBus.Unsubscribe<InteractionModeChangeEvent>(OnInteractionModeChangeEvent);
        }

        private void OnRaycastClickableEvent(RaycastClickableEvent e)
        {
            Debug.Log(
                $"InteractionManager received RaycastClickableEvent: {e.Clickable.GetType()}"
            );

            // var screenPosition = e.Hit.transform.position;
            // Ray ray = UnityEngine.Camera.main.ScreenPointToRay(e.Hit.transform.position);
            var screenPosition = UnityEngine.Camera.main.WorldToScreenPoint(e.Hit.transform.position);

            ServiceLocator.Current.Get<UIManager>().ShowEntityPopupMenu(e.Hit.transform.gameObject, screenPosition);
            
            if (e.Clickable.GetType() == typeof(Tile) && Mode == InteractionMode.SelectTile)
            {
                e.Clickable.OnClick();
            }
        }

        public void CycleInteractionMode()
        {
            switch (Mode)
            {
                case InteractionMode.FocusEntity:
                    EventBus.Publish(new InteractionModeChangeEvent(InteractionMode.SelectTile));
                    break;
                case InteractionMode.SelectTile:
                    EventBus.Publish(new InteractionModeChangeEvent(InteractionMode.CameraControl));
                    break;
                case InteractionMode.CameraControl:
                    EventBus.Publish(new InteractionModeChangeEvent(InteractionMode.FocusEntity));
                    break;
            }
        }

        private void OnInteractionModeChangeEvent(InteractionModeChangeEvent e)
        {
            switch (e.Mode)
            {
                case InteractionMode.FocusEntity:
                    _clickObject.enabled = true;
                    break;
                case InteractionMode.SelectTile:
                    _clickObject.enabled = true;
                    break;
                case InteractionMode.CameraControl:
                    _clickObject.enabled = false;
                    break;
            }
            Mode = e.Mode;
        }
    }
}
