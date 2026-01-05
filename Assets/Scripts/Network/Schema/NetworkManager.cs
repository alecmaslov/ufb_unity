using System.Collections;
using System.Collections.Generic;
using UFB.Core;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        var gameService = ServiceLocator.Current.Get<GameService>();
        if (gameService.Room == null)
        {
            Debug.LogError("Room is null");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
