using UnityEngine;
using UFB.Entities;
using UFB.StateSchema;
using UFB.Camera;
using UFB.Events;

namespace UFB.Entities {
    public class Chest : MonoBehaviour, ISpawnableEntity, ICameraFocusable, IClickable
    {
        public SpawnEntity SpawnEntity => throw new System.NotImplementedException();

        public SpawnEntityParameters Parameters => throw new System.NotImplementedException();

        public void Initialize(SpawnEntity spawnEntity)
        {
            return;
        }

        public void OnClick()
        {
            // have some global way of determining click behavior of items,
            // which is informed by the ui manager and its current state
            OnFocus();
        }

        public void OnFocus()
        {
            Debug.Log("Chest focused");
            EventBus.Publish(new CameraOrbitAroundEvent(transform, 0.5f));
        }

        public void OnUnfocus()
        {
            // throw new System.NotImplementedException();
        }
    }

}
