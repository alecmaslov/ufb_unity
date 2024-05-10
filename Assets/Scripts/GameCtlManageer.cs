using System.Collections;
using System.Collections.Generic;
using UFB.Character;
using UFB.Core;
using UFB.Events;
using UnityEngine;

public class GameCtlManageer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EventBus.Publish(
            new CameraOrbitAroundEvent(
                ServiceLocator.Current.Get<CharacterManager>().PlayerCharacter.transform,
                0.3f
            )
        );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
