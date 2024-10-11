using System.Collections;
using System.Collections.Generic;
using UFB.Entities;
using UFB.Map;
using UFB.StateSchema;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UFB.Events;
using UFB.Network.RoomMessageTypes;
using UnityEngine.TextCore.Text;
using UFB.Character;
using UFB.Core;
using UFB.Interactions;

namespace UFB.Events
{
    public class SpawnChangeEvent
    {
        public string tileId;
        public SpawnChangeEvent(string id)
        {
            tileId = id;
        }
    }
}


public class SpawnListPanel : MonoBehaviour
{
    [SerializeField]
    Transform scrolView;

    [SerializeField]
    ItemSection section;

    public string curTileId = "";

    public Dictionary<string, Tile> Tiles;
    public List<Tile> itemTileList = new List<Tile>();

    private void OnEnable()
    {
        EventBus.Subscribe<SpawnChangeEvent>(OnSpawnChangeEvent);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<SpawnChangeEvent>(OnSpawnChangeEvent);
    }

    private void OnSpawnChangeEvent(SpawnChangeEvent e)
    {
        curTileId = e.tileId;
    }

    public void AddSpawnItem(UFB.StateSchema.SpawnEntity spawnEntity, int i)
    {

        var tile = Tiles[spawnEntity.tileId];
        itemTileList.Add(tile);

        ItemSection item = Instantiate(section);
        item.transform.SetParent(scrolView);
        item.countText.text = (i + 1).ToString();
        item.target = tile.transform;
        item.tileId = spawnEntity.tileId;
        item.gameObject.SetActive(true);
        if(i == 0)
        {
            curTileId = item.tileId;
        }
    }

    public void InitSpawnItems(MapState mapState, Dictionary<string, Tile> tiles)
    {
        Tiles = tiles;
        int i = 0;
        foreach (UFB.StateSchema.SpawnEntity entity in mapState.spawnEntities.items.Values)
        {
            // var tile = Tiles[entity.tileId];
            Debug.Log(
                $"Spawning entity {entity.prefabAddress}"
            );
            if(entity.prefabAddress == "Entities/chest" /*|| entity.prefabAddress == "Entities/ItemBag"*/)
            {
                AddSpawnItem(entity, i);
                i++;
            }
        }
        HighlightRect.Instance.SetHighLightForSpawn(itemTileList);
    }

    public void OnConfirmClick()
    {
        if (curTileId == "") return; 
        EventBus.Publish(
            RoomSendMessageEvent.Create(
                "initSpawnMove",
                new RequestSpawnMessage
                {
                    tileId = curTileId,
                    destination = Tiles[curTileId].Coordinates,
                    playerId = UIGameManager.instance.controller.Id,
                }
            )
        );

        gameObject.SetActive( false );
        UIGameManager.instance.controller.InitMovePos(Tiles[curTileId]);
        ServiceLocator.Current.Get<CharacterManager>().PlayerCharacter.gameObject.SetActive( true );
        EventBus.Publish(
            new CameraOrbitAroundEvent(
                ServiceLocator.Current.Get<CharacterManager>().PlayerCharacter.transform,
                0.3f
            )
        );
    }

}
