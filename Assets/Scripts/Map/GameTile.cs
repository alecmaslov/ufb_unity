using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UFB.Entities;
using System.Linq;

namespace UFB.Map {

    public enum TileType
    {
        VerticalBridge,
        HorizontalBridge,
        DoubleBridge,
        Void,
        StairsNS,
        StairsSN,
        StairsEW,
        StairsWE,
        Upper,
        Lower,
        OpenTile,
        BlockNorth,
        BlockEast,
        BlockSouth,
        BlockWest,
        BlockNS,
        BlockEW,
        BlockNE,
        BlockES,
        BlockSW,
        BlockNW,
        BlockESW,
        BlockSWN,
        BlockWNE,
        BlockNES,
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
        Bottom,
        None
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

        public override string ToString() => $"[{EdgeProperty}: {Side}]";

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
            try
            {
                ColorUtility.TryParseHtmlString(Color.Color, out color);

            }
            catch (System.Exception e)
            {
                Debug.Log("Error parsing color: " + e + " Tile: " + Id);
                color = UnityEngine.Color.white;
            }
            return color;
        }

        public Texture GetTexture(string mapName)
        {
            return Resources.Load($"Maps/{mapName}/Tiles/{Id}") as Texture;
        }

        
        public TileSide SideOpposite(Coordinates other)
        {
            if (other.X > this.Coordinates.X)
                return TileSide.Left;
            if (other.X < this.Coordinates.X)
                return TileSide.Right;
            if (other.Y > this.Coordinates.Y)
                return TileSide.Bottom;
            if (other.Y < this.Coordinates.Y)
                return TileSide.Top;
            throw new System.Exception("Coordinates are not adjacent");
        }

        public override string ToString()
        {
            // return JsonConvert.SerializeObject(this);
            var edges = this.Edges.Select(edge => edge.ToString()).Aggregate((a, b) => a + ", " + b);
            return $"Tile: {Id} | Walls {edges} | Coordinates: {Coordinates}";
        }
    }
}

