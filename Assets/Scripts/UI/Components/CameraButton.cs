using UnityEngine;
using UFB.Core;
using UFB.Camera;
using UFB.Events;
using UFB.Character;

namespace UFB.UI
{
    public class CameraButton : MonoBehaviour
    {
        private bool isTopDown = false;

        public void OnChangeViewClick()
        {
            // CHANGE THIS, this is ugly
            if (isTopDown)
            {
                EventBus.Publish(
                    new CameraOrbitAroundEvent(
                        ServiceLocator.Current.Get<CharacterManager>().PlayerCharacter.transform,
                        0.3f
                    )
                );
            }
            else
            {
                EventBus.Publish(
                    new SetCameraPresetStateEvent
                    {
                        presetState = CameraController.PresetState.TopDown
                    }
                );
            }
            isTopDown = !isTopDown;
        }
    }
}
/// cleaner
///
// public class CameraButton : MonoBehaviour
// {
//     private bool isTopDown = false;

//     private Transform _focusTransform;


//     public void SetFocusTransform(Transform focusTransform)
//     {
//         _focusTransform = focusTransform;
//     }

//     public void OnChangeViewClick()
//     {
//         // CHANGE THIS, this is ugly
//         if (isTopDown)
//         {
//             EventBus.Publish(
//                 new CameraOrbitAroundEvent(
//                     _focusTransform,
//                     0.3f
//                 )
//             );
//         }
//         else
//         {
//             EventBus.Publish(
//                 new SetCameraPresetStateEvent
//                 {
//                     presetState = CameraController.PresetState.TopDown
//                 }
//             );
//         }
//         isTopDown = !isTopDown;
//     }
// }
