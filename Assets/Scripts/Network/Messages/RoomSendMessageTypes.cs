using UFB.Map;

namespace UFB.Network.RoomMessageTypes
{
    public interface ISendMessage { }

    public class UfbRoomRules : ISendMessage
    {
        public int maxPlayers = 4;
        public int initHealth = 100;
        public int initEnergy = 100;
        public float turnTime = 60f * 3f;
    }

    public class UfbRoomCreateOptions : ISendMessage
    {
        public string mapName = "kaiju";
        public UfbRoomRules rules = new UfbRoomRules();
    }

    public class UfbRoomJoinOptions : ISendMessage
    {
        public string token;
        public string playerId;
        public string displayName;
        public string characterId;
        public string characterClass;
    }

    public class RequestMoveMessage : ISendMessage
    {
        public Coordinates destination;
        public string tileId;
        public float originEnergy;
        public string left;
        public string right;
        public string top;
        public string down;
    }

    public class RequestSpawnMessage : ISendMessage
    {
        public Coordinates destination;
        public string tileId;
        public string playerId;
    }

    public class RequestGetSpawnMessage : ISendMessage
    {
        public int itemId;
        public string playerId;
        public int powerId;
        public int coinCount;
    }

    public class RequestGetResourceMessage : ISendMessage
    {
        public string playerId;
    }
}
