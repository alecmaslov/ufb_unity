using UFB.Map;
using UnityEngine;

namespace UFB.Map
{
    public struct TileParameters
    {
        public string Id;
        public Coordinates Coordinates;
    }

    // some game tiles could be implemented differently depending on
    // what they have on them, etc
    public interface ITile
    {
        string Id { get; }
        Vector3 Position { get; }
        Coordinates Coordinates { get; }
        TileParameters Parameters { get; }
        void SetParameters(TileParameters parameters);
    }
}
