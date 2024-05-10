using System.Collections;
using System.Collections.Generic;
using UFB.Events;
using UnityEngine;
using UnityEngine.UI;

public class ItemSection : MonoBehaviour, IClickable
{
    public Transform target;
    public string tileId;

    public void OnClick()
    {
        EventBus.Publish(new CameraOrbitAroundEvent(target, 0.5f));
        EventBus.Publish(new SpawnChangeEvent(tileId));
    }

    // Start is called before the first frame update
    public Text countText;

}
