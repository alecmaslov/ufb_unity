using UnityEngine;
using UFB.Events;
using UFB.Map;
using UFB.Core;
using UFB.UI;
using UFB.Network.RoomMessageTypes;

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
        SelectItem,
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

    // Controls state of game interactivity, allowing users to be in different states/modes of interaction
    // such as selecting a tile, focusing an entity, or controlling the camera
    [RequireComponent(typeof(ClickObject))]
    public class InteractionManager : MonoBehaviour, IService
    {
        public InteractionMode Mode { get; private set; }
        private ClickObject _clickObject;

        private void Awake()
        {
            _clickObject = GetComponent<ClickObject>();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<InteractionModeChangeEvent>(OnInteractionModeChangeEvent);
            EventBus.Publish(new InteractionModeChangeEvent(InteractionMode.SelectItem));
            ServiceLocator.Current.Register(this);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<InteractionModeChangeEvent>(OnInteractionModeChangeEvent);
            ServiceLocator.Current.Unregister<InteractionManager>();
        }

        public void OnRaycastClicked(Transform transform, IClickable clickable)
        {
            if (Mode != InteractionMode.SelectItem)
            {
                return;
            }

            ServiceLocator.Current.Get<UIManager>().OnPopupMenuEvent(
                new PopupMenuEvent(
                    transform.name,
                    transform,
                    () => Debug.Log("Calling Cancel"),
                    new PopupMenuEvent.CreateButton[]
                    {
                        new(
                            "move",
                            () =>
                                EventBus.Publish(
                                    new RoomSendMessageEvent(
                                        "Move To",
                                        new RequestMoveMessage
                                        {
                                            tileId = transform.GetComponent<Tile>().Id,
                                            destination = transform.GetComponent<Tile>().Coordinates
                                        }
                                    )
                                )
                        ),


                        new(
                            "Focus",
                            () => EventBus.Publish(new CameraOrbitAroundEvent(transform, 0.3f))
                        )
                        // we should make CreateButton a bit more feature rich, so
                        // we can have certain types of buttons with certain icons
                        // new("Move To", () => EventBus.Publish<)
                    }
                )
            );
        }

        public void CycleInteractionMode()
        {
            switch (Mode)
            {
                case InteractionMode.FocusEntity:
                    EventBus.Publish(new InteractionModeChangeEvent(InteractionMode.SelectItem));
                    break;
                case InteractionMode.SelectItem:
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
                case InteractionMode.SelectItem:
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
