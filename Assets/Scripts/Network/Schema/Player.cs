using Colyseus;
using Colyseus.Schema;
using UFB.Map;


namespace UFB.Network
{

    public class PlayerJoined : Schema
    {
        [Colyseus.Schema.Type(0, "string")]
        public string clientId;

        [Colyseus.Schema.Type(1, "boolean")]
        public bool isMe = false;

        [Colyseus.Schema.Type(2, "number")]
        public float x;

        [Colyseus.Schema.Type(3, "number")]
        public float y;

        public Coordinates Coordinates => new Coordinates((int)x, (int)y);
    }
}