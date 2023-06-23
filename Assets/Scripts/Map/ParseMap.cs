using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UFB.Entities;

namespace UFB.Map {

    public enum TileType
    {
        Bridge,
        Floor,
        Void,
        Chest,
        Enemy,
        Portal
    }

    public class TileColor
    {
        public string Name { get; set; }
        public string Color { get; set; }
    }

    public class SpawnEntity
    {
        public string Name { get; set; }
        [JsonProperty("properties")]
        public Dictionary<string, object> Properties { get; set; }
    }

    public enum TileSide
    {
        Left,
        Right,
        Top,
        Bottom
    }

    public enum EdgeProperty
    {
        Wall,
        Bridge
    }

    public class TileEdge
    {
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public TileSide Side { get; set; }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public EdgeProperty EdgeProperty { get; set; }
    }

    public class Coordinates
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class GameTile
    {
        public string Id { get; set; }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public TileType Type { get; set; }
        public List<SpawnEntity> SpawnItems { get; set; }
        public Coordinates Coordinates { get; set; }
        public TileColor Color { get; set; }
        public string LayerName { get; set; }
        public List<TileEdge> Edges { get; set; }
        public string OriginalCode { get; set; }

        public Color GetColor()
        {
            Color color;
            try {
                ColorUtility.TryParseHtmlString(Color.Color, out color);

            } catch (System.Exception e) {
                Debug.Log("Error parsing color: " + e);
                color = UnityEngine.Color.white;
            }
            return color;
        }

        public Texture GetTexture(string mapName)
        {
            return Resources.Load($"Maps/{mapName}/Tiles/{Id}") as Texture;
        }

        // public GameObject SpawnTile()
        // {
        //     GameObject tile = null;
        //     switch (Type)
        //     {
        //         case TileType.Floor:
        //             tile = GameObject.Instantiate(Resources.Load("Prefabs/Tile")) as GameObject;
        //             break;
        //         case TileType.Bridge:
        //             tile = GameObject.Instantiate(Resources.Load("Prefabs/Bridge")) as GameObject;
        //             break;
        //         case TileType.Chest:
        //             tile = GameObject.Instantiate(Resources.Load("Prefabs/Chest")) as GameObject;
        //             break;
        //         case TileType.Enemy:
        //             tile = GameObject.Instantiate(Resources.Load("Prefabs/Enemy")) as GameObject;
        //             break;
        //         case TileType.Portal:
        //             tile = GameObject.Instantiate(Resources.Load("Prefabs/Portal")) as GameObject;
        //             break;
        //         case TileType.Void:
        //             tile = GameObject.Instantiate(Resources.Load("Prefabs/Void")) as GameObject;
        //             break;
        //     }

        //     tile.name = Id;
        //     tile.transform.position = new Vector3(Coordinates.X, 0, Coordinates.Y);
        //     tile.GetComponent<SpriteRenderer>().color = HexToColor(Color.Color);
        //     return tile;
        // }
    }

    public class UFBMap
    {
        public string Name { get; set; }
        public List<GameTile> Tiles { get; set; }
    }


    public class MapParser
    {
        public static UFBMap Parse(TextAsset mapJSON)
        {

            return JsonConvert.DeserializeObject<UFBMap>(mapJSON.ToString());
        }
    }
}
