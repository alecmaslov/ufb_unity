using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UFB.Entities;

namespace UFB.Map
{


    public class UFBMap
    {
        public string Name { get; set; }
        public List<GameTile> Tiles { get; set; }
        public int Dimensions { get => (int)Mathf.Sqrt(Tiles.Count); }

        public Color[] GetUniqueColors()
        {
            List<Color> colors = new List<Color>();
            foreach (var tile in Tiles)
            {
                if (!colors.Contains(tile.GetColor()))
                {
                    colors.Add(tile.GetColor());
                }
            }
            return colors.ToArray();
        }
    }


    public class MapParser
    {
        public static UFBMap Parse(TextAsset mapJSON)
        {

            return JsonConvert.DeserializeObject<UFBMap>(mapJSON.ToString());
        }
    }
}
