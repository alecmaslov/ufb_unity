using System.Collections;
using System.Collections.Generic;
using UFB.Character;
using UFB.Core;
using UFB.Events;
using UFB.Interactions;
using UFB.Map;
using UFB.Network.RoomMessageTypes;
using UnityEngine;
using UnityEngine.UI;

public class SelectSpawnPanel : MonoBehaviour
{
    public Text posText;

    [HideInInspector]
    public Tile tile;

    [HideInInspector]
    public string playerId = string.Empty;

    public void InitSpawnData(Tile _tile, string _playerId)
    {
        tile = _tile;
        playerId = _playerId;

        posText.text = tile.TilePosText;

        gameObject.SetActive(true);
    }

    public void OnConfirmSpawn()
    {

        HighlightRect.Instance.ClearHighLightRect();
        EventBus.Publish(
            RoomSendMessageEvent.Create(
                "initSpawnMove",
                new RequestSpawnMessage
                {
                    tileId = tile.Id,
                    destination = tile.Coordinates,
                    playerId = playerId,
                }
            )
        );
        UIGameManager.instance.controller.InitMovePos(tile);

        ServiceLocator.Current.Get<CharacterManager>().PlayerCharacter.gameObject.SetActive(true);
        EventBus.Publish(
            new CameraOrbitAroundEvent(
                ServiceLocator.Current.Get<CharacterManager>().PlayerCharacter.transform,
                0.3f
            )
        );
        CameraManager.instance.SetTarget(UIGameManager.instance.controller.transform);
        CameraManager.instance.OnActiveInputAction();

        InteractionManager.Instance.isSpawn = false;

        gameObject.SetActive ( false );
    }

}
