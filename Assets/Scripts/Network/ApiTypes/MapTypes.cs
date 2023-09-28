using System;
using Colyseus.Schema;
using UFB.StateSchema;

namespace UFB.Network.ApiTypes
{
    [Serializable]
    public class Map
    {
        public string id;
        public string name;
        public string resourceAddress;
        public string thumbnailUrl;
        public string description;
        public string gridWidth;
        public string gridHeight;
        public string publisher;
    }

    public class MapTile
    {
        public string id;
        public string tileCode;
        public string mapId;
        public int x;
        public int y;
        public string type;
        public int[] walls;
        public string legacyCode;
    }

    
}
