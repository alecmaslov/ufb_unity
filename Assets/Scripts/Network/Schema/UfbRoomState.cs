using Colyseus;
using Colyseus.Schema;
using UFB.Player;

namespace UFB.Network
{
    public class UfbRoomState : Schema
    {
        [Type(0, "string")]
        public string mySynchronizedProperty;

        [Type(1, "number")]
        public float boardWidth;

        [Type(2, "number")]
        public float boardHeight;

        [Type(3, "number")]
        public float turn;

        [Type(4, "map", typeof(MapSchema<PlayerState>))]
        public MapSchema<PlayerState> players = new MapSchema<PlayerState>();
    }

}